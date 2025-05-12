using System.Collections.ObjectModel;
/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.07.09
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Collections
{
    // List<T>
    /// <summary>
    /// 可序列化列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public sealed class SList<T>:IList<T>,ICollection<T>, IEnumerable<T>,IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        [SerializeField]
        private List<T> m_List;

        public T this[int index] { get => m_List[index]; set => m_List[index] = value; }
        object IList.this[int index] { get => ((IList)m_List)[index]; set => ((IList)m_List)[index] = value; }

        public int Count => m_List.Count;
        public int capacity => m_List.Capacity;
        
        bool ICollection<T>.IsReadOnly => ((IList<T>)m_List).IsReadOnly;
        bool IList.IsReadOnly => ((IList)m_List).IsReadOnly;
        bool IList.IsFixedSize => ((IList)m_List).IsFixedSize;
        bool ICollection.IsSynchronized => ((ICollection)m_List).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection)m_List).SyncRoot;
        
        /// <summary>
        /// 原始列表
        /// </summary>
        public List<T> Original => m_List;

        public SList()
        {
            m_List = new List<T>();
        }

        public SList(int capacity)
        {
            m_List = new List<T>(capacity);
        }

        public SList(IEnumerable<T> collection)
        {
            m_List = new List<T>(collection);
        }

        #region List Original Methods
        public void Add(T item) => m_List.Add(item);
        public void AddRange(IEnumerable<T> collection) => m_List.AddRange(collection);
        public ReadOnlyCollection<T> AsReadOnly() => m_List.AsReadOnly();
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) => m_List.BinarySearch(index, count, item, comparer);
        public int BinarySearch(T item) => m_List.BinarySearch(item);
        public int BinarySearch(T item, IComparer<T> comparer) => m_List.BinarySearch(item, comparer);
        public void Clear() => m_List.Clear();
        public bool Contains(T item) => m_List.Contains(item);
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) => m_List.ConvertAll(converter);
        public void CopyTo(int index, T[] array, int arrayIndex, int count) => m_List.CopyTo(index, array, arrayIndex, count);
        public void CopyTo(T[] array) => m_List.CopyTo(array);
        public void CopyTo(T[] array, int arrayIndex) => m_List.CopyTo(array, arrayIndex);
        public bool Exists(Predicate<T> match) => m_List.Exists(match);
        public T Find(Predicate<T> match) => m_List.Find(match);
        public List<T> FindAll(Predicate<T> match) => m_List.FindAll(match);
        public int FindIndex(int startIndex, int count, Predicate<T> match) => m_List.FindIndex(startIndex, count, match);
        public int FindIndex(int startIndex, Predicate<T> match) => m_List.FindIndex(startIndex, match);
        public int FindIndex(Predicate<T> match) => m_List.FindIndex(match);
        public T FindLast(Predicate<T> match) => m_List.FindLast(match);
        public int FindLastIndex(int startIndex, int count, Predicate<T> match) => m_List.FindLastIndex(startIndex, count, match);
        public int FindLastIndex(int startIndex, Predicate<T> match) => m_List.FindLastIndex(startIndex, match);
        public int FindLastIndex(Predicate<T> match) => m_List.FindLastIndex(match);
        public void ForEach(Action<T> action) => m_List.ForEach(action);
        public List<T>.Enumerator GetEnumerator() => m_List.GetEnumerator();
        public List<T> GetRange(int index, int count) => m_List.GetRange(index, count);
        public int IndexOf(T item, int index, int count) => m_List.IndexOf(item, index, count);
        public int IndexOf(T item, int index) => m_List.IndexOf(item, index);
        public int IndexOf(T item) => m_List.IndexOf(item);
        public void Insert(int index, T item) => m_List.Insert(index, item);
        public void InsertRange(int index, IEnumerable<T> collection) => m_List.InsertRange(index, collection);
        public int LastIndexOf(T item, int index, int count) => m_List.LastIndexOf(item, index, count);
        public int LastIndexOf(T item, int index) => m_List.LastIndexOf(item, index);
        public int LastIndexOf(T item) => m_List.LastIndexOf(item);
        public bool Remove(T item) => m_List.Remove(item);
        public int RemoveAll(Predicate<T> match) => m_List.RemoveAll(match);
        public void RemoveAt(int index) => m_List.RemoveAt(index);
        public void RemoveRange(int index, int count) => m_List.RemoveRange(index, count);
        public void Reverse(int index, int count) => m_List.Reverse(index, count);
        public void Reverse() => m_List.Reverse();
        public void Sort(Comparison<T> comparison) => m_List.Sort(comparison);
        public void Sort(int index, int count, IComparer<T> comparer) => m_List.Sort(index, count, comparer);
        public void Sort() => m_List.Sort();
        public void Sort(IComparer<T> comparer) => m_List.Sort(comparer);
        public T[] ToArray() => m_List.ToArray();
        public void TrimExcess() => m_List.TrimExcess();
        public bool TrueForAll(Predicate<T> match) => m_List.TrueForAll(match);
        #endregion

        #region Interface Methods
        int IList.Add(object value) => ((IList)m_List).Add(value);
        bool IList.Contains(object value) => ((IList)m_List).Contains(value);
        int IList.IndexOf(object value) => ((IList)m_List).IndexOf(value);
        void IList.Insert(int index, object value) => ((IList)m_List).Insert(index, value);
        void IList.Remove(object value) => ((IList)m_List).Remove(value);
        void ICollection.CopyTo(Array array, int index) => ((ICollection)m_List).CopyTo(array, index);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_List).GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)m_List).GetEnumerator();

        #endregion
    }

}