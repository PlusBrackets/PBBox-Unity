/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.03.09
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;
using System;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace PBBox.Variables
{

    /// <summary>
    /// 可变值类, Value = Constant || ∏(Multiply) * ( base + ∑(Flat) + base * ∑(Percent) )
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public abstract partial class MutableValue<T> : IReferencePoolItem where T : struct, IEquatable<T>
    {
        public enum ModType
        {
            Flat = 0,
            Percent = 1,
            Multiply = 2,
            Constant = 3
        }

        [System.Serializable]
        protected struct ConstantMod
        {
            public int priority;
            public T mod;

            public ConstantMod(T mod, int priority)
            {
                this.mod = mod;
                this.priority = priority;
            }
        }

#if UNITY_5_3_OR_NEWER
        [SerializeField]
#endif
        protected T m_BaseValue = default;
        
        protected T m_ModdedValue = default;
        protected T? m_FlatMod = default;
        protected T? m_PrecentMod = default;
        protected T? m_MultMod = default;
        protected T? m_ConstMod = default;

        private Dictionary<int, T> m_FlatMods;
        private Dictionary<int, T> m_PercentMods;
        private Dictionary<int, T> m_MultMods;
        private Dictionary<int, ConstantMod> m_ConstMods;
        /// <summary>
        /// 是否需要计算
        /// </summary>
        protected bool m_NeedRecompute = true;

        /// <summary>
        /// 计算后的值，value = Constant || ∏(Multiply) * ( base + ∑(Additons) + base * ∑(Percents) )
        /// </summary>
        /// <value></value>
        public T Value
        {
            get
            {
                TryRecomputeValue();
                return m_ModdedValue;
            }
        }

        /// <summary>
        /// 基础属性
        /// </summary>
        /// <value></value>
        public T BaseValue
        {
            get
            {
                return m_BaseValue;
            }
            set
            {
                if (!m_BaseValue.Equals(value))
                {
                    m_BaseValue = value;
                }
            }
        }

        public T? FlatMod
        {
            get
            {
                TryRecomputeValue();
                return m_FlatMod;
            }
        }

        public T? PrecentMod
        {
            get
            {
                TryRecomputeValue();
                return m_PrecentMod;
            }
        }

        public T? MultMod
        {
            get
            {
                TryRecomputeValue();
                return m_MultMod;
            }
        }

        public T? ConstMod
        {
            get
            {
                TryRecomputeValue();
                return m_ConstMod;
            }
        }

        bool IReferencePoolItem.IsUsing { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MutableValue(T baseValue = default(T))
        {
            m_BaseValue = baseValue;
        }

        private void AddToModDict(int key, T value, ref Dictionary<int, T> dict)
        {
            if (dict == null)
            {
                dict = new Dictionary<int, T>();
            }
            if (dict.TryGetValue(key, out T _value))
            {
                if (_value.Equals(value))
                {
                    return;
                }
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
            m_NeedRecompute = true;
        }

        private void RemoveFromModDict(int key, Dictionary<int, T> dict)
        {
            if (dict == null)
            {
                return;
            }
            if (dict.Remove(key))
            {
                m_NeedRecompute = true;
            }
        }

        private bool ContainsInModDict(int key, Dictionary<int, T> dict)
        {
            if (dict == null)
            {
                return false;
            }
            return dict.ContainsKey(key);
        }

        private int CountOfModDict(Dictionary<int, T> dict)
        {
            if (dict == null)
            {
                return 0;
            }
            return dict.Count;
        }

        public void ModifyFlat(int key, T flat) => AddToModDict(key, flat, ref m_FlatMods);
        public void ModifyPercent(int key, T percent) => AddToModDict(key, percent, ref m_PercentMods);
        public void ModifyMult(int key, T mult) => AddToModDict(key, mult, ref m_MultMods);
        public void ModifyConst(int key, T constant, int priority)
        {
            if (m_ConstMods == null)
            {
                m_ConstMods = new Dictionary<int, ConstantMod>();
            }
            if (m_ConstMods.TryGetValue(key, out ConstantMod _value))
            {
                if (_value.priority == priority && _value.mod.Equals(constant))
                {
                    return;
                }
                _value.mod = constant;
                _value.priority = priority;
                m_ConstMods[key] = _value;
            }
            else
            {
                m_ConstMods.Add(key, new ConstantMod(constant, priority));
            }
            m_NeedRecompute = true;
        }

        public void Modify(ModType modType, int key, T modValue, int priority = 0)
        {
            switch (modType)
            {
                case ModType.Flat:
                    ModifyFlat(key, modValue);
                    return;
                case ModType.Percent:
                    ModifyPercent(key, modValue);
                    return;
                case ModType.Multiply:
                    ModifyMult(key, modValue);
                    return;
                case ModType.Constant:
                    ModifyConst(key, modValue, priority);
                    return;
            }
        }

        public void UnmodifyFlat(int key) => RemoveFromModDict(key, m_FlatMods);
        public void UnmodifyPercent(int key) => RemoveFromModDict(key, m_PercentMods);
        public void UnmodifyMult(int key) => RemoveFromModDict(key, m_MultMods);
        public void UnmodifyConst(int key)
        {
            if (m_ConstMods == null)
            {
                return;
            }
            if (m_ConstMods.Remove(key))
            {
                m_NeedRecompute = true;
            }
        }

        public void Unmodify(ModType modType, int key)
        {
            switch (modType)
            {
                case ModType.Flat:
                    UnmodifyFlat(key);
                    return;
                case ModType.Percent:
                    UnmodifyPercent(key);
                    return;
                case ModType.Multiply:
                    UnmodifyMult(key);
                    return;
                case ModType.Constant:
                    UnmodifyConst(key);
                    return;
            }
        }

        /// <summary>
        /// 移除所有mod
        /// </summary>
        public void UnmodifyAll()
        {
            m_FlatMods?.Clear();
            m_PercentMods?.Clear();
            m_MultMods?.Clear();
            m_ConstMods?.Clear();
            m_NeedRecompute = true;
        }

        public bool ContainsMod(ModType modType, int key)
        {
            switch (modType)
            {
                case ModType.Flat:
                    return ContainsInModDict(key, m_FlatMods);
                case ModType.Percent:
                    return ContainsInModDict(key, m_PercentMods);
                case ModType.Multiply:
                    return ContainsInModDict(key, m_MultMods);
                case ModType.Constant:
                    if (m_ConstMods == null)
                    {
                        return false;
                    }
                    return m_ConstMods.ContainsKey(key);
            }
            return false;
        }

        public bool ContainsAnyMod(int key)
        {
            return ContainsMod(ModType.Flat, key) || ContainsMod(ModType.Percent, key) || ContainsMod(ModType.Multiply, key) || ContainsMod(ModType.Constant, key);
        }

        public int GetModCount(ModType modType)
        {
            switch (modType)
            {
                case ModType.Flat:
                    return CountOfModDict(m_FlatMods);
                case ModType.Percent:
                    return CountOfModDict(m_PercentMods);
                case ModType.Multiply:
                    return CountOfModDict(m_MultMods);
                case ModType.Constant:
                    if (m_ConstMods == null)
                    {
                        return 0;
                    }
                    return m_ConstMods.Count;
            }
            return 0;
        }

        /// <summary>
        /// 更新buff数值
        /// </summary>
        private void TryRecomputeValue()
        {
            if (!m_NeedRecompute)
            {
                return;
            }

            m_FlatMod = ComputeFlatMod();
            m_PrecentMod = ComputePrecentMod();
            m_MultMod = ComputeMultMod();
            m_ConstMod = ComputeConstMod();
            m_ModdedValue = ComputeValue(m_BaseValue);

            m_NeedRecompute = false;
        }

        /// <summary>
        /// 使用其他的基础值计算效果
        /// </summary>
        /// <param name="otherBaseValue"></param>
        /// <returns></returns>
        public T ComputeEffective(T otherBaseValue)
        {
            if (otherBaseValue.Equals(m_BaseValue))
            {
                return Value;
            }
            else
            {
                TryRecomputeValue();
                return ComputeValue(otherBaseValue);
            }
        }

        protected bool SumAllValue(Dictionary<int, T> dict, out T result)
        {
            result = default(T);
            if (dict == null || dict.Count == 0)
            {
                return false;
            }
            foreach (var kvp in dict)
            {
                result = AddValue(result, kvp.Value);
            }
            return true;
        }

        protected bool ProdAllValue(Dictionary<int, T> dict, out T result)
        {
            result = default(T);
            if (dict == null || dict.Count == 0)
            {
                return false;
            }
            var _enumerator = dict.GetEnumerator();
            if (_enumerator.MoveNext())
            {
                result = _enumerator.Current.Value;
            }
            while (_enumerator.MoveNext())
            {
                result = MulValue(result, _enumerator.Current.Value);
            }
            return true;
        }

        protected virtual T? ComputeFlatMod()
        {
            if (SumAllValue(m_FlatMods, out var _result))
            {
                return _result;
            }
            return null;
        }

        protected virtual T? ComputePrecentMod()
        {
            if (SumAllValue(m_PercentMods, out var _result))
            {
                return _result;
            }
            return null;
        }

        protected virtual T? ComputeMultMod()
        {
            if (ProdAllValue(m_MultMods, out var _result))
            {
                return _result;
            }
            return null;
        }

        protected virtual T? ComputeConstMod()
        {
            if (m_ConstMods == null || m_ConstMods.Count == 0)
            {
                return null;
            }
            int p = int.MinValue;
            T _result = default(T);
            foreach (var kvp in m_ConstMods)
            {
                var _v = kvp.Value;
                if (_v.priority >= p)
                {
                    p = _v.priority;
                    _result = _v.mod;
                }
            }
            return _result;
        }

        protected virtual T ComputeValue(T baseValue)
        {
            if (m_ConstMod.HasValue)
            {
                return m_ConstMod.Value;
            }
            T _result = baseValue;
            if (m_FlatMod.HasValue)
            {
                _result = AddValue(baseValue, m_FlatMod.Value);
            }
            if (m_PrecentMod.HasValue)
            {
                _result = AddValue(_result, MulValue(baseValue, m_PrecentMod.Value));
            }
            if (m_MultMod.HasValue)
            {
                _result = MulValue(_result, m_MultMod.Value);
            }
            return _result;
        }

        protected abstract T MulValue(T a, T b);
        protected abstract T AddValue(T a, T b);

        public override string ToString()
        {
            return Value.ToString();
        }

        void IReferencePoolItem.OnReferenceAcquire()
        {
            
        }

        void IReferencePoolItem.OnReferenceRelease()
        {
            m_NeedRecompute = true;
            m_BaseValue = default(T);
            m_ModdedValue = default(T);
            UnmodifyAll();
        }
    }
}