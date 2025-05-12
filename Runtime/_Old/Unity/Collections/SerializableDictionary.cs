/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.04.12
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox.Collections
{
    // Dictionary<TKey, TValue>
    /// <summary>
    /// 可序列化字典,支持UnityInspector以及Unity的JsonUtility。性能略逊Dictionary（相当于封装Dictionary），但相差不大
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public sealed class SDictionary<TKey, TValue> : KeyValueEntryMap<SKeyValueEntry<TKey, TValue>, TKey, TValue> { }

}