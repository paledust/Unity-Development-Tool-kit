namespace BattleActor
{
    
    //属性类型
    public enum BattleActorAttributeType
    {
        Health = 0, //健康度
        Armor = 1, //护甲
        Shield = 2, //护盾
        Damage = 3, //伤害
        AttackSpeed = 4, //攻击速度
        CriticRate = 5, //暴击率
        CriticDamage = 6, //暴击伤害倍率
        MoveSpeed = 7, //移动速度
        MaxHealth = 8, //最大健康度
        MaxArmor = 9, //最大护甲
        MaxShield = 10, //最大护盾
    }

    //元素类型
    public enum ElementType
    {
        Physical = 0,//物理
        Cryo = 1,    //冰冻
        Electro = 2, //电磁  
        Thermal = 3, //热能
        Biochemical = 4, //生化
    }
    //基本攻击数据
    public struct AttackData
    {
        public float damage{ get; private set; } //基础伤害
        public readonly ElementType damageElement;
        public readonly float damageToBuilding; //对建筑伤害
        public readonly float damageToShield; //对护盾伤害
        public readonly float criticRate;
        public readonly float criticDamageMulti;
        public readonly bool penetrateArmor; //护甲穿透
        public readonly string damageSourceID; //伤害来源ID，可用于统计伤害，判断伤害来源单位属性等等

        public const float MAX_DAMAGE = 100000000; //攻击数值最大值，攻击数值超过此值，不再显示，默认是作为自杀
        public const float BASIC_CRITIC_MULTI = 1.5f; //基础暴击伤害倍率，暴击伤害 = 攻击伤害 * 暴击倍率
        public static readonly AttackData InstantKillAttack = new AttackData(MAX_DAMAGE, 0, 0, true, 0, 0, ElementType.Physical);
        
        public AttackData(float damage, float damageToBuilding, float damageToShield, bool penetrateArmor, float criticRate, float criticDamage, ElementType elementType, 
                            string damageID="")
        {
            this.damage = damage;
            this.damageToShield = damageToShield;
            this.damageToBuilding = damageToBuilding;
            this.penetrateArmor = penetrateArmor;
            this.criticRate = criticRate;
            this.criticDamageMulti = criticDamage;
            this.damageElement = elementType;
            this.damageSourceID = damageID;
        }
        public void ResetDamage(float resetValue)
        {
            damage = resetValue;
        }
    }
}