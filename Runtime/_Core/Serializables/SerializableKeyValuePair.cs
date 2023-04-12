/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.11.18
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;

namespace PBBox
{
    /// <summary>
    /// 可序列化的KeyValuePair
    /// </summary>
    [System.Serializable, System.Obsolete]
    public struct SKeyValuePair<TKey, TValue> : IEquatable<SKeyValuePair<TKey, TValue>>
    {
        public TKey key;
        public TValue value;

        public SKeyValuePair(TKey key, TValue value){
            this.key = key;
            this.value = value;
        }

        public KeyValuePair<TKey, TValue> ToKeyValuePair()
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }

        public bool Equals(SKeyValuePair<TKey, TValue> other)
        {
            return this.key.Equals(other.key) && this.value.Equals(other.value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(key, value);
        }

        public override string ToString()
        {
            return $"<{key.ToString()}, {value.ToString()}>";
        }
    }
}