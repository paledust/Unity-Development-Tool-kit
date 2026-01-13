namespace BattleBuff.Ability
{
    public class DynamicAbility : Ability
    {
        protected DynamicCondition dynamicCondition;
        protected string[] additionAbilityID;
        protected bool repeatable;
        protected bool isValid;
        
        public DynamicAbility(string buffID, string[] abilityID, DynamicCondition dynamicCondition, bool repeatable)
        {
            this.buffTypeID = buffID;
            this.additionAbilityID = abilityID;
            this.dynamicCondition = dynamicCondition;
            this.repeatable = repeatable;
            this.isValid = false;
        }
        protected override void BuffBegin()
        {
            dynamicCondition.Initialize(parent.gameObject);
        }
        public override void UpdateBuff()
        {
            base.UpdateBuff();
            if(isCooling)
                return;
            if(dynamicCondition.EvaluateCondition())
            {
                if(!isValid)
                {
                    isValid = true;
                    foreach(var abilityID in additionAbilityID)
                    {
                        parent.TryAddBuff(abilityID);
                    }
                    if(!repeatable)
                    {
                        ChangeBuffState(BuffState.Complete);
                        return;
                    }
                }
            }
            else
            {
                if(isValid)
                {
                    isValid = false;
                    if(repeatable)
                    {
                        foreach(var abilityID in additionAbilityID)
                        {
                            parent.RemoveBuff(abilityID);
                        }
                    }
                }
            }
        }
    }
}
