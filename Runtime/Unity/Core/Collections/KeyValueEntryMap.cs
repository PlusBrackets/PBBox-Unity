/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.04.12
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Collections
{
    /// <summary>
    /// 可序列化的字典结构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public class KeyValueEntryMap<T, TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver where T : IKeyValueEntry<TKey, TValue>, new()
    {
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("maps")]
        protected List<T> m_Maps = new List<T>();
        protected Dictionary<TKey, TValue> m_Dict;

        public TValue this[TKey key] { get => m_Dict[key]; set => m_Dict[key] = value; }

        public Dictionary<TKey, TValue>.KeyCollection Keys => m_Dict.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => m_Dict.Values;
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => m_Dict.Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => m_Dict.Values;
        public int Count => m_Dict.Count;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)m_Dict).IsReadOnly;


        public KeyValueEntryMap()
        {
            m_Dict = new Dictionary<TKey, TValue>();
        }

        public KeyValueEntryMap(int capacity)
        {
            m_Dict = new Dictionary<TKey, TValue>(capacity);
        }

        public KeyValueEntryMap(IDictionary<TKey, TValue> dict)
        {
            m_Dict = new Dictionary<TKey, TValue>(dict);
        }

        public void Add(TKey key, TValue value) => m_Dict.Add(key, value);
        public void Clear() => m_Dict.Clear();
        public bool ContainsKey(TKey key) => m_Dict.ContainsKey(key);
        public bool ContainsValue(TValue value) => m_Dict.ContainsValue(value);
        public int EnsureCapacity(int capacity) => m_Dict.EnsureCapacity(capacity);
        public Dictionary<TKey, TValue>.Enumerator GetEnumerator() => m_Dict.GetEnumerator();
        public bool Remove(TKey key) => m_Dict.Remove(key);
        public bool Remove(TKey key, out TValue value) => m_Dict.Remove(key, out value);
        public void TrimExcess() => m_Dict.TrimExcess();
        public void TrimExcess(int capacity) => m_Dict.TrimExcess(capacity);
        public void TryAdd(TKey key, TValue value) => m_Dict.TryAdd(key, value);
        public bool TryGetValue(TKey key, out TValue value) => m_Dict.TryGetValue(key, out value);

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)m_Dict).Add(item);
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)m_Dict).Contains(item);
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)m_Dict).CopyTo(array, arrayIndex);
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)m_Dict).Remove(item);

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => m_Dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_Dict.GetEnumerator();

        //TODO 解决创建时Key之冲突的问题
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_Dict == null)
            {
                m_Dict = new Dictionary<TKey, TValue>();
            }
            m_Dict.Clear();
            for (int i = 0; i < m_Maps.Count; i++)
            {
                var _entry = m_Maps[i];
                m_Dict.Add(_entry.Key, _entry.Value);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (m_Dict == null)
            {
                m_Maps.Clear();
                return;
            }
            if (m_Dict.Count != m_Maps.Count)
            {
                m_Maps.Clear();
            }
            int _index = 0;
            foreach (var _kvp in m_Dict)
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
    }
}