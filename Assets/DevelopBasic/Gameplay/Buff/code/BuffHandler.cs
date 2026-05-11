using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleBuff
{
    public class BuffHandler : MonoBehaviour
    {
        private Dictionary<string, Buff> buffDict;
        private Action<Buff> onBuffCreated;
        public Buff GetBuff(string buffKey) => buffDict[buffKey];
        void Update()
        {
            if (buffDict == null) return;
            foreach (var buff in buffDict.Values.ToList())
            {
                if (buff.IsPending)
                    buff.ChangeBuffState(BuffState.Working);
                if (buff.IsDone)
                    HandleBuffComplete(buff);
                else
                {
                    buff.UpdateBuff();
                    if (buff.IsDone)
                        HandleBuffComplete(buff);
                }
            }
        }
        public void CleanUpAllBuff()
        {
            if (buffDict == null) return;
            foreach (var buff in buffDict.Values.ToList())
            {
                HandleBuffRemove(buff);
            }
            buffDict.Clear();
        }
        public bool TryAddBuff(string buffID, GameObject caster = null)
        {
            var buff = BuffDataManager.Instance.GetBuff(buffID);
            return TryAddBuffRaw(buff, buffID, caster);
        }
        public bool TryAddBuff(string buffID, string instanceKey, GameObject caster = null)
        {
            var buff = BuffDataManager.Instance.GetBuff(buffID);
            return TryAddBuffRaw(buff, instanceKey, caster);
        }
        public bool TryAddBuffRaw(Buff buff, string instanceKey, GameObject caster = null)
        {
            buff.Initialize(this, caster);
            //Todo：通过Buff Manager创建Buff，并调用Buff Awake
            if (buffDict == null)
                buffDict = new Dictionary<string, Buff>();
            //若存在免疫此buff的Tag，则不添加buff
            foreach (var go in buffDict.Values)
            {
                if ((go.m_buffImmuneTag & buff.m_buffTag) != BuffTag.None)
                {
                    return false;
                }
            }

            if (buffDict.ContainsKey(instanceKey))
            {
                buffDict[instanceKey].RefreshBuff(buff);
                //Todo:同类型buff，触发刷新buff事件
            }
            else
            {
                buffDict.Add(instanceKey, buff);
                foreach (var go in buffDict.Values.ToHashSet())
                {
                    //若存在buff被此tag免疫，则移除该buff
                    if ((buff.m_buffImmuneTag & go.m_buffTag) != BuffTag.None)
                    {
                        HandleBuffRemove(go);
                    }
                }
                buff.ChangeBuffState(BuffState.Pending);
            }
            onBuffCreated?.Invoke(buff);
            return true;
        }
        public bool TryAddBuffRaw(Buff buff) => TryAddBuffRaw(buff, buff.m_buffTypeID);
        public void RemoveBuff(string id)
        {
            if (buffDict.ContainsKey(id))
            {
                HandleBuffRemove(buffDict[id]);
            }
        }
        void HandleBuffRemove(Buff buff)
        {
            buffDict.Remove(buff.m_buffTypeID);
            buff.ChangeBuffState(BuffState.Detached);
        }
        void HandleBuffComplete(Buff buff)
        {
            buffDict.Remove(buff.m_buffTypeID);
            buff.ChangeBuffState(BuffState.Detached);
        }
    }
}