/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.04.12
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Collections
{
    /// <summary>
    /// 可序列化的KeyValuePair
    /// </summary>
    [System.Serializable]
    public struct SKeyValueEntry<TKey, TValue> : IKeyValueEntry<TKey, TValue>, IEquatable<SKeyValueEntry<TKey, TValue>>
    {
        [UnityEngine.Serialization.FormerlySerializedAs("key")]
        [SerializeField]
        private TKey m_Key;
        [UnityEngine.Serialization.FormerlySerializedAs("value")]
        [SerializeField]
        private TValue m_Value;
        public TKey Key => m_Key;
        public TValue Value => m_Value;

        public SKeyValueEntry(TKey key, TValue value)
        {
            m_Key = key;
            m_Value = value;
        }

        public KeyValuePair<TKey, TValue> ToPair()
        {
            return new KeyValuePair<TKey, TValue>(m_Key, m_Value);
        }

        public void Set(TKey key, TValue value){
            m_Key = key;
            m_Value = value;
        }

        public bool Equals(SKeyValueEntry<TKey, TValue> other)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key)
            && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Value);
        }

        public override string ToString()
        {
            return "<" + Key + ": " + Value + ">";
        }
    }
}