using UnityEngine;

namespace BattleActor
{
    public interface IAttackable
    {
        AttackData GetAttackData();
    }
    // 战场内基本元素，包括：单位、基地、建筑等等
    public interface IBattleActor
    {
        GameObject gameObject { get; }
        bool IsDead { get; }

        //Meta
        BattleActorType battleActorType { get; }
        TeamMask teamType { get; } //所属队伍
        bool IsTargetable { get; } //是否可被锁定

        //Properties
        float currentHealth { get; } //健康度
        float currentHealthRatio { get; } //血量百分比
        Vector2 position { get; } //Actor的位置坐标
        Vector2 aimCenter { get; } //Actor的瞄准中心点
        BattleActorMotionLayer motionLayer { get; } //Actor的位置层级（空中，地面）

        //Methods
        bool IsPosInRange(Vector2 pos, float range); //目标点是否在特定范围内
        float GetSqDistanceTo(Vector2 pos); //目标点与自身的距离平方
        float GetDistanceTo(Vector2 pos); //目标点与自身的距离
        Vector2 GetClosestPointTo(Vector2 pos); //不同类型的搜索目标会因为搜索的位置返回不同的搜索点
        Vector2 GetHitPos(Vector2 attackObjPos); //获取到受击点位置，普通单位可以返回自身瞄准位置，基地使用更靠近攻击者的位置
        void TakeDamage(AttackData attackData, Vector2 hitPos);
        void PushActor(Vector2 force); //推动Actor移动

        //判断层级mask是否包含移动层级
        public static bool IncludeMotionLayer(BattleActorMotionLayerMask mask, BattleActorMotionLayer layer) => (mask & MotionLayerToMotionLayerMask(layer)) != 0;
        //将单位的移动层级转换为层级BitMask
        static BattleActorMotionLayerMask MotionLayerToMotionLayerMask(BattleActorMotionLayer layer) => (BattleActorMotionLayerMask)(1 << (int)layer);
        public static bool IsNull(IBattleActor battleActor) => battleActor == null || battleActor.Equals(null);
        public static bool IsInvalid(IBattleActor battleActor) => IsNull(battleActor) || !battleActor.IsTargetable;
    }

    public enum BattleActorType
    {
        Unit = 0, //单位
        Basement = 1, //基地
        Building = 2, //建筑
    }
    public enum BattleActorMotionLayer
    {
        Surface = 0, //地面层
        Submerged = 1, //水下层
        Air = 2, //空中层
    }
    [System.Flags]
    public enum BattleActorMotionLayerMask
    {
        None = 0, //无层级
        Surface = 1 << BattleActorMotionLayer.Surface, //地面层
        Submerged = 1 << BattleActorMotionLayer.Submerged, //水下层
        Air = 1 << BattleActorMotionLayer.Air, //空中层
        All = Surface | Submerged | Air, //所有层级
    }
}