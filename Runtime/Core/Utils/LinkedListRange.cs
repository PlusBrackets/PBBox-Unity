/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.18
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

namespace PBBox.Collections
{
    /// <summary>
    /// LinkedList的其中一段
    /// </summary>
    public struct LinkedListRange<T> : IEnumerable<T>, IEnumerable
    {
        public static readonly LinkedListRange<T> Empty = new LinkedListRange<T>(null, null);

        public bool IsVaild => Start != null && End != null;
        public LinkedListNode<T> Start { get; private set; }
        public LinkedListNode<T> End { get; private set; }
        private int m_Count;
        public int Count
        {
            get
            {
                if (IsVaild && m_Count < 0)
                {
                    m_Count = 0;
                    for (var n = Start; n != null && n != End.Next; n = n.Next)
                    {
                        m_Count++;
                    }
                }
                return m_Count;
            }
            internal set
            {
                m_Count = value;
            }
        }

        public LinkedListRange(LinkedListNode<T> start, LinkedListNode<T> end, int count = -1)
        {
            Start = start;
            End = end;
            m_Count = count;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(Start, End);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            public T Current => m_CurrentValue;
            object IEnumerator.Current => m_CurrentValue;
            private readonly LinkedListNode<T> m_Start;
            private readonly LinkedListNode<T> m_End;
            private LinkedListNode<T> m_Current;
            private T m_CurrentValue;

            public Enumerator(LinkedListNode<T> start, LinkedListNode<T> end)
            {
                m_Start = start;
                m_End = end;
                m_Current = m_Start;
                m_CurrentValue = default(T);
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
                m_CurrentValue = m_Current.Value;
                m_Current = m_Current.Next != m_End ? m_Current.Next : null;
                return true;
            }

            void IEnumerator.Reset()
            {
                m_Current = m_Start;
                m_CurrentValue = default(T);
            }
        }
    }
}