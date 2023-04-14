/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.18
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;

namespace PBBox.Collections
{
    /// <summary>
    /// 类似于KeyValuePair，但继承了IEquatable和IComparable，避免在自身比较时产生gc
    /// </summary>
    [Serializable]
    public readonly struct KeyValueEntry<TKey, TValue> : IKeyValueEntry<TKey, TValue>, IEquatable<KeyValueEntry<TKey, TValue>>, IComparable<KeyValueEntry<TKey, TValue>>
    {
        public TKey Key { get; }
        public TValue Value { get; }

        public KeyValueEntry(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        public KeyValuePair<TKey, TValue> ToPair()
        {
            return new KeyValuePair<TKey, TValue>(Key, Value);
        }

        public void Set(TKey key, TValue value)
        {
            
        }

        public bool Equals(KeyValueEntry<TKey, TValue> other)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key)
            && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Value);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public int CompareTo(KeyValueEntry<TKey, TValue> other)
        {
            return Comparer<TKey>.Default.Compare(Key, other.Key);
        }

        public static bool operator ==(KeyValueEntry<TKey, TValue> a, KeyValueEntry<TKey, TValue> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(KeyValueEntry<TKey, TValue> a, KeyValueEntry<TKey, TValue> b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return "<" + Key + ": " + Value + ">";
        }
    }

}