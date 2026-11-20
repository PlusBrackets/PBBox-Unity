/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.20
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PBBox.Properties
{
    public interface IValueMod<T>
    {
        T Compute(T baseValue);
    }

    /// <summary>
    /// 计算valueMod中各部分组合后的值，返回结果和是否需要强制刷新下一次计算
    /// </summary>
    /// <typeparam name="TMod"></typeparam>
    /// <param name="mods"></param>
    /// <returns></returns>
    public delegate (TMod? result, bool forceRefreshNext) ValueModPartCombineFunc<TMod>(Dictionary<int, (TMod mod, int order)> mods) where TMod : struct, IEquatable<TMod>;
    /// <summary>
    /// 将ValueMod中各部分的组合值应用到基础值上，计算最终结果
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TMod"></typeparam>
    /// <param name="mods"></param>
    /// <param name="baseValue"></param>
    /// <returns></returns>
    public delegate TValue ValueModComputeFunc<TValue, TMod>(TMod?[] mods, TValue baseValue) where TValue : struct, IEquatable<TValue> where TMod : struct, IEquatable<TMod>;


    /// <summary>
    /// ValueMod构建参数，包含各部分的组合函数和计算最终值的函数
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TMod"></typeparam>
    public readonly struct ValueModBuildParam<TValue, TMod> where TValue : struct, IEquatable<TValue> where TMod : struct, IEquatable<TMod>
    {
        /// <summary>
        /// ValueMod的各部分信息，包括名称标签和组合函数
        /// </summary>
        public readonly (string nameTag, ValueModPartCombineFunc<TMod> combineModFunc)[] parts;
        /// <summary>
        /// 计算最终值的函数
        /// </summary>
        public readonly ValueModComputeFunc<TValue, TMod> computeValue;
        /// <summary>
        /// 获取指定名称的mod索引的函数，输入mod名称，返回对应索引，若不存在返回-1，不设置时使用遍历查找
        /// </summary>
        public readonly Func<string, int> getPartIndexByName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parts">ValueMod的各部分信息，包括名称标签和组合函数</param>
        /// <param name="computeValue">计算最终值的函数</param>
        /// <param name="getPartIndexByName">获取指定名称的mod索引的函数，输入mod名称，返回对应索引，若不存在返回-1，不设置时使用遍历查找</param>
        public ValueModBuildParam((string nameTag, ValueModPartCombineFunc<TMod> combineModFunc)[] parts, ValueModComputeFunc<TValue, TMod> computeValue, Func<string, int> getPartIndexByName = null)
        {
            this.parts = parts;
            this.computeValue = computeValue;
            this.getPartIndexByName = getPartIndexByName;
        }
    }

    /// <summary>
    /// <para>ValueMod的单个部分，包含名称标签、修饰符集合以及组合函数</para>
    /// <para>可用于增益的单独乘区等</para>
    /// </summary>
    /// <typeparam name="TMod"></typeparam>
    public class ValueModPart<TMod> where TMod : struct, IEquatable<TMod>
    {
        public string NameTag { get; private set; }
        public bool IsDirty { get; private set; }
        /// <summary>
        /// 是否强制刷新，即使值未改变也会重新计算
        /// </summary>
        public bool IsForceRefresh { get; private set; }
        public bool IsNeedRefresh => IsDirty || IsForceRefresh;

        private Dictionary<int, (TMod, int)> _mods;
        private TMod? _cachedModCombined;
        private ValueModPartCombineFunc<TMod> _CombineMod;

        public ValueModPart(string nameTag, ValueModPartCombineFunc<TMod> combineMod)
        {
            NameTag = nameTag;
            _CombineMod = combineMod;
            IsDirty = true;
        }

        public bool AddMod(int key, TMod mod, int order)
        {
            if (_mods == null)
            {
                _mods = new Dictionary<int, (TMod, int)>();
            }

            if (_mods.TryGetValue(key, out var existingValue))
            {
                if (EqualityComparer<TMod>.Default.Equals(existingValue.Item1, mod) && existingValue.Item2 == order)
                {
                    return false;
                }
            }
            _mods[key] = (mod, order);
            IsDirty = true;
            return true;
        }

        public bool RemoveMod(int key)
        {
            if (_mods != null && _mods.Remove(key))
            {
                IsDirty = true;
            }
            return IsDirty;
        }

        public bool ClearMods()
        {
            if (_mods != null && _mods.Count > 0)
            {
                _mods.Clear();
                IsDirty = true;
            }
            return IsDirty;
        }

        public TMod? GetCombinedValue()
        {
            if (!IsNeedRefresh)
            {
                return _cachedModCombined;
            }

            var (result, forceRefresh) = _CombineMod(_mods);
            _cachedModCombined = result;
            IsForceRefresh = forceRefresh;
            IsDirty = false;
            return result;
        }
    }

    /// <summary>
    /// <para>ValueMod类，包含多个ValueModPart，通过组合函数计算最终值</para>
    /// <para>通常用于增益计算</para>
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TMod"></typeparam>
    public class ValueMod<TValue, TMod> : IValueMod<TValue> where TValue : struct, IEquatable<TValue> where TMod : struct, IEquatable<TMod>
    {
        /// <summary>
        /// ValueMod的各部分组合后的结果缓存，用于计算最终值
        /// </summary>
        protected readonly ValueModPart<TMod>[] _parts;
        /// <summary>
        /// ValueMod各部分的组合结果缓存，用于计算最终值
        /// </summary>
        protected readonly TMod?[] _modPartResults;
        /// <summary>
        /// 计算最终值的函数
        /// </summary>
        protected readonly ValueModComputeFunc<TValue, TMod> _computeValueFunc;
        /// <summary>
        /// 获取指定名称的mod索引的函数
        /// </summary>
        protected readonly Func<string, int> _getModifierIndexByName;
        /// <summary>
        /// 是否需要重新计算最终值的标志
        /// </summary>
        public bool IsDirty { get; protected set; } = false;
        /// <summary>
        /// 获取各部分组合结果的缓存数组
        /// </summary>
        public TMod?[] CachedModPartResults => _modPartResults;

        public ValueMod(ValueModBuildParam<TValue, TMod> param)
        {
            var modifierBuilds = param.parts;
            var computeValue = param.computeValue;
            _parts = new ValueModPart<TMod>[modifierBuilds.Length];
            _modPartResults = new TMod?[modifierBuilds.Length];
            for (int i = 0; i < modifierBuilds.Length; i++)
            {
                var modBuild = modifierBuilds[i];
                _parts[i] = new ValueModPart<TMod>(modBuild.nameTag, modBuild.combineModFunc);
                _modPartResults[i] = null;
            }
            _computeValueFunc = new ValueModComputeFunc<TValue, TMod>(computeValue);
            _getModifierIndexByName = param.getPartIndexByName;
        }

        private ValueModPart<TMod> GetModifierByName(string modifierName)
        {
            if (_getModifierIndexByName != null)
            {
                int i = _getModifierIndexByName(modifierName);
                return i >= 0 ? _parts[i] : null;
            }
            else
            {
                for (int i = 0; i < _parts.Length; i++)
                {
                    if (_parts[i].NameTag == modifierName)
                    {
                        return _parts[i];
                    }
                }
            }
            Log.Error($"Modifier not found: {modifierName}", "ValueMod", Log.PBBoxLoggerName);
            return null;
        }

        public virtual TValue Compute(TValue baseValue)
        {
            if (IsDirty)
            {
                RefreshPartResults();
            }
            return _computeValueFunc(_modPartResults, baseValue);
        }
        
        /// <summary>
        /// 刷新所有部分的计算结果
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void RefreshPartResults()
        {
            IsDirty = false;
            for (int i = 0; i < _parts.Length; i++)
            {
                _modPartResults[i] = _parts[i].GetCombinedValue();
                IsDirty |= _parts[i].IsNeedRefresh;
            }
        }

        public void AddMod(int partIndex, int key, TMod mod, int order = 0)
        {
            if (partIndex < 0 || partIndex >= _parts.Length)
            {
                Log.Error($"Invalid partIndex: {partIndex}", "ValueMod", Log.PBBoxLoggerName);
                return;
            }
            _parts[partIndex].AddMod(key, mod, order);
            IsDirty |= _parts[partIndex].IsNeedRefresh;
        }

        public void RemoveMod(int partIndex, int key)
        {
            if (partIndex < 0 || partIndex >= _parts.Length)
            {
                Log.Error($"Invalid partIndex: {partIndex}", "ValueMod", Log.PBBoxLoggerName);
                return;
            }
            _parts[partIndex].RemoveMod(key);
            IsDirty |= _parts[partIndex].IsNeedRefresh;
        }

        public void ClearMods()
        {
            for (int i = 0; i < _parts.Length; i++)
            {
                _parts[i].ClearMods();
                IsDirty |= _parts[i].IsNeedRefresh;
            }
        }

        public void ClearMod(int partIndex)
        {
            if (partIndex < 0 || partIndex >= _parts.Length)
            {
                Log.Error($"Invalid partIndex: {partIndex}", "ValueMod", Log.PBBoxLoggerName);
                return;
            }
            _parts[partIndex].ClearMods();
            IsDirty |= _parts[partIndex].IsNeedRefresh;
        }

        public void AddMod(string partName, int key, TMod mod, int order = 0)
        {
            var modifier = GetModifierByName(partName);
            if (modifier == null)
            {
                return;
            }
            modifier.AddMod(key, mod, order);
            IsDirty |= modifier.IsNeedRefresh;
        }

        public void RemoveMod(string partName, int key)
        {
            var modifier = GetModifierByName(partName);
            if (modifier == null)
            {
                return;
            }
            modifier.RemoveMod(key);
            IsDirty |= modifier.IsNeedRefresh;
        }

        public void ClearMod(string partName)
        {
            var modifier = GetModifierByName(partName);
            if (modifier == null)
            {
                return;
            }
            modifier.ClearMods();
            IsDirty |= modifier.IsNeedRefresh;
        }
    }

}