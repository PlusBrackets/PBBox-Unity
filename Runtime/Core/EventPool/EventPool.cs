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
    public sealed partial class EventPool<TKey>
    {
        private readonly Dictionary<TKey, SortedMutiLinkedList<Delegate>> m_EventHandlerDict;
        private readonly Queue<Event> m_EventQueue;
        private readonly Dictionary<Event, LinkedListNode<KeyItemPair<int, Delegate>>> m_NextTriggerHandlers;
        public bool EnableCheckMode { get; set; } = false;

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
                    _event.Emit();
                    _event.Release();
                }
#if !PB_THREAD_UNSAFE
            }
#endif
        }

        private void TriggerEvent<T>(Event e, T eventArgs)
        {
            IEventArgs _eventArgs = eventArgs as IEventArgs;
            if (m_EventHandlerDict.TryGetValue(e.EventId, out var _handlers))
            {
                var _currentHandler = _handlers.First;
                while (_currentHandler != null)
                {
                    m_NextTriggerHandlers[e] = _currentHandler.Next;
                    if (_currentHandler.Value is EventHandler<T> __handler)
                    {
                        //若state被设置为Interrupted，则中断事件的传输
                        if (_eventArgs != null)
                        {
                            if (_eventArgs.EventState == EventArgsState.Interrupted)
                            {
                                break;
                            }
                            else
                            {
                                _eventArgs.EventState = EventArgsState.Sending;
                            }
                        }
                        __handler(e.Sender, eventArgs);
                    }
                    else if (EnableCheckMode)
                    {
                        Log.Warning(
                            "Type mismatch, please check if the passed-in args and event handler match."
                            + $"\n( {e.EventId},  {typeof(T)},  {_currentHandler.Value.GetType()} )\n",
                            "EventPool",
                            Log.PBBoxLoggerName);
                    }
                    _currentHandler = m_NextTriggerHandlers[e];
                }
                m_NextTriggerHandlers.Remove(e);
            }
            if (_eventArgs != null)
            {
                if (_eventArgs.EventState != EventArgsState.Interrupted)
                {
                    _eventArgs.EventState = EventArgsState.Finished;
                }
                _eventArgs.Release();
            }
        }

        private void TriggerEvent(Event e)
        {
            if (m_EventHandlerDict.TryGetValue(e.EventId, out var _handlers))
            {
                var _currentHandler = _handlers.First;
                while (_currentHandler != null)
                {
                    m_NextTriggerHandlers[e] = _currentHandler.Next;
                    if (_currentHandler.Value.Item is Action<object> __handler)
                    {
                        __handler(e.Sender);
                    }
                    else if (EnableCheckMode)
                    {
                        Log.Warning(
                            "Type mismatch, please check if the passed-in args and event handler match."
                            + $"\n( {e.EventId},  No args,  {_currentHandler.Value.GetType()} )\n",
                            "EventPool",
                            Log.PBBoxLoggerName);
                    }
                    _currentHandler = m_NextTriggerHandlers[e];
                }
                m_NextTriggerHandlers.Remove(e);
            }
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

        private void EmitImpl<T>(TKey eventId, object sender, T eventArgs, bool immediately)
        {
            Event e = Event.Create(eventId, sender);
            e.SetTriggerAction(CallTriggerEvent);

            if (eventArgs is IEventArgs _eventArgs)
            {
                _eventArgs.EventState = EventArgsState.Standby;
            }

            if (immediately)
            {
                e.Emit();
                e.Release();
            }
            else
            {
                EnqueueEvent(e);
            }

            void CallTriggerEvent()
            {
                TriggerEvent<T>(e, eventArgs);
            }
        }

        private void EmitImpl(TKey eventId, object sender, bool immediately)
        {
            Event e = Event.Create(eventId, sender);
            e.SetTriggerAction(CallTriggerEvent);

            if (immediately)
            {
                e.Emit();
                e.Release();
            }
            else
            {
                EnqueueEvent(e);
            }

            void CallTriggerEvent()
            {
                TriggerEvent(e);
            }
        }

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public void On<T>(TKey eventId, EventHandler<T> handler, int order = 0) => OnImpl(eventId, handler, order);

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        public void On(TKey eventId, Action<object> handler, int order = 0) => OnImpl(eventId, handler, order);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public void Off<T>(TKey eventId, EventHandler<T> handler, int order = 0) => OffImpl(eventId, handler, order);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        public void Off(TKey eventId, Action<object> handler, int order = 0) => OffImpl(eventId, handler, order);

        /// <summary>
        /// 发送事件，事件会在下一帧触发
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        /// <param name="eventArgs">事件参数可以继承EventArgsBase获得更多的功能</param>
        /// <typeparam name="T"></typeparam>
        public void Emit<T>(TKey eventId, object sender, T eventArgs) => EmitImpl<T>(eventId, sender, eventArgs, false);

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
        public void EmitNow<T>(TKey eventId, object sender, T eventArgs) => EmitImpl<T>(eventId, sender, eventArgs, true);

        /// <summary>
        /// 立即发送事件，这种事件是非线程安全的
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        public void EmitNow(TKey eventId, object sender) => EmitImpl(eventId, sender, true);

        /// <summary>
        /// 中断EventArgs的传递
        /// </summary>
        /// <param name="eventArgs"></param>
        public static void InterruptEventArgs(IEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new Log.FetalErrorException("EventArgs can not be null.", "EventPool", Log.PBBoxLoggerName);
            }
            eventArgs.EventState = EventArgsState.Interrupted;
        }
    }
}