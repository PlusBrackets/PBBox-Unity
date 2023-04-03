/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using PBBox.Collections;

namespace PBBox
{
    /// <summary>
    /// 事件池
    /// </summary>
    public sealed partial class EventPool<TKey> : IReferencePoolItem
    {

        private readonly Dictionary<TKey, SortedMutiLinkedList<Delegate>> m_EventHandlerDict;
        private readonly Queue<Event> m_EventQueue;
        /// <summary>
        /// 下一个要触发的EventHandler的链表节点，用于解决遍历EventHandler链表时移除节点的一些问题。
        /// </summary>
        private readonly Dictionary<Event, LinkedListNode<KeyItemPair<int, Delegate>>> m_NextTriggerHandlers;

        bool IReferencePoolItem.IsUsing { get; set; }
        public bool IsUsing => ((IReferencePoolItem)this).IsUsing;

        public EventPool()
        {
            m_EventHandlerDict = new Dictionary<TKey, SortedMutiLinkedList<Delegate>>();
            m_EventQueue = new Queue<Event>();
            m_NextTriggerHandlers = new Dictionary<Event, LinkedListNode<KeyItemPair<int, Delegate>>>();
        }

        public void Update()
        {
            if (m_EventQueue.Count == 0)
                return;
#if !PB_THREAD_UNSAFE
            lock (m_EventQueue)
            {
#endif
                Event _event = null;
                while (m_EventQueue.TryDequeue(out _event))
                {
                    TriggerEvent(_event);
                }
#if !PB_THREAD_UNSAFE
            }
#endif
        }

        private void TriggerEvent(Event e)
        {
            if (m_EventHandlerDict.TryGetValue(e.EventId, out var _handlers))
            {
                var _currentHandler = _handlers.First;
                bool _keepSending = true;//若Handler返回false时，终止传递事件
                while (_currentHandler != null && _keepSending)
                {
                    m_NextTriggerHandlers[e] = _currentHandler.Next;
                    _keepSending = e.Trigger(_currentHandler.Value.Item);


                    _currentHandler = m_NextTriggerHandlers[e];
                }
                m_NextTriggerHandlers.Remove(e);
            }
            e.Release();
        }

        private void EnqueueEvent(Event e)
        {
#if !PB_THREAD_UNSAFE
            lock (m_EventQueue)
            {
#endif
                m_EventQueue.Enqueue(e);
#if !PB_THREAD_UNSAFE
            }
#endif
        }

        private void OnImpl(TKey eventId, Delegate handler, int order)
        {
            //TODO 未进行线程保护
            if (!m_EventHandlerDict.TryGetValue(eventId, out var _handlers))
            {
                _handlers = new SortedMutiLinkedList<Delegate>();
                m_EventHandlerDict.Add(eventId, _handlers);
            }
            _handlers.Add(order, handler);
        }

        private void OffImpl(TKey eventId, Delegate handler, int order)
        {
            if (m_EventHandlerDict.TryGetValue(eventId, out var _handlers))
            {
                //如果有正准备触发的handler
                if (m_NextTriggerHandlers.Count > 0)
                {
                    Dictionary<Event, LinkedListNode<KeyItemPair<int, Delegate>>> _temp = null;
                    foreach (var kvp in m_NextTriggerHandlers)
                    {
                        //检测正准备触发的handler是否和要移除的handler相同
                        if (EqualityComparer<TKey>.Default.Equals(kvp.Key.EventId, eventId) && kvp.Value != null && kvp.Value.Value.Item == handler)
                        {
                            //有则将next存在temp中
                            if (_temp == null)
                            {
                                _temp = ReferencePool.Acquire<Dictionary<Event, LinkedListNode<KeyItemPair<int, Delegate>>>>();
                            }
                            _temp[kvp.Key] = kvp.Value.Next;
                        }
                    }
                    //移除handler
                    _handlers.Remove(order, handler);
                    if (_temp != null && _temp.Count > 0)
                    {
                        foreach (var kvp in _temp)
                        {
                            //如果下次触发的handler正好被移除，则设置为temp中的值
                            if (m_NextTriggerHandlers[kvp.Key].List == null)
                            {
                                m_NextTriggerHandlers[kvp.Key] = kvp.Value;
                            }
                        }
                        _temp.Clear();
                        ReferencePool.Release(_temp);
                    }

                }
                else
                {
                    _handlers.Remove(order, handler);
                }
            }
        }

        private void EmitImpl<T>(TKey eventId, object sender, T eventArgs, bool immediately, IReferenceCacheBase releaseEventArgsPool)
        {
            var _event = ReferencePool.Acquire<Event<T>>();
            _event.EventId = eventId;
            _event.Sender = sender;
            _event.EventArgs = eventArgs;
            _event.EventArgsReferencePool = releaseEventArgsPool;

            if (immediately)
            {
                TriggerEvent(_event);
            }
            else
            {
                EnqueueEvent(_event);
            }
        }

        private void EmitImpl(TKey eventId, object sender, bool immediately)
        {
            var _event = ReferencePool.Acquire<Event>();
            _event.EventId = eventId;
            _event.Sender = sender;

            if (immediately)
            {
                TriggerEvent(_event);
            }
            else
            {
                EnqueueEvent(_event);
            }
        }

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public void On<T>(TKey eventId, Action<object, T> handler, int order = 0) => OnImpl(eventId, handler, order);

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        public void On(TKey eventId, Action<object> handler, int order = 0) => OnImpl(eventId, handler, order);
        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler">返回false时可以中断事件传递</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public void On<T>(TKey eventId, Func<object, T, bool> handler, int order = 0) => OnImpl(eventId, handler, order);

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler">返回false时可以中断事件传递</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        public void On(TKey eventId, Func<object, bool> handler, int order = 0) => OnImpl(eventId, handler, order);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public void Off<T>(TKey eventId, Action<object, T> handler, int order = 0) => OffImpl(eventId, handler, order);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        public void Off(TKey eventId, Action<object> handler, int order = 0) => OffImpl(eventId, handler, order);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler">返回false时可以中断事件传递</param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public void Off<T>(TKey eventId, Func<object, T, bool> handler, int order = 0) => OffImpl(eventId, handler, order);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler">返回false时可以中断事件传递</param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        public void Off(TKey eventId, Func<object, bool> handler, int order = 0) => OffImpl(eventId, handler, order);

        /// <summary>
        /// 发送事件，事件会在下一帧触发
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        /// <param name="eventArgs">事件参数可以继承EventArgsBase获得更多的功能</param>
        /// <typeparam name="T"></typeparam>
        public void Emit<T>(TKey eventId, object sender, T eventArgs, IReferenceCacheBase releaseEventArgsCache = null) => EmitImpl<T>(eventId, sender, eventArgs, false, releaseEventArgsCache);

        /// <summary>
        /// 发送事件，事件会在下一帧触发
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        public void Emit(TKey eventId, object sender) => EmitImpl(eventId, sender, false);

        /// <summary>
        /// 立即发送事件，这种触发是非线程安全的
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        /// <param name="eventArgs">事件参数可以继承EventArgsBase获得更多的功能</param>
        /// <typeparam name="T"></typeparam>
        public void EmitNow<T>(TKey eventId, object sender, T eventArgs, IReferenceCacheBase releaseEventArgsCache = null) => EmitImpl<T>(eventId, sender, eventArgs, true, releaseEventArgsCache);

        /// <summary>
        /// 立即发送事件，这种事件是非线程安全的
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        public void EmitNow(TKey eventId, object sender) => EmitImpl(eventId, sender, true);

        public void Clear()
        {
            m_NextTriggerHandlers.Clear();
            m_EventHandlerDict.Clear();
            m_EventQueue.Clear();
        }

        void IReferencePoolItem.OnReferenceAcquire()
        {

        }

        void IReferencePoolItem.OnReferenceRelease()
        {
            Clear();
        }
    }
}