using System;
using System.Collections.Generic;
using PBBox.Collections;

namespace PBBox
{
    public sealed class Updater<T> : IUpdater<T> where T : IUpdateParameter
    {
        private SortedMutiLinkedList<IUpdatable<T>> m_Updatables;
        private LinkedListNode<KeyItemPair<int, IUpdatable<T>>> m_NextUpdater = null;
        public bool IsUpdating { get; private set; } = false;

        public Updater()
        {
            m_Updatables = new SortedMutiLinkedList<IUpdatable<T>>();
        }

        public void Attach(IUpdatable<T> updatable)
        {
            if (updatable.CurrentUpdater != null)
            {
                Log.Warning("Updatable has already attached!", "Updater", Log.PBBoxLoggerName);
                return;
            }
            if (updatable is ISortedUpdatable<T> _updatable)
            {
                m_Updatables.Add(_updatable.SortedOrder, updatable);
            }
            else
            {
                m_Updatables.Add(0, updatable);
            }
            updatable.CurrentUpdater = this;
        }

        public void Unattach(IUpdatable<T> updatable, bool immediately = false)
        {
            if (updatable.CurrentUpdater != this)
            {
                Log.Warning("Updatable and Updater is not match! Can not unattach", "Updater", Log.PBBoxLoggerName);
                return;
            }
            updatable.CurrentUpdater = null;
            if (immediately)
            {
                int _order = (updatable is ISortedUpdatable<T> _updatable) ? _updatable.SortedOrder : 0;
                //要移除的updatable正好为遍历中的下一个updatable
                if (m_NextUpdater != null && updatable == m_NextUpdater.Value.Item)
                {
                    m_NextUpdater = m_NextUpdater.Next;
                    m_Updatables.Remove(m_NextUpdater.Previous);
                }
                else
                {
                    m_Updatables.Remove(_order, updatable);
                }
            }
        }

        public void Update(ref T param)
        {
            IsUpdating = true;
            for (var _node = m_Updatables.First; _node != null; _node = m_NextUpdater)
            {
                m_NextUpdater = _node.Next;
                var _updatable = _node.Value.Item;
                if (_updatable == null || _updatable.CurrentUpdater != this)
                {
                    m_Updatables.Remove(_node);
                    continue;
                }
                if (_updatable.IsUpdateEnable)
                {
                    _updatable.OnUpdate(ref param);
                }
            }
            m_NextUpdater = null;
            IsUpdating = false;
        }

        public void Clear()
        {
            for (var _node = m_Updatables.First; _node != null; _node = m_NextUpdater)
            {
                m_NextUpdater = _node.Next;
                var _updatable = _node.Value.Item;
                if (_updatable.CurrentUpdater == this)
                {
                    _updatable.CurrentUpdater = null;
                }
            }
            m_NextUpdater = null;
            m_Updatables.Clear();
        }

    }
}