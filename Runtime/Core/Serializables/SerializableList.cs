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
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace PBBox
{

    /// <summary>
    /// 可单独序列化的list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class SList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        #if USE_ODIN
        [InlineProperty]
        #endif
        [SerializeField]
        List<T> values;
        public List<T> Source => values;
        public T this[int index] { get => values[index]; set => values[index] = value; }
        public int Count => values.Count;
        bool ICollection<T>.IsReadOnly => ((ICollection<T>)values).IsReadOnly;


        public SList(List<T> list = null)
        {
            if (list == null)
            {
                this.values = new List<T>();
            }
            else
            {
                this.values = list;
            }
        }

        public int IndexOf(T item) => values.IndexOf(item);
        public void Insert(int index, T item) => values.Insert(index, item);
        public void RemoveAt(int index) => values.RemoveAt(index);
        public void Add(T item) => values.Add(item);
        public void Clear() => values.Clear();
        public bool Contains(T item) => values.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => values.CopyTo(array, arrayIndex);
        public bool Remove(T item) => values.Remove(item);
        public IEnumerator<T> GetEnumerator() => values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();

    }

}