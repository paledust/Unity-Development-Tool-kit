using System;
using BattleActor;
using UnityEngine;

namespace BattleBuff.Ability
{
    public enum ComparisonType
    {
        Less,
        LessEqual,
        Equal,
        Greater,
        GreaterEqual
    }
    public abstract class DynamicCondition
    {
        public abstract void Initialize(GameObject context);
        public abstract bool EvaluateCondition();
    }
}