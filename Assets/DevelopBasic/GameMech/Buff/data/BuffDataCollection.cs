using UnityEngine;

namespace BattleBuff
{
    [CreateAssetMenu(fileName = "BuffDataCollection", menuName = "Assets/Buff/BuffDataCollection")]
    public class BuffDataCollection : DataCollection<BuffData>
    {
        public override BuffData GetDataByKey(string key) => DataList.Find(x => x.m_buffID == key);
    }
}
