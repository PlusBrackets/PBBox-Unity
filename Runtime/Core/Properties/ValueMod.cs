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
    /// 将ValueMod中各部分的组合值应用到基础值上，计算最终结果
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TMod"></typeparam>
    /// <param name="mods"></param>
    /// <param name="baseValue"></param>
    /// <returns></returns>
    public delegate TValue ValueModComputeFunc<TValue, TMod>(TMod?[] mods, TValue baseValue) where TValue : struct, IEquatable<TValue> where TMod : struct, IEquatable<TMod>;

    //TODO 待支持.net 7.0时，考虑使用INumber约束T进行泛型计算
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
        public bool IsDirty { get; protected set; }
        private readonly bool _isCacheComputedValue;
        private TValue? _cachedBaseValue = null;
        protected TValue? _cachedComputedValue = null;


        public ValueMod(ValueModBuildParam<TValue, TMod> param)
        {
            var parts = param.parts;
            var computeFunc = param.computeValueFunc;
            _isCacheComputedValue = param.cacheComputedValue;
            _parts = new ValueModPart<TMod>[parts.Length];
            _modPartResults = new TMod?[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                var modBuild = parts[i];
                _parts[i] = new ValueModPart<TMod>(modBuild.nameTag, modBuild.combineModFunc);
                _modPartResults[i] = null;
            }
            _computeValueFunc = new ValueModComputeFunc<TValue, TMod>(computeFunc);
            _getModifierIndexByName = param.getPartIndexByName;
            IsDirty = true;
        }

        private ValueModPart<TMod> GetModifierByName(string modifierName)
        {
            if (_getModifierIndexByName != null)
            {
                int i = _getModifierIndexByName(modifierName);
                return i >= 0 && i < _parts.Length ? _parts[i] : null;
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

        public TValue Compute(TValue baseValue)
        {
            if (IsDirty)
            {
                RefreshPartResults();
            }
            if (_cachedBaseValue.HasValue && _cachedComputedValue.HasValue && EqualityComparer<TValue>.Default.Equals(_cachedBaseValue.Value, baseValue))
            {
                return _cachedComputedValue.Value;
            }
            if (_isCacheComputedValue)
            {
                _cachedBaseValue = baseValue;
                _cachedComputedValue = _computeValueFunc(_modPartResults, baseValue);
                return _cachedComputedValue.Value;
            }
            else
            {
                return _computeValueFunc(_modPartResults, baseValue);
            }
        }

        /// <summary>
        /// 计算值但不把该结果缓存，当值类型包含堆对象时避免该值被重复引用
        /// </summary>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        public TValue ComputeWithoutCache(TValue baseValue)
        {
            if (IsDirty)
            {
                RefreshPartResults();
            }
            return _computeValueFunc(_modPartResults, baseValue);
        }

        public void ClearComputedCache()
        {
            _cachedBaseValue = null;
            _cachedComputedValue = null;
        }

        /// <summary>
        /// 刷新所有部分的计算结果
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RefreshPartResults()
        {
            IsDirty = false;
            _cachedComputedValue = null;
            for (int i = 0; i < _parts.Length; i++)
            {
                _modPartResults[i] = _parts[i].GetCombinedValue();
                IsDirty |= _parts[i].IsNeedRefresh;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckPartIndexInternal(int partIndex)
        {
            if (IsPartIndexValid(partIndex))
            {
                return true;
            }
            Log.Error($"Invalid partIndex: {partIndex}", "ValueMod", Log.PBBoxLoggerName);
            return false;
        }

        public bool IsPartIndexValid(int partIndex)
        {
            return partIndex >= 0 && partIndex < _parts.Length;
        }

        /// <summary>
        /// 获取指定部分的缓存计算结果
        /// </summary>
        /// <param name="partIndex"></param>
        /// <returns></returns>
        public TMod? GetCachedPartResult(int partIndex)
        {
            if (!CheckPartIndexInternal(partIndex))
            {
                return null;
            }
            return _modPartResults[partIndex];
        }

        /// <summary>
        /// 获取指定名称部分的缓存计算结果
        /// </summary>
        /// <param name="partName"></param>
        /// <returns></returns>
        public TMod? GetCachedPartResult(string partName)
        {
            var modifier = GetModifierByName(partName);
            if (modifier == null)
            {
                return null;
            }
            return modifier.GetCombinedValue();
        }

        public void AddMod(int partIndex, int key, TMod mod, int order = 0)
        {
            if (!CheckPartIndexInternal(partIndex))
            {
                return;
            }
            _parts[partIndex].AddMod(key, mod, order);
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

        public void RemoveMod(int partIndex, int key)
        {
            if (!CheckPartIndexInternal(partIndex))
            {
                return;
            }
            _parts[partIndex].RemoveMod(key);
            IsDirty |= _parts[partIndex].IsNeedRefresh;
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

        public bool ContainsMod(int partIndex, int key)
        {
            if (!CheckPartIndexInternal(partIndex))
            {
                return false;
            }
            return _parts[partIndex].ContainsMod(key);
        }

        public bool ContainsMod(string partName, int key)
        {
            var modifier = GetModifierByName(partName);
            if (modifier == null)
            {
                return false;
            }
            return modifier.ContainsMod(key);
        }

        public bool TryGetMod(int partIndex, int key, out (TMod, int) mod)
        {
            mod = default;
            if (!CheckPartIndexInternal(partIndex))
            {
                return false;
            }
            return _parts[partIndex].TryGetMod(key, out mod);
        }

        public bool TryGetMod(string partName, int key, out (TMod, int) mod)
        {
            mod = default;
            var modifier = GetModifierByName(partName);
            if (modifier == null)
            {
                return false;
            }
            return modifier.TryGetMod(key, out mod);
        }

        public int GetModCount(int partIndex)
        {
            if (!CheckPartIndexInternal(partIndex))
            {
                return 0;
            }
            return _parts[partIndex].ModCount;
        }
        
        public int GetModCount()
        {
            int count = 0;
            for (int i = 0; i < _parts.Length; i++)
            {
                count += _parts[i].ModCount;
            }
            return count;
        }

        public int GetModCount(string partName)
        {
            var modifier = GetModifierByName(partName);
            if (modifier == null)
            {
                return 0;
            }
            return modifier.ModCount;
        }

        public void ClearMod(int partIndex)
        {
            if (!CheckPartIndexInternal(partIndex))
            {
                return;
            }
            _parts[partIndex].ClearMods();
            IsDirty |= _parts[partIndex].IsNeedRefresh;
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

        public void ClearMods()
        {
            for (int i = 0; i < _parts.Length; i++)
            {
                _parts[i].ClearMods();
                IsDirty |= _parts[i].IsNeedRefresh;
            }
        }
    }

    /// <summary>
    /// <para>ValueMod类，包含多个ValueModPart，通过组合函数计算最终值</para>
    /// <para>通常用于增益计算</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueMod<T> : ValueMod<T, T> where T : struct, IEquatable<T>
    {
        public ValueMod(ValueModBuildParam<T, T> param) : base(param)
        {
        }
    }
}