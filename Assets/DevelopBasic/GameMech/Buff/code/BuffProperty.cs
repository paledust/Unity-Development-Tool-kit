/** 
BuffProperty用于处理复杂的数值加成，采用的基本公式为 动态数据 = (基础数据 * (1+百分比加成) + 加成) * 倍率
类似最大生命值，攻击力等等都可以通过使用BuffProperty包装。
其特点是，加成运算类型复杂，并且希望加成顺序不同时，也可以保证一个相同的结果。

注意1：加成并非只有增益，也需要考虑减益。
注意2：并非所有可加成数值都需要包装为BuffProperty。例如子弹的散射数，召唤上限这类参数，
其特点是其加成方式只由单一的运算规则决定（ e.g：最终子弹散射 = 基础值 + 加成值 ），因此直接修改数值会更快捷。
注意3：BuffProperty会引入额外的数据，因此当单位数量庞大，且可修改数值类型庞大时，会占用更多空间。
因此可以根据实际情况区分不同类的数值：
例如一个英雄有更多增益，可以让它的参数里包含更多buffProperty，一个普通敌人一般不需要太多复杂增益，可以让参数里包含少量buffProperty
模块中的UnitBase的动态数据多数为buffProperty，作为结构示范，实际根据需要增减。
**/

using UnityEngine;

namespace BattleBuff
{
    public enum AttributeModifyType
    {
        Add = 0,
        AddPercentage = 1,
        Multiply = 2,
    }
    [System.Serializable]
    //可修改的float数值
    public struct BuffProperty
    {
        public float baseValue { get; private set; }
        [SerializeField] private float _cachedValue;
        private float bonus;
        private float riser;
        private float multiplier;
        private float maxValue;
        private bool isInt;

        public float cachedValue => _cachedValue;

        public BuffProperty(float value, float max = -1, bool isInt = false)
        {
            baseValue = _cachedValue = value;
            bonus = 0;
            riser = 0;
            multiplier = 1;
            maxValue = max;
            this.isInt = isInt;
        }
        private void RecalculateValue()
        {
            _cachedValue = (baseValue * (1 + riser) + bonus) * multiplier;

            if (maxValue >= 0)
                _cachedValue = Mathf.Min(_cachedValue, maxValue);
            if (isInt)
                _cachedValue = Mathf.RoundToInt(_cachedValue);
        }
        //修改基础参数，例如全局加成
        public void OverrideBaseValue(float newValue)
        {
            baseValue = newValue;
            RecalculateValue();
        }

        /// <summary>
        /// 重置所有非初始值为零，初始记录数据除外。
        /// </summary>
        public void Reset()
        {
            bonus = 0;
            riser = 0;
            multiplier = 1;
            RecalculateValue();
        }

        /// <summary>
        /// 以当前值为基础值，并重置所有加成。此操作会丢失初始值。
        /// </summary>
        public void RebaseValue()
        {
            baseValue = cachedValue;
            Reset();
        }
        //修改当前数值
        public void ModifyValue(float modifier, AttributeModifyType modifiType)
        {
            switch (modifiType)
            {
                case AttributeModifyType.Add:
                    bonus += modifier;
                    break;
                case AttributeModifyType.AddPercentage:
                    riser += modifier;
                    break;
                case AttributeModifyType.Multiply:
                    multiplier *= modifier;
                    break;
            }
            RecalculateValue();
        }
        public static float GetReverseModifier(float modifier, AttributeModifyType modifiType)
        {
            float reverseValue;
            switch (modifiType)
            {
                case AttributeModifyType.Add:
                case AttributeModifyType.AddPercentage:
                    reverseValue = -modifier;
                    break;
                case AttributeModifyType.Multiply:
                    reverseValue = 1 / modifier;
                    break;
                default:
                    reverseValue = 0;
                    break;
            }
            return reverseValue;
        }
    }
}