using BattleActor;
using UnityEngine;

namespace BattleBuff.Ability
{
    //技能是一种特殊的buff，它负责设定特定的触发时机，并在触发时给特定对象赋予一系列特定buff
    public abstract class Ability : Buff
    {
        protected string[] buffIDs;

        //静态判定条件
        protected int maxTriggerCount;
        protected int maxIntersect = 0;
        protected float coolDownTime = 0f;

        //冷却计时
        protected int triggerCount;
        protected int intersectCount = 0;
        protected float coolDownTimer = 0f;
        protected bool isCooling = false;

        //作用范围相关
        protected float areaRadius;
        protected TeamMask areaTeamMask;

        //VFX
        protected string vfxAbility;

        public void SetAbilityVFX(string vfxKey) => vfxAbility = vfxKey;
        public override void UpdateBuff()
        {
            base.UpdateBuff();
            if(isCooling)
            {
                coolDownTimer += Time.deltaTime;
                if(coolDownTimer >= coolDownTime)
                {
                    coolDownTimer = 0f;
                    isCooling = false;
                }
            }
        }
        protected void UseAbilityToTarget(BuffHandler buffHandler, GameObject caster = null)
        {
            if(!CoolDownCheck())
                return;
            foreach (var buff in buffIDs)
            {
                buffHandler.TryAddBuff(buff, caster);
            }
        }
        protected void UseAbilityOnTargetsNearPos(Vector2 targetPos, GameObject caster = null)
        {
            if(!CoolDownCheck())
                return;
            //针对范围的技能，则获取范围内的目标
            //注意：该范围为瞬间范围，持续性范围采用生成zone的方式去处理
            var targets = BattleActorScanSystem.Instance.FindTargets<IBattleActor>(targetPos, areaRadius, ActorScanOrder.Default, areaTeamMask);
            foreach (var actor in targets)
            {
                var handler = actor.gameObject.GetComponent<BuffHandler>();
                if (handler != null)
                    UseAbilityToTarget(handler, caster);
            }
        }
        protected void UseAbilityOnPos(GameObject caster, Vector2 targetPos)
        {
            if(!CoolDownCheck())
                return;
            foreach (var buff in buffIDs)
            {
                if (BuffDataManager.Instance.IsPositionBuff(buff))
                {
                    (BuffDataManager.Instance.GetBuff(buff) as PositionBasedBuff).ExcuteBuffOnPosition(caster, targetPos);
                }
            }
        }
        protected bool CoolDownCheck()
        {
            if(intersectCount > 0 || isCooling)
                return false;
            return true;
        }
        protected void OnAbilityExcute()
        {
            //若处于冷却倒计时，只更新倒计时，不记录触发次数
            if(!isCooling)
            {
                coolDownTimer = 0f;
                isCooling = true;
                return;
            }

            if(intersectCount>0)
            {
                intersectCount --;
            }
            else
            {
                if (maxTriggerCount > 0)
                {
                    triggerCount++;
                    if (triggerCount >= maxTriggerCount)
                    {
                        ChangeBuffState(BuffState.Complete);
                    }
                }
                intersectCount = maxIntersect;
            }
        }
    }
}
