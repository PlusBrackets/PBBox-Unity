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
    /// 计算valueMod中各部分组合后的值，返回结果和是否需要强制刷新下一次计算
    /// </summary>
    /// <typeparam name="TMod"></typeparam>
    /// <param name="mods"></param>
    /// <returns></returns>
    public delegate (TMod? result, bool forceRefreshNext) ValueModPartCombineFunc<TMod>(Dictionary<int, (TMod mod, int order)> mods) where TMod : struct, IEquatable<TMod>;
    
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
        public int ModCount => _mods?.Count ?? 0;

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
                return true;
            }
            return false;
        }

        public bool ClearMods()
        {
            if (_mods != null && _mods.Count > 0)
            {
                _mods.Clear();
                IsDirty = true;
                return true;
            }
            return false;
        }

        public bool ContainsMod(int key)
        {
            return _mods != null && _mods.ContainsKey(key);
        }

        public bool TryGetMod(int key, out (TMod, int) mod)
        {
            if (_mods != null && _mods.TryGetValue(key, out var value))
            {
                mod = value;
                return true;
            }
            mod = default;
            return false;
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

}