/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.11.18
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
#if USE_ODIN
using Sirenix.OdinInspector;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PBBox
{
    // public interface ISDictionary {}

    // Dictionary<TKey, TValue>
    /// <summary>
    /// 可序列化字典,支持unityInspector以及Unity的JsonUtility
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public class SDictionary<TKey, TValue> : ISerializationCallbackReceiver, IDictionary<TKey, TValue>//,ISDictionary
    {
        [SerializeField]
        List<SKeyValuePair<TKey, TValue>> maps = new List<SKeyValuePair<TKey, TValue>>();

        Dictionary<TKey, int> keyIndexs => _keyIndexs.Value;
        
        [NonSerialized]
        Lazy<Dictionary<TKey, int>> _keyIndexs;

        public SDictionary()
        {
            _keyIndexs = new Lazy<Dictionary<TKey, int>>(LazyDictionaryIniter);
        }

        public SDictionary(Dictionary<TKey, TValue> target) : this()
        {
            foreach (var key in target.Keys)
            {
                maps.Add(new SKeyValuePair<TKey, TValue>(key, target[key]));
            }
        }

        public SDictionary(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }

        Dictionary<TKey, int> LazyDictionaryIniter()
        {
            var d = new Dictionary<TKey, int>();
            for (int i = 0; i < maps.Count; i++)
            {
                if (!d.TryAdd(maps[i].key, i))
                {
                    DebugUtils.Internal.LogError($"[{GetType().Name}]有重复的key值:{maps[i].key},index:{i}");
                }
            }
            return d;
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            _keyIndexs = new Lazy<Dictionary<TKey, int>>(LazyDictionaryIniter);
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            return ToDictionary(new Dictionary<TKey, TValue>());
        }

        public Dictionary<TKey, TValue> ToDictionary(Dictionary<TKey, TValue> dict)
        {
            dict.Clear();
            foreach (var kvp in maps)
            {
                dict.TryAdd(kvp.key, kvp.value);
            }
            return dict;
        }

        #region Dictionary Func    

        public TValue this[TKey key]
        {
            get => maps[keyIndexs[key]].value;
            set
            {
                SKeyValuePair<TKey, TValue> keyValue = new SKeyValuePair<TKey, TValue>(key, value);
                if (keyIndexs.ContainsKey(key))
                {
                    maps[keyIndexs[key]] = keyValue;
                }
                else
                {
                    keyIndexs.Add(key, maps.Count);
                    maps.Add(keyValue);
                }
            }
        }

        public ICollection<TKey> Keys => maps.Select(t => t.key).ToArray();
        public ICollection<TValue> Values => maps.Select(t => t.value).ToArray();
        public int Count => maps.Count;
        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
                throw new ArgumentException("An element with the same key already exists in the dictionary.");
        }

        public void Add(KeyValuePair<TKey, TValue> kvp)
        {
            Add(kvp.Key, kvp.Value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            if (keyIndexs.ContainsKey(key))
            {
                return false;
            }
            else
            {
                keyIndexs[key] = maps.Count;
                maps.Add(new SKeyValuePair<TKey, TValue>(key, value));
                return true;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (keyIndexs.TryGetValue(key, out int index))
            {
                value = maps[index].value;
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return keyIndexs.ContainsKey(key);
        }

        public bool Contains(KeyValuePair<TKey, TValue> kvp)
        {
            return keyIndexs.ContainsKey(kvp.Key);
        }


        public bool Remove(TKey key)
        {
            if (keyIndexs.TryGetValue(key, out int index))
            {
                keyIndexs.Remove(key);
                maps.RemoveAt(index);
                for (var i = index; i < maps.Count; i++)
                    keyIndexs[maps[i].key] = i;

                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> kvp)
        {
            return Remove(kvp.Key);
        }

        public void Clear()
        {
            maps.Clear();
            if (_keyIndexs.IsValueCreated)
            {
                keyIndexs.Clear();
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int keyCount = maps.Count;
            if (array.Length - arrayIndex < keyCount)
                throw new ArgumentException("arrayIndex");
            for (var i = 0; i < keyCount; i++, arrayIndex++)
            {
                var item = maps[i];
                array[arrayIndex] = new KeyValuePair<TKey, TValue>(item.key, item.value);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return maps.Select(ToKeyValuePair).GetEnumerator();

            static KeyValuePair<TKey, TValue> ToKeyValuePair(SKeyValuePair<TKey, TValue> kvp)
            {
                return new KeyValuePair<TKey, TValue>(kvp.key, kvp.value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}