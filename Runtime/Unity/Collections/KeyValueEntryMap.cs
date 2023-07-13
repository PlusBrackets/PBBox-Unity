/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.04.12
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Collections
{
    /// <summary>
    /// 可序列化的字典结构, 性能略逊Dictionary（相当于封装Dictionary）但相差不大
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public class KeyValueEntryMap<T, TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver where T : IKeyValueEntry<TKey, TValue>, new()
    {
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("maps")]
        protected List<T> m_Maps = new List<T>();
        protected Lazy<Dictionary<TKey, TValue>> m_Dict;
        public Dictionary<TKey, TValue> Dictionary => m_Dict.Value;

        public TValue this[TKey key] { get => Dictionary[key]; set => Dictionary[key] = value; }

        public Dictionary<TKey, TValue>.KeyCollection Keys => Dictionary.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => Dictionary.Values;
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Dictionary.Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Dictionary.Values;
        public int Count => Dictionary.Count;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).IsReadOnly;

        public KeyValueEntryMap()
        {
            m_Dict = new Lazy<Dictionary<TKey, TValue>>(CreateDict);
        }

        public KeyValueEntryMap(IDictionary<TKey, TValue> dict)
        {
            m_Dict = new Lazy<Dictionary<TKey, TValue>>(CreateDict);
            var _dict = m_Dict.Value;
            foreach (var _kvp in dict)
            {
                _dict.Add(_kvp.Key, _kvp.Value);
            }
        }

        private Dictionary<TKey, TValue> CreateDict()
        {
            var _dict = new Dictionary<TKey, TValue>();
            for (int i = 0; i < m_Maps.Count; i++)
            {
                var _entry = m_Maps[i];
                if (!_dict.TryAdd(_entry.Key, _entry.Value))
                {
                    Log.Error($"There has same key [{_entry.Key}] in {GetType()}, will pass this entry.", "KeyValueEntryMap");
                }
            }
#if !UNITY_EDITOR
            //非编辑器模式下会把数据清除掉，节省空间
            m_Maps.Clear();
#endif
            return _dict;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_Dict == null)
            {
                m_Dict = new Lazy<Dictionary<TKey, TValue>>(CreateDict);
            }
            else if (m_Dict.IsValueCreated)
            {
                m_Dict.Value.Clear();
                foreach (var _kvp in m_Maps)
                {
                    m_Dict.Value.Add(_kvp.Key, _kvp.Value);
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (m_Dict == null || !m_Dict.IsValueCreated)
            {
                //未被使用过，不做操作
                return;
            }
            if (Dictionary.Count != m_Maps.Count)
            {
                m_Maps.Clear();
            }
            int _index = 0;
            foreach (var _kvp in Dictionary)
            {
                var _entry = new T();
                _entry.Set(_kvp.Key, _kvp.Value);
                if (_index >= m_Maps.Count)
                {
                    m_Maps.Add(_entry);
                }
                else
                {
                    m_Maps[_index] = _entry;
                }
                _index++;
            }
        }

        public void Add(TKey key, TValue value) => Dictionary.Add(key, value);

        public void Clear() => Dictionary.Clear();

        public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);

        public bool ContainsValue(TValue value) => Dictionary.ContainsValue(value);

        public int EnsureCapacity(int capacity) => Dictionary.EnsureCapacity(capacity);

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator() => Dictionary.GetEnumerator();

        public bool Remove(TKey key) => Dictionary.Remove(key);

        public bool Remove(TKey key, out TValue value) => Dictionary.Remove(key, out value);

        public void TrimExcess() => Dictionary.TrimExcess();

        public void TrimExcess(int capacity) => Dictionary.TrimExcess(capacity);

        public bool TryAdd(TKey key, TValue value) => Dictionary.TryAdd(key, value);

        public bool TryGetValue(TKey key, out TValue value) => Dictionary.TryGetValue(key, out value);

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Add(item);
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Contains(item);
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).CopyTo(array, arrayIndex);
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Remove(item);

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => Dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Dictionary.GetEnumerator();
    }
}