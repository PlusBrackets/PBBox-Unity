/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.24
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using PBBox.Collections;
using UnityEngine;
using UnityEngine.Profiling;

namespace PBBox
{

    /// <summary>
    /// 事件处理器
    /// 如果要OnLate方法生效，调用Update方法来触发Later事件，否则请关闭Later事件
    /// </summary>
    public sealed partial class EventPool : IReferencePoolItem
    {
        private readonly Dictionary<int, EventCollections> m_EventHandlerDict;
        private readonly Queue<Event> m_EventQueue;
        /// <summary>
        /// 下一个要触发的EventHandler的链表节点，用于解决遍历EventHandler链表时移除节点的一些问题。
        /// </summary>
        private readonly Dictionary<Event, LinkedListNode<KeyValueEntry<int, Delegate>>> m_NextTriggerHandlers;
        /// <summary>
        /// 是否启用LateEvent，默认启用，如果不需要使用LateEvent，请关闭它，否则会堆积Event对象
        /// </summary>
        public bool EnableLateEvent { get; set; } = true;

        bool IReferencePoolItem.IsUsing { get; set; } = true;
        public bool IsUsing => ((IReferencePoolItem)this).IsUsing;

        public EventPool()
        {
            m_EventHandlerDict = new Dictionary<int, EventCollections>();
            m_EventQueue = new Queue<Event>();
            m_NextTriggerHandlers = new Dictionary<Event, LinkedListNode<KeyValueEntry<int, Delegate>>>();
        }

        public EventPool(bool enableLateEvent)
        {
            m_EventHandlerDict = new Dictionary<int, EventCollections>();
            m_EventQueue = new Queue<Event>();
            m_NextTriggerHandlers = new Dictionary<Event, LinkedListNode<KeyValueEntry<int, Delegate>>>();
            EnableLateEvent = enableLateEvent;
        }

        public void Update()
        {
            if (!EnableLateEvent)
                return;
            if (m_EventQueue.Count == 0)
                return;
#if PB_THREAD_SAFE
            lock (m_EventQueue)
            {
#endif
            Event e = null;
            while (m_EventQueue.TryDequeue(out e))
            {
                TriggerEvent(e, true);
                e.Release();
            }
#if PB_THREAD_SAFE
            }
#endif
        }

        private void TriggerEvent(Event e, bool isLateEvent)
        {
            if (m_EventHandlerDict.TryGetValue(e.EventId, out var collections))
            {
                var curListenerNode = collections.GetValue(isLateEvent)?.First;
                if (curListenerNode == null)
                {
                    return;
                }
                bool keepSending = true;//若Handler返回false时，终止传递事件
                
                while (curListenerNode != null && keepSending)
                {
                    m_NextTriggerHandlers[e] = curListenerNode.Next;
                    keepSending = e.Dispatch(curListenerNode.Value.Value);

                    curListenerNode = m_NextTriggerHandlers[e];
                }
                m_NextTriggerHandlers.Remove(e);
            }
        }

        private void EnqueueEvent(Event e)
        {
            if (!EnableLateEvent)
            {
                e.Release();
                return;
            }
#if PB_THREAD_SAFE
            lock (m_EventQueue)
            {
#endif
            m_EventQueue.Enqueue(e);
#if PB_THREAD_SAFE
            }
#endif
        }

        private Subscription SubscribeImpl(int eventId, Delegate listener, int order, bool isLateEvent)
        {
            if(isLateEvent && !EnableLateEvent)
            {
                Log.Error($"This event is late event, but the late event is disabled, please enable it. EventId: {eventId}", "EventPool", Log.PBBoxLoggerName);
                return default;
            }
            //TODO 未进行线程保护
            if (!m_EventHandlerDict.TryGetValue(eventId, out var collections))
            {
                collections = new EventCollections();
                m_EventHandlerDict.Add(eventId, collections);
            }
            //var eventList = isLateEvent ? collections.m_LateEvents : collections.m_Events;
            //eventList.AcquiredValue.Add(order, listener);
            collections.AcquireValue(isLateEvent).Add(order, listener);
            //Log.Debug(eventId + " eventList created: " + eventList.IsCreated);
            return new Subscription(this, eventId, listener, order, isLateEvent);
        }

        private void UnsubscribeImpl(int eventId, Delegate listener, int order, bool isLateEvent)
        {
            if (m_EventHandlerDict.TryGetValue(eventId, out var collections))
            {
                //var eventList = isLateEvent ? collections.m_LateEvents : collections.m_Events;
                var eventList = collections.GetValue(isLateEvent);
                if (eventList == null)
                {
                    return;
                }
                //如果有正准备触发的handler
                if (m_NextTriggerHandlers.Count > 0)
                {
                    //TODO 尝试优化，使用当前正在触发的Node来判断
                    Dictionary<Event, LinkedListNode<KeyValueEntry<int, Delegate>>> temp = null;
                    foreach (var kvp in m_NextTriggerHandlers)
                    {
                        //检测正准备触发的handler是否和要移除的handler相同
                        if (kvp.Key.EventId == eventId && kvp.Value != null && kvp.Value.Value.Value == listener)
                        {
                            //有则将next存在temp中
                            if (temp == null)
                            {
                                temp = ReferencePool.Acquire<Dictionary<Event, LinkedListNode<KeyValueEntry<int, Delegate>>>>();
                            }
                            temp[kvp.Key] = kvp.Value.Next;
                        }
                    }
                    //移除handler
                    eventList.Remove(order, listener);
                    if (temp != null && temp.Count > 0)
                    {
                        foreach (var kvp in temp)
                        {
                            //如果下次触发的handler正好被移除，则设置为temp中的值
                            if (m_NextTriggerHandlers[kvp.Key].List == null)
                            {
                                m_NextTriggerHandlers[kvp.Key] = kvp.Value;
                            }
                        }
                        temp.Clear();
                        ReferencePool.Release(temp);
                    }

                }
                else
                {
                    eventList.Remove(order, listener);
                }
            }
        }

        private void EmitImpl(int eventId, object sender)
        {
            var e = ReferencePool.Acquire<Event>();
            e.EventId = eventId;
            e.Sender = sender;
            TriggerEvent(e, false);
            EnqueueEvent(e);
        }

        private void EmitImpl<TEventArgs>(int eventId, object sender, TEventArgs args, IReferenceCacheBase releaseEventArgsPool)
        {
            var e = ReferencePool.Acquire<Event<TEventArgs>>();
            e.EventId = eventId;
            e.Sender = sender;
            e.EventArgs = args;
            e.EventArgsReferencePool = releaseEventArgsPool;
            TriggerEvent(e, false);
            EnqueueEvent(e);
        }

        public void Clear()
        {
            m_NextTriggerHandlers.Clear();
            foreach (var kvp in m_EventHandlerDict)
            {
                kvp.Value.Clear();
            }
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
        
        /// <summary>
        /// 从字符串中获取事件ID
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public static int GetEventId(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
                return 0;

            unchecked
            {
                int hash = 23;
                foreach (char c in eventName)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }
    }
}
