using System.Collections;
using System.Collections.Generic;

namespace PBBox
{

    public partial class SortedMutiLinkedList<TKey, TValue>
    {
        protected struct Group
        {
            public TKey OrderKey { get; private set; }
            public LinkedListNode<TValue> Start { get; private set; }
            public LinkedListNode<TValue> End { get; private set; }
            public int Count { get; private set; }

            public Group(TKey key, LinkedListNode<TValue> startNode, LinkedListNode<TValue> endNode)
            {
                OrderKey = key;
                Start = startNode;
                End = endNode;
                Count = 1;
            }

            public Group AddNode(LinkedListNode<TValue> node)
            {
                End.List.AddAfter(End, node);
                End = node;
                Count++;
                return this;
            }

            public Group RemoveNode(TValue item, out LinkedListNode<TValue> removedNode)
            {
                removedNode = GetNode(item);
                if (removedNode != null)
                {
                    if (End == removedNode && End != Start)
                    {
                        End = End.Previous;
                    }
                    else if (Start == removedNode && Start != End)
                    {
                        Start = Start.Next;
                    }
                    End.List.Remove(removedNode);
                    Count--;
                }
                return this;
            }

            public LinkedListNode<TValue> GetNode(TValue item)
            {
                for (var n = Start; n != End.Next; n = n.Next)
                {
                    if (EqualityComparer<TValue>.Default.Equals(item, n.Value))
                    {
                        return n;
                    }
                }
                return null;
            }
        }

    }
}