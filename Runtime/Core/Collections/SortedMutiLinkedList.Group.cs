/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.18
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;

namespace PBBox.Collections
{
    public partial class SortedMutiLinkedList<TKey, TValue>
    {
        protected struct Group
        {
            public TKey OrderKey { get; private set; }
            public LinkedListNode<KeyValueEntry<TKey,TValue>> Start { get; private set; }
            public LinkedListNode<KeyValueEntry<TKey,TValue>> End { get; private set; }
            public int Count { get; private set; }

            public Group(TKey key, LinkedListNode<KeyValueEntry<TKey,TValue>> startNode, LinkedListNode<KeyValueEntry<TKey,TValue>> endNode)
            {
                OrderKey = key;
                Start = startNode;
                End = endNode;
                Count = 1;
            }

            public Group AddNode(LinkedListNode<KeyValueEntry<TKey,TValue>> node)
            {
                End.List.AddAfter(End, node);
                End = node;
                Count++;
                return this;
            }

            public Group RemoveNode(KeyValueEntry<TKey,TValue> item, out LinkedListNode<KeyValueEntry<TKey,TValue>> removedNode)
            {
                removedNode = GetNode(item);
                return RemoveNode(removedNode, out removedNode);
            }

            public Group RemoveNode(LinkedListNode<KeyValueEntry<TKey,TValue>> node, out LinkedListNode<KeyValueEntry<TKey,TValue>> removedNode)
            {
                removedNode = null;
                if (node != null && OrderKeyEquals(node.Value))
                {
                    if (End == node && End != Start)
                    {
                        End = End.Previous;
                    }
                    else if (Start == node && Start != End)
                    {
                        Start = Start.Next;
                    }
                    End.List.Remove(node);
                    Count--;
                    removedNode = node;
                }
                return this;
            }

            public LinkedListNode<KeyValueEntry<TKey,TValue>> GetNode(KeyValueEntry<TKey,TValue> item)
            {
                if (!OrderKeyEquals(item))
                {
                    return null;
                }
                for (var n = Start; n != End.Next; n = n.Next)
                {
                    if (EqualityComparer<KeyValueEntry<TKey,TValue>>.Default.Equals(item, n.Value))
                    {
                        return n;
                    }
                }
                return null;
            }

            private bool OrderKeyEquals(KeyValueEntry<TKey,TValue> orderItem)
            {
                return EqualityComparer<TKey>.Default.Equals(orderItem.Key, OrderKey);
            }
        }

    }
}