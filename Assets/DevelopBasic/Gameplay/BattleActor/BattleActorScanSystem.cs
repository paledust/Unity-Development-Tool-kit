using System;
using System.Collections.Generic;
using UnityEngine;
using SimpleShuffle;

using Random = UnityEngine.Random;
using System.Linq;

namespace BattleActor
{
    [System.Flags]
    public enum TeamMask
    {
        None = 0, //中立方
        Player = 1 << 1, //玩家方
        Enemy = 1 << 2, //敌方
        Both = Player | Enemy, //完全敌对
    }
    [System.Flags]
    public enum TeamRelation
    {
        None = 0, //中立
        SameSide = 1 << 1, //友方
        OppositeSide = 1 << 2, //敌方
        Both = SameSide | OppositeSide, //全部
    }
    public enum ActorScanOrder
    {
        Default = 0,
        Random = 1, //随机
        CloserDistance = 2, //距离近到远
        HigherHealth = 3, //生命值从大到小
        LowerHealthRatio = 4, //生命值百分比从小到大
    }

    public class BattleActorScanSystem : Singleton<BattleActorScanSystem>
    {
        private Collider2D[] physicsResults;
        private const int MAX_SEARCH_NUM = 64;

        #region Helper
        private static ContactFilter2D TeamToContactFilter2D(TeamMask teamMask)
        {
            ContactFilter2D filter2D = new ContactFilter2D();
            filter2D.useTriggers = true;
            filter2D.useLayerMask = true;
            filter2D.layerMask = BattleActorService.TeamLayerMasks[teamMask];

            return filter2D;
        }
        private static ContactFilter2D LayerMaskToContactFilter2D(LayerMask layerMask)
        {
            ContactFilter2D filter2D = new ContactFilter2D();
            filter2D.useTriggers = true;
            filter2D.useLayerMask = true;
            filter2D.layerMask = layerMask;

            return filter2D;
        }
        #endregion

        //核心索敌逻辑
        protected List<T> FindTargetInRange<T>(Vector2 pos, float scanRange,
            ContactFilter2D contactFilter2D, Predicate<T> optionalCondition = null, int limit = -1) where T : IBattleActor
        {
            if (physicsResults == null) physicsResults = new Collider2D[MAX_SEARCH_NUM]; //如果没使用过物理搜索，则创建新的搜索list
            int count = Physics2D.OverlapCircle(pos, scanRange, contactFilter2D, physicsResults);
            if (count <= 0) return null; //如果没有搜索到任何结果，则返回null
            if (limit > 0) //限制目标数量
                count = Mathf.Min(count, limit);

            List<T> targetSet = null;
            T go;
            for (int i = 0; i < count; i++)
            {
                go = physicsResults[i].GetComponent<T>();
                if (IBattleActor.IsInvalid(go)) continue; //舍弃无效目标
                if (optionalCondition != null && !optionalCondition(go)) continue; //舍弃不满足条件目标
                if (targetSet == null)
                    targetSet = new List<T>();
                targetSet.Add(go);
            }
            return targetSet;
        }

        #region 多个搜索
        //根据队伍标签搜索敌人
        public List<T> FindTargets<T>(Vector2 scanCenter, float scanRange, ActorScanOrder scanOrder,
                                      TeamMask teamMask = TeamMask.Enemy,
                                      Predicate<T> optionalCondition = null,
                                      int limit = -1) where T : IBattleActor
            => FindTargets(scanCenter, scanRange, scanOrder, BattleActorService.TeamLayerMasks[teamMask], optionalCondition, limit);
        //根据layermask搜索敌人
        public List<T> FindTargets<T>(Vector2 scanCenter, float scanRange, ActorScanOrder scanOrder,
                                      LayerMask layerMask,
                                      Predicate<T> optionalCondition = null,
                                      int limit = -1) where T : IBattleActor
        {
            switch (scanOrder)
            {
                case ActorScanOrder.CloserDistance:
                    return FindTargetInRange_CloserDistance<T>(scanCenter, scanRange, LayerMaskToContactFilter2D(layerMask),
                    optionalCondition, limit);
                case ActorScanOrder.Random:
                    return FindTargetInRange_Randomly<T>(scanCenter, scanRange, LayerMaskToContactFilter2D(layerMask),
                    optionalCondition, limit);
                case ActorScanOrder.HigherHealth:
                    return FindTargets_HigherHealth<T>(scanCenter, scanRange, LayerMaskToContactFilter2D(layerMask),
                    optionalCondition, limit);
                case ActorScanOrder.LowerHealthRatio:
                    return FindTargets_LowerHealthRatio<T>(scanCenter, scanRange, LayerMaskToContactFilter2D(layerMask),
                    optionalCondition, limit);
                default:
                    return FindTargetInRange<T>(scanCenter, scanRange, LayerMaskToContactFilter2D(layerMask),
                    optionalCondition, limit);
            }
        }
        #endregion

        #region 单个搜索
        public bool TryGetClosestTargetInRange<T>(out T actor, Vector2 pos, float scanRange,
            TeamMask teamMask, Predicate<T> optionalCondition = null) where T : IBattleActor
        {
            var targetList = FindTargetInRange_CloserDistance(pos, scanRange, TeamToContactFilter2D(teamMask), optionalCondition, -1);
            if (targetList == null || targetList.Count == 0)
            {
                actor = default;
                return false;
            }
            actor = targetList[0];
            return true;
        }
        public bool TryGetAnyTargetInRange<T>(out T actor, Vector2 pos, float scanRange,
            TeamMask teamMask, Predicate<T> optionalCondition = null) where T : IBattleActor
        {
            var targetList = FindTargetInRange(pos, scanRange, TeamToContactFilter2D(teamMask), optionalCondition);
            if (targetList != null && targetList.Count > 0)
            {
                actor = targetList[Random.Range(0, targetList.Count)];
                return true;
            }
            actor = default;
            return false;
        }
        #endregion

        #region Scan Helper
        //索敌范围内按生命值百分比从大到小排序，用于寻找最健康的目标
        protected List<T> FindTargets_HigherHealth<T>(Vector2 pos, float scanRange,
            ContactFilter2D filter2D, Predicate<T> optionalCondition = null, int limit = -1) where T : IBattleActor
        {
            List<T> collection = FindTargetInRange<T>(pos, scanRange, filter2D, optionalCondition);
            if (collection == null)
                return null;
            collection.Sort((x, y) => -x.currentHealthRatio.CompareTo(y.currentHealthRatio));
            if(limit>0)
                collection = collection.GetRange(0, Mathf.Min(limit, collection.Count));
            return collection;
        }
        //索敌范围内按生命值百分比从小到大排序，可用于寻找最虚弱的目标
        protected List<T> FindTargets_LowerHealthRatio<T>(Vector2 pos, float scanRange,
            ContactFilter2D filter2D, Predicate<T> optionalCondition, int limit) where T : IBattleActor
        {
            List<T> collection = FindTargetInRange<T>(pos, scanRange, filter2D, optionalCondition);
            if (collection == null)
                return null;
            collection.Sort((x, y) => x.currentHealthRatio.CompareTo(y.currentHealthRatio));
            if(limit>0)
                collection = collection.GetRange(0, Mathf.Min(limit, collection.Count));
            return collection;
        }
        //索敌范围内按距离近到远排序
        protected List<T> FindTargetInRange_CloserDistance<T>(Vector2 pos, float scanRange,
            ContactFilter2D filter2D, Predicate<T> optionalCondition, int limit) where T : IBattleActor
        {
            List<T> collection = FindTargetInRange<T>(pos, scanRange, filter2D, optionalCondition);
            if (collection == null)
                return null;
            collection.Sort((x, y) => x.GetSqDistanceTo(pos).CompareTo(y.GetSqDistanceTo(pos)));
            if(limit>0)
                collection = collection.GetRange(0, Mathf.Min(limit, collection.Count));
            return collection;
        }
        protected List<T> FindTargetInRange_Randomly<T>(Vector2 pos, float scanRange,
            ContactFilter2D filter2D, Predicate<T> optionalCondition, int limit) where T : IBattleActor
        {
            List<T> collection = FindTargetInRange<T>(pos, scanRange, filter2D, optionalCondition);
            if (collection == null)
                return null;
            ShuffleHelper.Shuffle(ref collection);
            if(limit>0)
                collection = collection.GetRange(0, Mathf.Min(limit, collection.Count));
            return collection;
        }
        #endregion
    }
}
