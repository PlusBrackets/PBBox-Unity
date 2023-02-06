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
    public partial class SortedMutiLinkedList<TKey, TValue> : IEnumerable<TValue>, IEnumerable where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        //节点cache，减少移除添加时的gc
        private readonly Lazy<ReferenceTempCache<LinkedListNode<KeyItemPair<TKey, TValue>>>> m_NodeCache;
        private readonly Lazy<ReferenceTempCache<LinkedListNode<Group>>> m_GroupNodeCache;

        //终结节点字典，用于快速查询。
        private Dictionary<TKey, LinkedListNode<Group>> m_GroupLookUp;
        //存储实际数据的链表
        private LinkedList<KeyItemPair<TKey, TValue>> m_List;
        //终结节点链表，用于存放order的排序信息以及终结节点的前后信息
        protected LinkedList<Group> m_SortedGroupList;

        public LinkedListNode<KeyItemPair<TKey, TValue>> First => m_List.First;
        public LinkedListNode<KeyItemPair<TKey, TValue>> Last => m_List.Last;

        public int Count => m_List.Count;
        public int GroupCount => m_SortedGroupList.Count;

        public SortedMutiLinkedList()
        {
            m_GroupLookUp = new Dictionary<TKey, LinkedListNode<Group>>();
            m_List = new LinkedList<KeyItemPair<TKey, TValue>>();
            m_SortedGroupList = new LinkedList<Group>();
            m_NodeCache = new Lazy<ReferenceTempCache<LinkedListNode<KeyItemPair<TKey, TValue>>>>();
            m_GroupNodeCache = new Lazy<ReferenceTempCache<LinkedListNode<Group>>>();
        }

        private LinkedListNode<KeyItemPair<TKey, TValue>> TryAcquireNode(KeyItemPair<TKey, TValue> item)
        {
            //从缓存中取LinkedListNode，若无则创建
            if (!m_NodeCache.IsValueCreated || !m_NodeCache.Value.TryAcquire(out var _node))
            {
                _node = new LinkedListNode<KeyItemPair<TKey, TValue>>(item);
            }
            else
            {
                _node.Value = item;
            }
            return _node;
        }

        private LinkedListNode<Group> TryAcquireGroupNode(Group item)
        {
            //从缓存中取LinkedListNode，若无则创建
            if (!m_GroupNodeCache.IsValueCreated || !m_GroupNodeCache.Value.TryAcquire(out var _node))
            {
                _node = new LinkedListNode<Group>(item);
            }
            else
            {
                _node.Value = item;
            }
            return _node;
        }

        protected virtual bool CheckTravelDirection(TKey orderKey)
        {
            return Comparer<TKey>.Default.Compare(orderKey, m_SortedGroupList.Last.Value.OrderKey) > 0;
        }

        private void InsertNewGroupNode(TKey orderKey, LinkedListNode<KeyItemPair<TKey, TValue>> newNode)
        {
            var _newGroupNode = TryAcquireGroupNode(new Group(orderKey, newNode, newNode));
            //若没有数据，则直接放到第一位
            if (m_SortedGroupList.Count == 0)
            {
                m_List.AddFirst((LinkedListNode<KeyItemPair<TKey, TValue>>)newNode);
                m_SortedGroupList.AddFirst(_newGroupNode);
            }
            //判定顺序遍历还是逆序遍历
            else if (CheckTravelDirection(orderKey))
            {
                for (var _group = m_SortedGroupList.First; _group != null; _group = _group.Next)
                {
                    //顺序遍历遇到第一个大于order的，把该group加入到其前面
                    if (Comparer<TKey>.Default.Compare(_group.Value.OrderKey, orderKey) > 0)
                    {
                        m_List.AddBefore(_group.Value.Start, newNode);
                        m_SortedGroupList.AddBefore(_group, _newGroupNode);
                        break;
                    }
                }
            }
            else
            {
                for (var _group = m_SortedGroupList.Last; _group != null; _group = _group.Previous)
                {
                    //逆序遍历遇到第一个比order小的，把该group加入到其后面
                    if (Comparer<TKey>.Default.Compare(_group.Value.OrderKey, orderKey) < 0)
                    {
                        m_List.AddAfter(_group.Value.End, newNode);
                        m_SortedGroupList.AddAfter(_group, _newGroupNode);
                        break;
                    }
                }
            }
            m_GroupLookUp.Add(orderKey, _newGroupNode);
        }

        public LinkedListRange<KeyItemPair<TKey, TValue>> GetGroup(TKey orderKey)
        {
            if (m_GroupLookUp.TryGetValue(orderKey, out var _group))
            {
                return new LinkedListRange<KeyItemPair<TKey, TValue>>(
                    _group.Value.Start,
                    _group.Value.End,
                    _group.Value.Count);
            }
            return LinkedListRange<KeyItemPair<TKey, TValue>>.Empty;
        }

        public bool Contains(KeyItemPair<TKey, TValue> orderItem)
        {
            if (m_GroupLookUp.TryGetValue(orderItem.Key, out var _group))
            {
                return _group.Value.GetNode(orderItem) != null;
            }
            return false;
        }

        public bool Contains(TKey orderKey, TValue item) => Contains(new KeyItemPair<TKey, TValue>(orderKey, item));

        public bool ContainsKey(TKey orderKey)
        {
            return m_GroupLookUp.ContainsKey(orderKey);
        }

        public bool ContainsValue(TValue item)
        {
            for (var _n = First; _n != null; _n = _n.Next)
            {
                if (EqualityComparer<TValue>.Default.Equals(_n.Value.Item, item))
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(TKey orderKey, TValue item) => Add(new KeyItemPair<TKey, TValue>(orderKey, item));

        public void Add(KeyItemPair<TKey, TValue> orderItem)
        {
            LinkedListNode<Group> _groupNode = null;
            LinkedListNode<KeyItemPair<TKey, TValue>> _node = TryAcquireNode(orderItem);

            if (!m_GroupLookUp.TryGetValue(orderItem.Key, out _groupNode))
            {
                InsertNewGroupNode(orderItem.Key, _node);
            }
            else
            {
                _groupNode.Value = _groupNode.Value.AddNode(_node);
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
        public bool Remove(TKey orderKey, TValue item) => Remove(new KeyItemPair<TKey, TValue>(orderKey, item));

        /// <summary>
        /// 移除项目，需要遍历列表获取节点
        /// </summary>
        /// <param name="orderItem"></param>
        /// <returns></returns>
        public bool Remove(KeyItemPair<TKey, TValue> orderItem)
        {
            if (m_GroupLookUp.TryGetValue(orderItem.Key, out var _groupNode))
            {
                var _node = _groupNode.Value.GetNode(orderItem);
                return RemoveInternal(_groupNode, _node);
            }
            return false;
        }

        /// <summary>
        /// 移除项目，不需要遍历获取节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Remove(LinkedListNode<KeyItemPair<TKey, TValue>> node)
        {
            var orderItem = node.Value;
            if (m_GroupLookUp.TryGetValue(orderItem.Key, out var _groupNode))
            {
                return RemoveInternal(_groupNode, node);
            }
            return false;
        }

        private bool RemoveInternal(LinkedListNode<Group> groupNode, LinkedListNode<KeyItemPair<TKey, TValue>> node)
        {
            if (node == null)
            {
                return false;
            }
            groupNode.Value = groupNode.Value.RemoveNode(node, out var _removedNode);
            if (_removedNode != null)
            {
                if (groupNode.Value.Count == 0)
                {
                    m_GroupLookUp.Remove(groupNode.Value.OrderKey);
                    m_SortedGroupList.Remove(groupNode);
                    //回收terminal
                    groupNode.Value = default(Group);
                    m_GroupNodeCache.Value.Release(groupNode);
                }
                _removedNode.Value = default(KeyItemPair<TKey, TValue>);
                m_NodeCache.Value.Release(_removedNode);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            m_List.Clear();
            m_SortedGroupList.Clear();
            m_GroupLookUp.Clear();
            m_NodeCache.Value.Clear();
            m_GroupNodeCache.Value.Clear();
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

        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<TValue>, IEnumerator
        {
            public TValue Current => m_CurrentValue;
            object IEnumerator.Current => m_CurrentValue;
            private readonly LinkedList<KeyItemPair<TKey, TValue>> m_List;
            private LinkedListNode<KeyItemPair<TKey, TValue>> m_Current;
            private TValue m_CurrentValue;

            public Enumerator(LinkedList<KeyItemPair<TKey, TValue>> list)
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
                m_CurrentValue = m_Current.Value.Item;
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