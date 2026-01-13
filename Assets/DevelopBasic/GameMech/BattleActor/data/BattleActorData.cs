using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BattleActor
{
    public abstract class BattleActorData : ScriptableObject
    {
        public abstract BattleActorType actorType{ get; }
        public float health = 10f;
        public float shield = 0f;

        public bool penetrateArmor = false;
        public float attackRange = 2f;
        public float attackRangeAdjustment = 0f;
        public ElementType damageType = ElementType.Physical;
        public float attackDamage = 1;
        public float damageMultiToBuilding = 0;
        public float damageMultiToShield = 0;
        public float criticRate;
        public float criticDamageMultiplier;
        public float attackSpeed = 1;
        public string m_actorKey => this.name;

        protected virtual bool IsRange() => true;
    }
}