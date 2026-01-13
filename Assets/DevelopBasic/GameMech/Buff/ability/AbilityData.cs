using BattleActor;
using UnityEngine;

namespace BattleBuff.Ability
{
    public abstract class AbilityData : BuffData
    {
        [SerializeField] private BuffData[] buffs;
        [SerializeField, Tooltip("-1表示可触发无数次")] protected int triggerCount = -1;
        [SerializeField, Tooltip("当技能触发，但处于冷却时，消耗间隔次数")] protected int coolDownCount = 0;  
        [SerializeField] protected float coolDownTime = 10f;
        [SerializeField] protected float preCastTime = 0f;
        [SerializeField] protected TeamMask areaTeamMask;
        [SerializeField] protected float areaRadius;
        protected abstract bool IsArea();
        protected string[] GetAddBuffIDs()
        {
            // 获取所有Buff的ID
            string[] buffIDs = new string[buffs.Length];
            for (int i = 0; i < buffs.Length; i++)
            {
                buffIDs[i] = buffs[i].m_buffID;
            }
            return buffIDs;
        }
    }
}
