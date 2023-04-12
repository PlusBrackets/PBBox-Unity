using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Collections
{
    public interface IKeyValueEntry<TKey, TValue>
    {
        TKey Key { get; }
        TValue Value { get; }
        KeyValuePair<TKey, TValue> ToPair();
        void Set(TKey key, TValue value);
    }
}