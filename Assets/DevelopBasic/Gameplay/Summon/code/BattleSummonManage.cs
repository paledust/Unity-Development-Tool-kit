using System.Collections.Generic;
using UnityEngine;
using BattleActor;

namespace BattleSummon
{
    //召唤管理器，与直接生成单位不同，召唤管理器能够动态检查是否满足召唤上限，以及根据key回收被召唤单位
    public class BattleSummonManage : Singleton<BattleSummonManage>
    {
        private bool isActivated = true; //是否开启召唤功能，当未开启时，召唤不会成功
        private Dictionary<string, HashSet<ISummonnee>> summonDict = new Dictionary<string, HashSet<ISummonnee>>();

        void OnEnable()
        {
            BattleSummonEventSystem.E_OnSummonneeRemoved += OnSummonneeRemove;
        }
        void OnDisable()
        {
            BattleSummonEventSystem.E_OnSummonneeRemoved -= OnSummonneeRemove;
        }
        public ISummonnee TryCreateSummon(BattleSummonArg summonArgs)
        {
            if (!isActivated)
            {
                return null;
            }

            int summonLimit = summonArgs.summonLimit;
            string summonnerID = summonArgs.summonnerID;

            //召唤前检查
            //是否创建字典
            if (summonDict == null)
                summonDict = new Dictionary<string, HashSet<ISummonnee>>();
            //是否包含召唤者ID
            if (!summonDict.ContainsKey(summonnerID))
                summonDict.Add(summonnerID, new HashSet<ISummonnee>());

            //是否超过数量限制
            if (summonLimit >= 0)
            {
                if (summonDict[summonnerID].Count == summonLimit)
                    return null;
                else if (summonDict[summonnerID].Count > summonLimit)
                {
                    Debug.LogError($"有超过数量上限的召唤物出现!!! Summonner ID: {summonArgs.summonnerID}, Summonnee Name: {summonArgs.summonneeKey}");
                    return null;
                }
            }

            //检查是否可以修正召唤位置
            ISummonnee summon = null;
            switch (summonArgs.summonneeType)
            {
                case BattleActorType.Unit:
                    summon = BattleSummonEventSystem.Call_OnSummonUnit(summonArgs);
                    break;
                case BattleActorType.Building:
                    summon = BattleSummonEventSystem.Call_OnSummonBuilding(summonArgs);
                    break;
            }
            if (summon == null)
            {
                Debug.LogError($"创建召唤物失败! Summonner ID: {summonnerID}, Summonnee Name: {summonArgs.summonneeKey}");
                return null;
            }
            summon.OnSummon(summonnerID);

            //召唤后存入列表
            summonDict[summonnerID].Add(summon);

            return summon;
        }
        public int GetCurrentSummonAmount(string summonerID)
        {
            if (summonDict.ContainsKey(summonerID))
                return summonDict[summonerID].Count;
            else
                return 0;
        }
        void CleanUpSummonDict()
        {
            if (summonDict != null)
                summonDict.Clear();
        }
        #region Event Function
        void OnSummonneeRemove(ISummonnee summonnee)
        {
            string summonneeName = summonnee.m_summonneeName;
            string summonnerID = summonnee.m_summonnerID;

            if (summonDict.ContainsKey(summonnerID))
            {
                if (summonDict[summonnerID].Count > 0)
                    summonDict[summonnerID].Remove(summonnee);
                else
                    Debug.LogAssertion($"召唤物数量错误!!! Summonner ID: {summonnerID}, Summonnee Name: {summonneeName}");
            }
            else
            {
                Debug.LogWarning($"未找到召唤者ID! Summonner ID: {summonnerID}, Summonnee Name: {summonneeName}");
            }
        }
        #endregion
    }
}
