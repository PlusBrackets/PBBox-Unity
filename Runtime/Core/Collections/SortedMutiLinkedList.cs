/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.18
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

namespace PBBox.Collections
{
    /// <summary>
    /// <para>按序号排列的多重LinkedList。</para>
    /// <para>序号可以相同，不同的序号过多会在插入时造成一定程度的性能问题。遍历性能与LinkedList一致。</para>
    /// <para>使用int作为key，稍微提升插入性能。</para>
    /// </summary>
    public sealed class SortedMutiLinkedList<T> : SortedMutiLinkedList<int, T>
    {
        protected override bool CheckTravelDirection(int order)
        {
            return (m_SortedGroupList.First.Value.OrderKey + m_SortedGroupList.Last.Value.OrderKey) / 2 >= order;
        }
    }

    /// <summary>
    /// <para>按Key排列的多重LinkedList。</para>
    /// <para>Key可以相同，不同的Key过多会在插入时造成一定程度的性能问题。遍历性能与LinkedList一致。</para>
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public partial class SortedMutiLinkedList<TKey, TValue> : IEnumerable<TValue>, IEnumerable, IReferencePoolItem where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        //节点cache，减少移除添加时的gc
        private readonly Lazy<ReferenceTempCache<LinkedListNode<KeyValueEntry<TKey, TValue>>>> m_NodeCache;
        private readonly Lazy<ReferenceTempCache<LinkedListNode<Group>>> m_GroupNodeCache;

        //终结节点字典，用于快速查询。
        private Dictionary<TKey, LinkedListNode<Group>> m_GroupLookUp;
        //存储实际数据的链表
        private LinkedList<KeyValueEntry<TKey, TValue>> m_List;
        //终结节点链表，用于存放order的排序信息以及终结节点的前后信息
        protected LinkedList<Group> m_SortedGroupList;

        public LinkedListNode<KeyValueEntry<TKey, TValue>> First => m_List.First;
        public LinkedListNode<KeyValueEntry<TKey, TValue>> Last => m_List.Last;

        public int Count => m_List.Count;
        public int GroupCount => m_SortedGroupList.Count;

        bool IReferencePoolItem.IsUsing { get; set; } = true;

        public SortedMutiLinkedList()
        {
            m_GroupLookUp = new Dictionary<TKey, LinkedListNode<Group>>();
            m_List = new LinkedList<KeyValueEntry<TKey, TValue>>();
            m_SortedGroupList = new LinkedList<Group>();
            m_NodeCache = new Lazy<ReferenceTempCache<LinkedListNode<KeyValueEntry<TKey, TValue>>>>();
            m_GroupNodeCache = new Lazy<ReferenceTempCache<LinkedListNode<Group>>>();
        }

        private LinkedListNode<KeyValueEntry<TKey, TValue>> TryAcquireNode(KeyValueEntry<TKey, TValue> item)
        {
            //从缓存中取LinkedListNode，若无则创建
            if (!m_NodeCache.IsValueCreated || !m_NodeCache.Value.TryAcquire(out var node))
            {
                node = new LinkedListNode<KeyValueEntry<TKey, TValue>>(item);
            }
            else
            {
                node.Value = item;
            }
            return node;
        }

        private LinkedListNode<Group> TryAcquireGroupNode(Group item)
        {
            //从缓存中取LinkedListNode，若无则创建
            if (!m_GroupNodeCache.IsValueCreated || !m_GroupNodeCache.Value.TryAcquire(out var node))
            {
                node = new LinkedListNode<Group>(item);
            }
            else
            {
                node.Value = item;
            }
            return node;
        }

        protected virtual bool CheckTravelDirection(TKey orderKey)
        {
            return Comparer<TKey>.Default.Compare(orderKey, m_SortedGroupList.Last.Value.OrderKey) > 0;
        }

        private void InsertNewGroupNode(TKey orderKey, LinkedListNode<KeyValueEntry<TKey, TValue>> newNode)
        {
            var newGroupNode = TryAcquireGroupNode(new Group(orderKey, newNode, newNode));
            //若没有数据，则直接放到第一位
            if (m_SortedGroupList.Count == 0)
            {
                m_List.AddFirst((LinkedListNode<KeyValueEntry<TKey, TValue>>)newNode);
                m_SortedGroupList.AddFirst(newGroupNode);
            }
            //判定顺序遍历还是逆序遍历
            else if (CheckTravelDirection(orderKey))
            {
                bool inserted = false;
                for (var group = m_SortedGroupList.First; group != null; group = group.Next)
                {
                    //顺序遍历遇到第一个大于order的，把该group加入到其前面
                    if (Comparer<TKey>.Default.Compare(group.Value.OrderKey, orderKey) > 0)
                    {
                        m_List.AddBefore(group.Value.Start, newNode);
                        m_SortedGroupList.AddBefore(group, newGroupNode);
                        inserted = true;
                        break;
                    }
                }
                // 如果没有找到比它大的，则添加到末尾
                if (!inserted)
                {
                    m_List.AddLast(newNode);
                    m_SortedGroupList.AddLast(newGroupNode);
                }
            }
            else
            {
                bool inserted = false;
                for (var group = m_SortedGroupList.Last; group != null; group = group.Previous)
                {
                    //逆序遍历遇到第一个比order小的，把该group加入到其后面
                    if (Comparer<TKey>.Default.Compare(group.Value.OrderKey, orderKey) < 0)
                    {
                        m_List.AddAfter(group.Value.End, newNode);
                        m_SortedGroupList.AddAfter(group, newGroupNode);
                        inserted = true;
                        break;
                    }
                }
                // 如果没有找到比它小的，则添加到开头
                if (!inserted)
                {
                    m_List.AddFirst(newNode);
                    m_SortedGroupList.AddFirst(newGroupNode);
                }
            }
            m_GroupLookUp.Add(orderKey, newGroupNode);
        }

        public LinkedListRange<KeyValueEntry<TKey, TValue>> GetGroup(TKey orderKey)
        {
            if (m_GroupLookUp.TryGetValue(orderKey, out var group))
            {
                return new LinkedListRange<KeyValueEntry<TKey, TValue>>(
                    group.Value.Start,
                    group.Value.End,
                    group.Value.Count);
            }
            return LinkedListRange<KeyValueEntry<TKey, TValue>>.Empty;
        }

        public LinkedListNode<KeyValueEntry<TKey, TValue>> GetNode(TKey orderKey, TValue item)
        {
            if (m_GroupLookUp.TryGetValue(orderKey, out var group))
            {
                return group.Value.GetNode(new KeyValueEntry<TKey, TValue>(orderKey, item));
            }
            return null;
        }

        public bool Contains(KeyValueEntry<TKey, TValue> orderItem)
        {
            if (m_GroupLookUp.TryGetValue(orderItem.Key, out var group))
            {
                return group.Value.GetNode(orderItem) != null;
            }
            return false;
        }

        public bool Contains(TKey orderKey, TValue item) => Contains(new KeyValueEntry<TKey, TValue>(orderKey, item));

        public bool ContainsKey(TKey orderKey)
        {
            return m_GroupLookUp.ContainsKey(orderKey);
        }

        public bool ContainsValue(TValue item)
        {
            for (var n = First; n != null; n = n.Next)
            {
                if (EqualityComparer<TValue>.Default.Equals(n.Value.Value, item))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsNode(LinkedListNode<KeyValueEntry<TKey, TValue>> node)
        {
            if (node == null)
            {
                return false;
            }
            return m_List == node.List;
        }

        public void Add(TKey orderKey, TValue item) => Add(new KeyValueEntry<TKey, TValue>(orderKey, item));

        public void Add(KeyValueEntry<TKey, TValue> orderItem)
        {
            LinkedListNode<Group> groupNode;
            LinkedListNode<KeyValueEntry<TKey, TValue>> node = TryAcquireNode(orderItem);

            if (!m_GroupLookUp.TryGetValue(orderItem.Key, out groupNode))
            {
                InsertNewGroupNode(orderItem.Key, node);
            }
            else
            {
                groupNode.Value = groupNode.Value.AddNode(node);
            }
        }

        /// <summary>
        /// 移除项目，需要遍历列表获取节点
        /// </summary>
        /// <param name="orderKey"></param>
        /// <param name="item"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public bool Remove(TKey orderKey, TValue item) => Remove(new KeyValueEntry<TKey, TValue>(orderKey, item));

        /// <summary>
        /// 移除项目，需要遍历列表获取节点
        /// </summary>
        /// <param name="orderItem"></param>
        /// <returns></returns>
        public bool Remove(KeyValueEntry<TKey, TValue> orderItem)
        {
            if (m_GroupLookUp.TryGetValue(orderItem.Key, out var groupNode))
            {
                var node = groupNode.Value.GetNode(orderItem);
                return RemoveInternal(groupNode, node);
            }
            return false;
        }

        /// <summary>
        /// 移除项目，不需要遍历获取节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Remove(LinkedListNode<KeyValueEntry<TKey, TValue>> node)
        {
            var orderItem = node.Value;
            if (m_GroupLookUp.TryGetValue(orderItem.Key, out var groupNode))
            {
                return RemoveInternal(groupNode, node);
            }
            return false;
        }

        private bool RemoveInternal(LinkedListNode<Group> groupNode, LinkedListNode<KeyValueEntry<TKey, TValue>> node)
        {
            if (node == null)
            {
                return false;
            }
            groupNode.Value = groupNode.Value.RemoveNode(node, out var removedNode);
            if (removedNode != null)
            {
                if (groupNode.Value.Count == 0)
                {
                    m_GroupLookUp.Remove(groupNode.Value.OrderKey);
                    m_SortedGroupList.Remove(groupNode);
                    //回收terminal
                    groupNode.Value = default(Group);
                    m_GroupNodeCache.Value.Release(groupNode);
                }
                removedNode.Value = default(KeyValueEntry<TKey, TValue>);
                m_NodeCache.Value.Release(removedNode);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            m_List.Clear();
            m_SortedGroupList.Clear();
            m_GroupLookUp.Clear();
            if (m_NodeCache.IsValueCreated)
            {
                m_NodeCache.Value.Clear();
            }
            if (m_GroupNodeCache.IsValueCreated)
            {
                m_GroupNodeCache.Value.Clear();
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(m_List);
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        void IReferencePoolItem.OnReferenceAcquire()
        {
            if(m_List.Count > 0)
            {
                throw new Log.FetalErrorException(
                    "The SortedMutiLinkedList is not empty when acquiring from the reference pool. Please check the code.",
                    "SortedMutiLinkedList",
                    Log.PBBoxLoggerName);
            }
            Clear();
        }

        void IReferencePoolItem.OnReferenceRelease()
        {
            Clear();
        }

        public void Release()
        {
            ReferencePool.Release(this);
        }

        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<TValue>, IEnumerator
        {
            public TValue Current => m_CurrentValue;
            object IEnumerator.Current => m_CurrentValue;
            private readonly LinkedList<KeyValueEntry<TKey, TValue>> m_List;
            private LinkedListNode<KeyValueEntry<TKey, TValue>> m_Current;
            private TValue m_CurrentValue;

            public Enumerator(LinkedList<KeyValueEntry<TKey, TValue>> list)
            {
                m_List = list;
                m_Current = m_List.First;
                m_CurrentValue = default(TValue);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (m_Current == null)
                {
                    return false;
                }
                m_CurrentValue = m_Current.Value.Value;
                m_Current = m_Current.Next;
                return true;
            }

            void IEnumerator.Reset()
            {
                m_Current = m_List.First;
                m_CurrentValue = default(TValue);
            }
        }
    }
}