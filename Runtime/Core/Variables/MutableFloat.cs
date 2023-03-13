/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.03.09
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox.Properties
{
    /// <summary>
    /// 可变值类, Value = Constant || ∏(Multiply) * ( base + ∑(Flat) + base * ∑(Percent) )
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed partial class MutableFloat : MutableValue<float>
    {
        public MutableFloat(float baseValue = 0f) : base(baseValue) { }
        public MutableFloat() : this(0f) { }

        protected override float AddValue(float a, float b)
        {
            return a + b;
        }

        protected override float MulValue(float a, float b)
        {
            return a * b;
        }

        /// <summary>
        /// 使用其他的基础值计算效果
        /// </summary>
        /// <param name="otherBaseValue"></param>
        /// <param name="rounding">取整方向，小于0向下取整，等于0则四舍五入,大于0向上取整</param>
        /// <returns></returns>
        public long ComputeEffective(long otherBaseValue, int rounding = -1)
        {
            if (m_ConstMod.HasValue)
            {
                return (long)m_ConstMod.Value;
            }
            double _tempValue = otherBaseValue;
            if (m_FlatMod.HasValue)
            {
                _tempValue += m_FlatMod.Value;
            }
            if (m_PrecentMod.HasValue)
            {
                _tempValue *= m_PrecentMod.Value;
            }
            if (m_MultMod.HasValue)
            {
                _tempValue *= m_MultMod.Value;
            }
            return (long)(rounding < 0 ? _tempValue : (rounding == 0 ? Math.Round(_tempValue) : Math.Ceiling(_tempValue)));
        }

        /// <summary>
        /// 使用其他的基础值计算效果
        /// </summary>
        /// <param name="otherBaseValue"></param>
        /// <param name="rounding">取整方向，小于0向下取整，等于0则四舍五入,大于0向上取整</param>
        /// <returns></returns>
        public int ComputeEffective(int otherBaseValue, int rounding = -1)
        {
            if (m_ConstMod.HasValue)
            {
                return (int)m_ConstMod.Value;
            }
            float _tempValue = otherBaseValue;
            if (m_FlatMod.HasValue)
            {
                _tempValue += m_FlatMod.Value;
            }
            if (m_PrecentMod.HasValue)
            {
                _tempValue *= m_PrecentMod.Value;
            }
            if (m_MultMod.HasValue)
            {
                _tempValue *= m_MultMod.Value;
            }
            return (int)(rounding < 0 ? _tempValue : (rounding == 0 ? MathF.Round(_tempValue) : MathF.Ceiling(_tempValue)));
        }
    }
}