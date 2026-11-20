/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.21
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;

namespace PBBox.Properties
{
    /// <summary>
    /// <para>默认浮点数类型的ValueMod，无参数构造时使用默认的FloatMod定义</para>
    /// <para>默认的公式为Value = MaxOrOrder(Constant) || ∏(Multiply) * ( base + ∑(Flat) + base * ∑(Percent) )</para>
    /// </summary>
    public sealed class FloatMod : ValueMod<float, float>
    {
        public const string ModConstant = "Constant";
        public const string ModMultiply = "Multiply";
        public const string ModFlat = "Flat";
        public const string ModPercent = "Percent";

        /// <summary>
        /// 默认的FloatMod定义，
        /// 提供Value = MaxOrOrder(Constant) || ∏(Multiply) * ( base + ∑(Flat) + base * ∑(Percent) )的计算方式
        /// </summary>
        public static ValueModBuildParam<float, float> DefaultFloatModBuildParam = new ValueModBuildParam<float, float>(
            new (string nameTag, ValueModPartCombineFunc<float> combineModFunc)[]
            {
                (ModMultiply, CombineValueModMultiply),
                (ModFlat, CombineValueModAdd),
                (ModPercent, CombineValueModAdd),
                (ModConstant, CombineValueModMaxAndOrder),
            },
            computeValue: ComputeValueDefault
        //getPartIndexByName: name => //?量少，使用遍历可能更快，待测试
        //{
        //    return name switch
        //    {
        //        ModMultiply => 0,
        //        ModFlat => 1,
        //        ModPercent => 2,
        //        ModConstant => 3,
        //        _ => -1
        //    };
        //}
        );

        /// <summary>
        /// 计算最终的浮点值，按照 Constant || Multiply * ( base + Flat + base * Percent ) 的顺序进行计算
        /// </summary>
        /// <param name="mods"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        public static float ComputeValueDefault(float?[] mods, float baseValue)
        {
            float? mult = mods[0];
            float? flat = mods[1];
            float? percent = mods[2];
            float? constant = mods[3];
            if (constant.HasValue)
            {
                return constant.Value;
            }
            float result = baseValue;
            if (flat.HasValue)
            {
                result += flat.Value;
            }
            if (percent.HasValue)
            {
                result += baseValue * percent.Value;
            }
            if (mult.HasValue)
            {
                result *= mult.Value;
            }
            return result;
        }

        /// <summary>
        /// 叠加所有的mod值
        /// </summary>
        /// <param name="mods"></param>
        /// <returns></returns>
        public static (float? result, bool forceRefreshNext) CombineValueModAdd(Dictionary<int, (float, int)> mods)
        {
            if (mods == null || mods.Count == 0)
            {
                return (null, false);
            }
            float result = 0;
            foreach (var mod in mods.Values)
            {
                result += mod.Item1;
            }
            return (result, false);
        }

        /// <summary>
        /// 计算所有mod的乘积
        /// </summary>
        /// <param name="mods"></param>
        /// <returns></returns>
        public static (float? result, bool forceRefreshNext) CombineValueModMultiply(Dictionary<int, (float, int)> mods)
        {
            if (mods == null || mods.Count == 0)
            {
                return (null, false);
            }
            float result = 1;
            foreach (var mod in mods.Values)
            {
                result *= mod.Item1;
            }
            return (result, false);
        }

        /// <summary>
        /// 返回最大Order或者同Order中的最大值
        /// </summary>
        /// <param name="mods"></param>
        /// <returns></returns>
        public static (float? result, bool forecRefreshNext) CombineValueModMaxAndOrder(Dictionary<int, (float mod, int order)> mods)
        {
            if (mods == null || mods.Count == 0)
            {
                return (null, false);
            }
            float result = float.MinValue;
            int maxOrder = int.MinValue;
            foreach (var m in mods.Values)
            {
                if (m.order > maxOrder)
                {
                    maxOrder = m.order;
                    result = m.mod;
                }
                else if (m.order == maxOrder)
                {
                    result = Math.Max(result, m.mod);
                }
            }
            return (result, false);
        }

        public FloatMod(ValueModBuildParam<float, float> param) : base(param)
        {

        }

        /// <summary>
        /// <para>使用默认的FloatMod构建参数</para>
        /// <para>提供Value = MaxOrOrder(Constant) || ∏(Multiply) * ( base + ∑(Flat) + base * ∑(Percent) )的计算方式</para>
        /// </summary>
        public FloatMod() : base(DefaultFloatModBuildParam)
        {

        }

    }
}