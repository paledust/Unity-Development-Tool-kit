using UnityEngine;

namespace BattleBuff
{
    public class BuffDataManager : Singleton<BuffDataManager>
    {
        [SerializeField] private BuffDataCollection buffDataCollection_SO;
        public Buff GetBuff(string buffID)
        {
            var buffData = buffDataCollection_SO.GetDataByKey(buffID);
            if (buffData == null)
                return null;
            else
                return buffData.GetBuff();
        }
        public BuffData GetBuffData(string buffID)
        {
            return buffDataCollection_SO.GetDataByKey(buffID);
        }
        public bool IsPositionBuff(string buffID)
        {
            var buffData = buffDataCollection_SO.GetDataByKey(buffID);
            return buffData.m_positionbasedBuff;
        }
    }
}