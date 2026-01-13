using UnityEngine;

namespace BattleSummon
{
    public interface ISummonnee
    {
        GameObject gameObject{ get; }
        string m_summonnerID { get; }
        string m_summonneeName { get; }
        void OnSummon(string summonnerID); //召唤时执行
        void ApplySummonModify<T>(T buffArgs); //添加召唤修改，召唤成功之后可执行
    }
}