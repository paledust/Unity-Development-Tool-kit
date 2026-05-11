using System;
using UnityEngine;

namespace BattleBuff
{
    [System.Flags]
    public enum BuffTag
    {
        None = 0, //无Tag
        ClearableBuff = 1 << 1, //增益
        ClearableDebuff = 1 << 2, //减益
        Stun = 1 << 3, //控制
        AttributeModify = 1 << 4, //移动速度修改
        AttackModify = 1 << 5, //攻击数据修改
        ReplaceableUpgrade = 1 << 6, //可替换升级
        Zone = 1<<7, //区域Buff
    }
    public enum BuffState
    {
        Detached,   //挂起
        Pending,    //载入
        Working,    //生效中
        Complete,   //结束
    }
    public abstract class Buff
    {
        protected string buffTypeID;
        protected BuffTag buffTag = BuffTag.None;
        protected BuffTag buffImmuneTag = BuffTag.None;
        protected BuffHandler parent = null; //buff作用者
        protected GameObject buffCaster = null; //buff施加者
        protected BuffState buffState = BuffState.Detached;//buff生命状态

        public string m_buffTypeID => buffTypeID;
        public BuffTag m_buffTag => buffTag;
        public BuffTag m_buffImmuneTag => buffImmuneTag;
        public bool IsPending => buffState == BuffState.Pending;
        public bool IsDone => buffState == BuffState.Complete;
        public virtual bool m_isAnnonumous => false; //是否为匿名Buff

        public event Action onBuffStart;
        public event Action onBuffComplete;
        public event Action onBuffRemoved;

        //当buff被添加时，首先执行初始化，此时buff还未执行
        protected virtual void Initialize(BuffHandler parent)
        {
            this.parent = parent; 
        }
        public virtual void Initialize(BuffHandler parent, GameObject buffCaster)
        {
            Initialize(parent);
            this.buffCaster = buffCaster;
        }
        public void SetTag(BuffTag tag) => buffTag = tag;
        public void SetImmuneTag(BuffTag tag) => buffImmuneTag = tag;

        //修改buff的状态，标记buff完成状态=修改buff为complete，中途终止buff=修改buff为detached
        public void ChangeBuffState(BuffState newState) 
        {
            if (buffState == newState) return;
            buffState = newState;
            switch (buffState)
            {
                case BuffState.Working:
                    onBuffStart?.Invoke();
                    BuffBegin();
                    break;
                case BuffState.Complete:
                    onBuffComplete?.Invoke();
                    BuffComplete();
                    break;
                case BuffState.Detached:
                    onBuffRemoved?.Invoke();
                    BuffRemove();
                    break;
                default:
                    break;
            }
        }
        public virtual void RefreshBuff(Buff incomingBuff) { } //buff刷新后的行为，e.g：更新倒计时，延长buff时间
        public virtual void UpdateBuff() { } //buff生效的执行方式，e.g：执行倒计时

        protected virtual void BuffBegin() { } //buff生效后，e.g：修改属性
        protected virtual void BuffComplete() { } //buff销毁前，e.g：延迟爆炸
        protected virtual void BuffRemove() { } //buff销毁后，e.g：还原属性修改
    }

    #region 基本buff范例
    public abstract class AttributeModifierBuff : Buff
    {
        protected float modifier;
        protected AttributeModifyType attributeModifyType;
    }
    public class AddBuffBuff : Buff
    {
        private string buffID;
        public AddBuffBuff(string buffTypeID, string additionBuffID)
        {
            this.buffTypeID = buffTypeID;
            this.buffID = additionBuffID;
        }
        protected override void BuffBegin()
        {
            base.BuffBegin();
            parent.TryAddBuff(buffID);
            ChangeBuffState(BuffState.Complete);
        }
    }
    public class TagBuff : Buff
    {
        public TagBuff(string buffTypeID, BuffTag buffTag, BuffTag buffImmuneTag = BuffTag.None)
        {
            this.buffTypeID = buffTypeID;
            this.buffTag = buffTag;
            this.buffImmuneTag = buffImmuneTag;
        }
    }
    /// <summary>
    /// 倒数buff，每次刷新减少一次计数，计数为0时触发效果 或 每次倒计数时触发效果
    /// </summary>
    public abstract class CountDownBuff : Buff
    {
        public enum CountDownTriggerType
        {
            OnEachCountDown, //每次倒计时触发
            OnCountToZero,   //倒计时结束触发
        }
        private int maxCount = 1;
        private CountDownTriggerType triggerType;
        public CountDownBuff(string buffTypeID, int maxCount, CountDownTriggerType triggerType, BuffTag buffTag = BuffTag.None, BuffTag buffImmuneTag = BuffTag.None)
        {
            this.buffTypeID = buffTypeID;
            this.buffTag = buffTag;
            this.buffImmuneTag = buffImmuneTag;
            this.maxCount = maxCount;
            this.triggerType = triggerType;
        }
        public override void RefreshBuff(Buff incomingBuff)
        {
            if(incomingBuff is CountDownBuff incomingCountDownBuff)
            {
                maxCount --;
                switch(triggerType)
                {
                    case CountDownTriggerType.OnEachCountDown:
                        break;
                    case CountDownTriggerType.OnCountToZero:
                        break;
                }
                if(maxCount==0)
                {
                    ChangeBuffState(BuffState.Complete);
                }
            }
        }
        protected abstract void OnCountDownTrigger();
    }
    /// <summary>
    /// 倒数buff实例用途，倒数添加其余Buff
    /// </summary>
    public class CountDownAddBuff : CountDownBuff
    {
        protected string additionBuffID;
        public CountDownAddBuff(string buffTypeID, int maxCount, string additionBuffID, CountDownTriggerType triggerType, BuffTag buffTag = BuffTag.None, BuffTag buffImmuneTag = BuffTag.None)
            : base(buffTypeID, maxCount, triggerType, buffTag, buffImmuneTag)
        {
            this.additionBuffID = additionBuffID;
        }
        protected override void OnCountDownTrigger() => parent.TryAddBuff(additionBuffID);
    }
    #endregion

    #region 匿名Buff
    public abstract class AnnonomusBuff : Buff
    {
        public override bool m_isAnnonumous => true;
        public abstract void ExcuteBuff();
    }
    //基于地点执行的Buff
    public abstract class PositionBasedBuff : AnnonomusBuff
    {
        protected Vector2 controlPosition;
        protected GameObject caster; //用于buff执行的对象
        public void ExcuteBuffOnPosition(GameObject caster, Vector2 pos)
        {
            this.caster = caster;
            controlPosition = pos;
            ExcuteBuff();
        }
    }
    //基于地点创建Prefab的简易Buff
    public class BuffSpawnObj : PositionBasedBuff
    {
        private GameObject prefab;
        private int spawnCount;
        private float spawnRadius;
        public BuffSpawnObj(string buffTypeID, GameObject prefab, int spawnCount = 1, float spawnRadius = 0)
        {
            this.buffTypeID = buffTypeID;
            this.prefab = prefab;
            this.spawnCount = spawnCount;
            this.spawnRadius = spawnRadius;
        }
        public override void ExcuteBuff()
        {
            if (prefab != null)
            {
                for (int i = 0; i < spawnCount; i++)
                {
                    Vector2 spawnPos = controlPosition;
                    if (spawnRadius > 0)
                    {
                        spawnPos += UnityEngine.Random.insideUnitCircle * spawnRadius;
                    }
                    GameObject.Instantiate(prefab, spawnPos, Quaternion.identity);
                }
            }
        }
    }
    #endregion
}