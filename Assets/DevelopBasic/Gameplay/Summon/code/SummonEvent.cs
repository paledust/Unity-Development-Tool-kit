using System;

namespace BattleSummon
{
    public static class BattleSummonEventSystem
    {
        public static Func<BattleSummonArg, ISummonnee> E_OnSummonBuilding;
        public static ISummonnee Call_OnSummonBuilding(BattleSummonArg summonArgs) => E_OnSummonBuilding?.Invoke(summonArgs);
        public static Func<BattleSummonArg, ISummonnee> E_OnSummonUnit;
        public static ISummonnee Call_OnSummonUnit(BattleSummonArg summonArgs) => E_OnSummonUnit?.Invoke(summonArgs);
        public static Action<ISummonnee> E_OnSummonneeRemoved;
        public static void Call_OnSummonneeRemoved(ISummonnee summonnee) => E_OnSummonneeRemoved?.Invoke(summonnee);
    }
}