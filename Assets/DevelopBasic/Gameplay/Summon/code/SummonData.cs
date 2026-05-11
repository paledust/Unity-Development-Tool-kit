using BattleActor;
using UnityEngine;

namespace BattleSummon
{
    public struct BattleSummonArg
    {
        public readonly BattleActorType summonneeType; //召唤物种类
        public readonly string summonneeKey; //召唤物名称
        public readonly string summonnerID; //召唤者ID
        public readonly bool isEnemy; //召唤物阵营
        public readonly int summonneeLevel; //召唤物等级

        #region 动态数据
        public Vector3 summonPosition; //召唤物位置
        public int summonLimit; //召唤物最大数量, -1表示无限
        #endregion

        public BattleSummonArg(BattleActorData summonneeData, int summonLimit, Vector3 summonPosition, string summonnerID, bool isEnemy, int summonLevel)
        {
            summonneeType = summonneeData.actorType;
            summonneeKey = summonneeData.m_actorKey;

            this.summonPosition = summonPosition;
            this.summonnerID = summonnerID;
            this.isEnemy = isEnemy;
            this.summonneeLevel = summonLevel;
            this.summonLimit = summonLimit;
        }
    }
}