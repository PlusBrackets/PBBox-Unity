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
    public readonly struct KeyItemPair<TKey, TItem> : IEquatable<KeyItemPair<TKey, TItem>>, IComparable<KeyItemPair<TKey, TItem>>
    {
        public TKey Key { get; }
        public TItem Item { get; }

        public KeyItemPair(TKey key, TItem item)
        {
            Key = key;
            Item = item;
        }

        public bool Equals(KeyItemPair<TKey, TItem> other)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key)
            && EqualityComparer<TItem>.Default.Equals(Item, other.Item);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Item);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public int CompareTo(KeyItemPair<TKey, TItem> other)
        {
            return Comparer<TKey>.Default.Compare(Key, other.Key);
        }

        public static bool operator ==(KeyItemPair<TKey, TItem> a, KeyItemPair<TKey, TItem> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(KeyItemPair<TKey, TItem> a, KeyItemPair<TKey, TItem> b)
        {
            return !a.Equals(b);
        }
    }

}