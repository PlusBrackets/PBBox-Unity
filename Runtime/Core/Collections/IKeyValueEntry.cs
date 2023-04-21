/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.04.21
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;

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