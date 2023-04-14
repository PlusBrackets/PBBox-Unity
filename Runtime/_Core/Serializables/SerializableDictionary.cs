// /*--------------------------------------------------------
//  *Copyright (c) 2016-2022 PlusBrackets
//  *@update: 2022.12.15
//  *@author: PlusBrackets
//  --------------------------------------------------------*/
// #if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
// #define USE_ODIN
// #endif
// #if USE_ODIN
// using Sirenix.OdinInspector;
// #endif
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Linq;

// namespace PBBox
// {
//     // Dictionary<TKey, TValue>
//     /// <summary>
//     /// 可序列化字典,支持UnityInspector以及Unity的JsonUtility。性能约为Dictionary的一半。
//     /// </summary>
//     /// <typeparam name="TKey"></typeparam>
//     /// <typeparam name="TValue"></typeparam>
//     [System.Serializable, System.Obsolete]
//     public class SDictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<SKeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
//     {
//         [SerializeField]
//         private List<SKeyValuePair<TKey, TValue>> maps = new List<SKeyValuePair<TKey, TValue>>();

//         [NonSerialized]
//         private Lazy<Dictionary<TKey, int>> _keyIndexs;
//         private Dictionary<TKey, int> m_KeyIndexs => _keyIndexs.Value;

//         public SDictionary()
//         {
//             _keyIndexs = new Lazy<Dictionary<TKey, int>>(LazyDictionaryIniter);
//         }

//         public SDictionary(Dictionary<TKey, TValue> target) : this()
//         {
//             foreach (var key in target.Keys)
//             {
//                 maps.Add(new SKeyValuePair<TKey, TValue>(key, target[key]));
//             }
//         }

//         Dictionary<TKey, int> LazyDictionaryIniter()
//         {
//             var d = new Dictionary<TKey, int>();
//             for (int i = 0; i < maps.Count; i++)
//             {
//                 if (!d.TryAdd(maps[i].key, i))
//                 {
//                     DebugUtils.Internal.LogError($"[{GetType().Name}]有重复的key值:{maps[i].key},index:{i}");
//                 }
//             }
//             return d;
//         }

//         public Dictionary<TKey, TValue> ToDictionary()
//         {
//             return ToDictionary(new Dictionary<TKey, TValue>());
//         }

//         public Dictionary<TKey, TValue> ToDictionary(Dictionary<TKey, TValue> dict)
//         {
//             dict.Clear();
//             foreach (var kvp in maps)
//             {
//                 dict.TryAdd(kvp.key, kvp.value);
//             }
//             return dict;
//         }

//         /// <summary>
//         /// 快捷转变，方便使用Linq等利用IEnumerable。
//         /// </summary>
//         /// <returns></returns>
//         public IEnumerable<SKeyValuePair<TKey, TValue>> AsEnumerable() => (IEnumerable<SKeyValuePair<TKey, TValue>>)this;

//         /// <summary>
//         /// 获得index位置的SKeyValuePair<Key,Value>
//         /// </summary>
//         /// <param name="index"></param>
//         /// <returns></returns>
//         public SKeyValuePair<TKey, TValue> GetPair(int index) => maps[index];

//         /// <summary>
//         /// 尝试加入一个对值，若已存在key值，则返回false
//         /// </summary>
//         /// <param name="key"></param>
//         /// <param name="value"></param>
//         /// <returns></returns>
//         public bool TryAdd(TKey key, TValue value)
//         {
//             if (m_KeyIndexs.ContainsKey(key))
//             {
//                 return false;
//             }
//             else
//             {
//                 m_KeyIndexs[key] = maps.Count;
//                 maps.Add(new SKeyValuePair<TKey, TValue>(key, value));
//                 return true;
//             }
//         }

//         #region Interface Func 
//         void ISerializationCallbackReceiver.OnBeforeSerialize()
//         {
//         }

//         void ISerializationCallbackReceiver.OnAfterDeserialize()
//         {
//             _keyIndexs = new Lazy<Dictionary<TKey, int>>(LazyDictionaryIniter);
//         }

//         public TValue this[TKey key]
//         {
//             get => maps[m_KeyIndexs[key]].value;
//             set
//             {
//                 SKeyValuePair<TKey, TValue> keyValue = new SKeyValuePair<TKey, TValue>(key, value);
//                 if (m_KeyIndexs.ContainsKey(key))
//                 {
//                     maps[m_KeyIndexs[key]] = keyValue;
//                 }
//                 else
//                 {
//                     m_KeyIndexs.Add(key, maps.Count);
//                     maps.Add(keyValue);
//                 }
//             }
//         }

//         ICollection<TKey> IDictionary<TKey, TValue>.Keys => maps.Select(t => t.key).ToArray();
//         ICollection<TValue> IDictionary<TKey, TValue>.Values => maps.Select(t => t.value).ToArray();
//         bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<SKeyValuePair<TKey, TValue>>)maps).IsReadOnly;
//         public int Count => maps.Count;

//         public void Add(TKey key, TValue value)
//         {
//             if (!TryAdd(key, value))
//                 throw new ArgumentException("An element with the same key already exists in the dictionary.");
//         }

//         public void Add(KeyValuePair<TKey, TValue> kvp) => Add(kvp.Key, kvp.Value);

//         public bool TryGetValue(TKey key, out TValue value)
//         {
//             if (m_KeyIndexs.TryGetValue(key, out int index))
//             {
//                 value = maps[index].value;
//                 return true;
//             }
//             else
//             {
//                 value = default(TValue);
//                 return false;
//             }
//         }

//         public bool ContainsKey(TKey key) => m_KeyIndexs.ContainsKey(key);

//         public bool Contains(KeyValuePair<TKey, TValue> kvp) => m_KeyIndexs.ContainsKey(kvp.Key);

//         public bool Remove(TKey key)
//         {
//             if (m_KeyIndexs.TryGetValue(key, out int index))
//             {
//                 m_KeyIndexs.Remove(key);
//                 maps.RemoveAt(index);
//                 for (var i = index; i < maps.Count; i++)
//                     m_KeyIndexs[maps[i].key] = i;

//                 return true;
//             }
//             return false;
//         }

//         public bool Remove(KeyValuePair<TKey, TValue> kvp) => Remove(kvp.Key);

//         public void Clear()
//         {
//             maps.Clear();
//             if (_keyIndexs.IsValueCreated)
//             {
//                 m_KeyIndexs.Clear();
//             }
//         }

//         public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
//         {
//             int keyCount = maps.Count;
//             if (array.Length - arrayIndex < keyCount)
//                 throw new ArgumentException("arrayIndex");
//             for (var i = 0; i < keyCount; i++, arrayIndex++)
//             {
//                 var item = maps[i];
//                 array[arrayIndex] = new KeyValuePair<TKey, TValue>(item.key, item.value);
//             }
//         }

//         public IEnumerator<SKeyValuePair<TKey, TValue>> GetEnumerator() => maps.GetEnumerator();

//         IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
//         {
//             return maps.Select(ToKeyValuePair).GetEnumerator();

//             static KeyValuePair<TKey, TValue> ToKeyValuePair(SKeyValuePair<TKey, TValue> kvp)
//             {
//                 return new KeyValuePair<TKey, TValue>(kvp.key, kvp.value);
//             }
//         }

//         IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//         #endregion
//     }

// #if UNITY_EDITOR
//     [UnityEditor.CustomPropertyDrawer(typeof(SDictionary<,>))]
//     internal class SDictionaryDrawer : UnityEditor.PropertyDrawer
//     {
//         public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
//         {
//             var listProperty = property.FindPropertyRelative("maps");
//             UnityEditor.EditorGUI.PropertyField(position, listProperty, label, true);
//         }

//         public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
//         {
//             var listProperty = property.FindPropertyRelative("maps");
//             return UnityEditor.EditorGUI.GetPropertyHeight(listProperty, true);
//         }
//     }
// #endif
// }