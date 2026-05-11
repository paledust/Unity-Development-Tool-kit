using UnityEngine;

namespace BattleBuff
{
    public abstract class BuffData : ScriptableObject
    {
        [SerializeField] protected string buffTypeID;
        [SerializeField] private BuffTag buffTag;
        [SerializeField] private BuffTag immuneTag;
        public BuffTag m_buffTag => buffTag;
        public BuffTag m_immuneTag => immuneTag;
        
        public virtual string m_buffID => this.name;
        public virtual bool m_positionbasedBuff => false;
        
        public Buff GetBuff()
        {
            var buff = GetBuffInstance();
            buff.SetTag(m_buffTag);
            buff.SetImmuneTag(m_immuneTag);
            return buff;
        }
        protected abstract Buff GetBuffInstance();
    }
}