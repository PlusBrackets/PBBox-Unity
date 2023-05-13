/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.04.12
 *@author: PlusBrackets
 --------------------------------------------------------*/

namespace PBBox.Collections
{
    // Dictionary<TKey, TValue>
    /// <summary>
    /// 可序列化字典,支持UnityInspector以及Unity的JsonUtility。Value添加了[SerializeReference]特性
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public sealed class SReferenceDictionary<TKey, TValue> : KeyValueEntryMap<SReferenceKeyValueEntry<TKey, TValue>, TKey, TValue> { }

}