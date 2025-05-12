/*--------------------------------------------------------
 *Copyright (c) 2016-2024 PlusBrackets
 *@update: 2024.04.24
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Collections
{
    [System.Serializable]
    /// <summary>
    /// 可序列化的HashSet
    /// </summary>
    public sealed class SHashSet<T> : ISerializationCallbackReceiver, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ISet<T>
    {
        
        [SerializeField]
        private T[] m_Elements;
        private Lazy<HashSet<T>> m_HashSet;

        private HashSet<T> mySet => m_HashSet.Value;
        
        public int Count => mySet.Count;
        public bool IsReadOnly => ((ICollection<T>)mySet).IsReadOnly;


        public SHashSet()
        {
            m_HashSet = new Lazy<HashSet<T>>(CreateHashSet);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_HashSet = new Lazy<HashSet<T>>(CreateHashSet);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (m_HashSet != null)
            {
                m_Elements = new T[m_HashSet.Value.Count];
                m_HashSet.Value.CopyTo(m_Elements);
            }
        }

        private HashSet<T> CreateHashSet()
        {
            return m_Elements == null ? new HashSet<T>() : new HashSet<T>(m_Elements);
        }

        public bool Add(T item)
        {
            return ((ISet<T>)mySet).Add(item);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            ((ISet<T>)mySet).ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            ((ISet<T>)mySet).IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return ((ISet<T>)mySet).IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return ((ISet<T>)mySet).IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return ((ISet<T>)mySet).IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return ((ISet<T>)mySet).IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return ((ISet<T>)mySet).Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return ((ISet<T>)mySet).SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            ((ISet<T>)mySet).SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            ((ISet<T>)mySet).UnionWith(other);
        }

        void ICollection<T>.Add(T item)
        {
            ((ICollection<T>)mySet).Add(item);
        }

        public void Clear()
        {
            ((ICollection<T>)mySet).Clear();
        }

        public bool Contains(T item)
        {
            return ((ICollection<T>)mySet).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)mySet).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return ((ICollection<T>)mySet).Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)mySet).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)mySet).GetEnumerator();
        }
    }
}