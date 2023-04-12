/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.02.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using PBBox.Collections;

namespace PBBox
{
    /// <summary>
    /// 更新器，可以按顺序执行Attach进来的UpdaterHandler
    /// </summary>
    /// <typeparam name="TUpdater"></typeparam>
    public abstract class LogicUpdaterBase<TUpdater> : ILogicUpdater<TUpdater> where TUpdater : LogicUpdaterBase<TUpdater>
    {
        private SortedMutiLinkedList<ILogicUpdateHandler<TUpdater>> m_Handlers;
        private LinkedListNode<KeyValueEntry<int, ILogicUpdateHandler<TUpdater>>> m_NextHandler = null;
        public bool IsUpdating { get; private set; } = false;
        public int Count => m_Handlers.Count;

        public LogicUpdaterBase()
        {
            m_Handlers = new SortedMutiLinkedList<ILogicUpdateHandler<TUpdater>>();
        }

        public void Attach(ILogicUpdateHandler<TUpdater> handler)
        {
            if (handler.CurrentUpdater != null)
            {
                Log.Warning("Handler has already attached!", "Updater", Log.PBBoxLoggerName);
                return;
            }
            m_Handlers.Add(handler.SortedOrder, handler);
            handler.CurrentUpdater = (TUpdater)this;
        }

        public void Unattach(ILogicUpdateHandler<TUpdater> handler, bool immediately = false)
        {
            if (handler.CurrentUpdater != this)
            {
                Log.Warning("Handler and Updater is not match! Can not unattach", "Updater", Log.PBBoxLoggerName);
                return;
            }
            handler.CurrentUpdater = null;
            if (immediately)
            {
                //要移除的updatable正好为遍历中的下一个updatable
                if (m_NextHandler != null && handler == m_NextHandler.Value.Value)
                {
                    m_NextHandler = m_NextHandler.Next;
                    m_Handlers.Remove(m_NextHandler.Previous);
                }
                else
                {
                    m_Handlers.Remove(handler.SortedOrder, handler);
                }
            }
        }

        public void UpdateHandlers(float deltaTime)
        {
            IsUpdating = true;
            for (var _node = m_Handlers.First; _node != null; _node = m_NextHandler)
            {
                m_NextHandler = _node.Next;
                var _updatable = _node.Value.Value;
                if (_updatable == null || _updatable.CurrentUpdater != this)
                {
                    m_Handlers.Remove(_node);
                    continue;
                }
                _updatable.OnUpdate(deltaTime);
            }
            m_NextHandler = null;
            IsUpdating = false;
        }

        public void Clear()
        {
            for (var _node = m_Handlers.First; _node != null; _node = m_NextHandler)
            {
                m_NextHandler = _node.Next;
                var _updatable = _node.Value.Value;
                if (_updatable.CurrentUpdater == this)
                {
                    _updatable.CurrentUpdater = null;
                }
            }
            m_NextHandler = null;
            m_Handlers.Clear();
        }
    }
}