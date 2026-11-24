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
    /// ValueMod构建参数，包含各部分的组合函数和计算最终值的函数
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TMod"></typeparam>
    public readonly struct ValueModBuildParam<TValue, TMod> where TValue : struct, IEquatable<TValue> where TMod : struct, IEquatable<TMod>
    {
        public readonly (string nameTag, ValueModPartCombineFunc<TMod> combineModFunc)[] parts;
        public readonly ValueModComputeFunc<TValue, TMod> computeValueFunc;
        public readonly Func<string, int> getPartIndexByName;
        public readonly bool cacheComputedValue;

        /// <param name="parts">ValueMod的各部分信息，包括名称标签和组合函数</param>
        /// <param name="computeValue">计算最终值的函数</param>
        /// <param name="getPartIndexByName">获取指定名称的mod索引的函数，输入mod名称，返回对应索引，若不存在返回-1，不设置时使用遍历查找</param>
        /// <param name="cacheComputedValue">是否缓存最终计算结果以提高重复计算的性能</param>
        public ValueModBuildParam((string nameTag, ValueModPartCombineFunc<TMod> combineModFunc)[] parts, ValueModComputeFunc<TValue, TMod> computeValue, Func<string, int> getPartIndexByName = null, bool cacheComputedValue = true)
        {
            this.parts = parts;
            this.computeValueFunc = computeValue;
            this.getPartIndexByName = getPartIndexByName;
            this.cacheComputedValue = cacheComputedValue;
        }
    }
    
    public static class ValueMod
    {
        public const string ModConstant = "Constant";
        public const string ModMultiply = "Multiply";
        public const string ModFlat = "Flat";
        public const string ModPercent = "Percent";

        public static int GetPartIndexDefault(string partName)
        {
            return partName switch
            {
                ModMultiply => 0,
                ModFlat => 1,
                ModPercent => 2,
                ModConstant => 3,
                _ => -1
            };
        }

        /// <summary>
        /// 默认的FloatMod定义，
        /// 提供Value = MaxOrOrder(Constant) || ∏(Multiply) * ( base + ∑(Flat) + base * ∑(Percent) )的计算方式
        /// </summary>
        public static ValueModBuildParam<float, float> DefaultFloatModBuildParam = new ValueModBuildParam<float, float>(
            new (string nameTag, ValueModPartCombineFunc<float> combineModFunc)[]
            {
                (ModMultiply, CombineModPartMultiply),
                (ModFlat, CombineModPartAdd),
                (ModPercent, CombineModPartAdd),
                (ModConstant, CombineModPartMaxAndOrder),
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
        /// 默认的双精度浮点数类型的ValueMod构建参数
        /// </summary>
        public static ValueModBuildParam<double, double> DefaultDoubleModBuildParam = new ValueModBuildParam<double, double>(
            new (string nameTag, ValueModPartCombineFunc<double> combineModFunc)[]
            {
                (ModMultiply, CombineModPartMultiply),
                (ModFlat, CombineModPartAdd),
                (ModPercent, CombineModPartAdd),
                (ModConstant, CombineModPartMaxAndOrder),
            },
            computeValue: ComputeValueDefault
        );

        /// <summary>
        /// 计算最终的浮点值，按照 Constant || Multiply * ( base + Flat + base * Percent ) 的顺序进行计算
        /// </summary>
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
        /// 计算最终的双精度浮点值，按照 Constant || Multiply * ( base + Flat + base * Percent ) 的顺序进行计算
        /// </summary>
        public static double ComputeValueDefault(double?[] mods, double baseValue)
        {
            double? mult = mods[0];
            double? flat = mods[1];
            double? percent = mods[2];
            double? constant = mods[3];
            if (constant.HasValue)
            {
                return constant.Value;
            }
            double result = baseValue;
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
        public static (float? result, bool forceRefreshNext) CombineModPartAdd(Dictionary<int, (float, int)> mods)
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
        /// 叠加所有的mod值
        /// </summary>
        public static (double? result, bool forceRefreshNext) CombineModPartAdd(Dictionary<int, (double, int)> mods)
        {
            if (mods == null || mods.Count == 0)
            {
                return (null, false);
            }
            double result = 0;
            foreach (var mod in mods.Values)
            {
                result += mod.Item1;
            }
            return (result, false);
        }

        /// <summary>
        /// 计算所有mod的乘积
        /// </summary>
        public static (float? result, bool forceRefreshNext) CombineModPartMultiply(Dictionary<int, (float, int)> mods)
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
        /// 计算所有mod的乘积
        /// </summary>
        public static (double? result, bool forceRefreshNext) CombineModPartMultiply(Dictionary<int, (double, int)> mods)
        {
            if (mods == null || mods.Count == 0)
            {
                return (null, false);
            }
            double result = 1;
            foreach (var mod in mods.Values)
            {
                result *= mod.Item1;
            }
            return (result, false);
        }

        /// <summary>
        /// 返回最大Order或者同Order中的最大值
        /// </summary>
        public static (float? result, bool forecRefreshNext) CombineModPartMaxAndOrder(Dictionary<int, (float mod, int order)> mods)
        {
            if (mods == null || mods.Count == 0)
            {
                return (null, false);
            }
            float result = default;
            int maxOrder = default;
            bool hasResult = false;
            foreach (var m in mods.Values)
            {
                if (!hasResult || m.order > maxOrder)
                {
                    hasResult = true;
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
        
        /// <summary>
        /// 返回最大Order或者同Order中的最大值
        /// </summary>
        public static (double? result, bool forceRefreshNext) CombineModPartMaxAndOrder(Dictionary<int, (double mod, int order)> mods)
        {
            if (mods == null || mods.Count == 0)
            {
                return (null, false);
            }
            double result = default;
            int maxOrder = default;
            bool hasResult = false;
            foreach (var m in mods.Values)
            {
                if (!hasResult || m.order > maxOrder)
                {
                    hasResult = true;
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

        /// <summary>
        /// 使用默认配置创建Float类型的ValueMod实例
        /// </summary>
        /// <returns></returns>
        public static ValueMod<float> CreateFloatDefault()
        {
            return new ValueMod<float>(DefaultFloatModBuildParam);
        }

        /// <summary>
        /// 使用默认配置创建Double类型的ValueMod实例
        /// </summary>
        /// <returns></returns>
        public static ValueMod<double> CreateDoubleDefault()
        {
            return new ValueMod<double>(DefaultDoubleModBuildParam);
        }

    }
}