/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.24
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using PBBox.Collections;

namespace PBBox
{

    /// <summary>
    /// 事件处理器
    /// </summary>
    [SingletonPriority(-1)]
    public sealed partial class EventPool : IReferencePoolItem, IEventManager, ISingletonLifecycle, ILogicUpdateHandler<LogicUpdater.Default>
    {
        private readonly Dictionary<int, EventCollections> m_EventHandlerDict = new Dictionary<int, EventCollections>();
        private readonly Queue<Event> m_EventQueue = new Queue<Event>();

        /// <summary>
        /// 事件触发上下文栈，用于在触发事件时记录当前处理的节点
        /// </summary>
        private readonly Stack<EventTriggerContext> m_EventTriggerContextStack = new Stack<EventTriggerContext>();
        private ReferenceCache<EventTriggerContext> m_EventTriggerContextStackReferencePool = new ReferenceCache<EventTriggerContext>();

        /// <summary>
        /// 记录每个事件ID的LateListener数量，用于判断是否需要触发Later事件
        /// </summary>
        private Lazy<Dictionary<int, int>> m_LaterListenerCountDict = new Lazy<Dictionary<int, int>>();

        bool IReferencePoolItem.IsUsing { get; set; } = true;
        public bool IsUsing => ((IReferencePoolItem)this).IsUsing;

        LogicUpdater.Default ILogicUpdateHandler<LogicUpdater.Default>.CurrentUpdater { get; set; }
        int ILogicUpdateHandler<LogicUpdater.Default>.SortedOrder => 0;

        public EventPool()
        {
            
        }

        /// <summary>
        /// 如果需要使用LateEvent，请在Update方法中调用此方法来触发Later事件。
        /// </summary>
        public void Update()
        {
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
                var eventList = collections.GetValue(isLateEvent);
                var curListenerNode = eventList?.First;
                if (curListenerNode == null)
                {
                    return;
                }

                bool keepSending = true;//若Handler返回false时，终止传递事件
                var context = m_EventTriggerContextStackReferencePool.Acquire();
                m_EventTriggerContextStack.Push(context);

                while (curListenerNode != null && keepSending)
                {
                    context.currentNode = curListenerNode;
                    //m_NextTriggerHandlers[e] = curListenerNode.Next;
                    keepSending = e.Dispatch(curListenerNode.Value.Value);

                    if (context.currentNode == null)
                    {
                        curListenerNode = eventList.First;
                    }
                    else
                    {
                        curListenerNode = context.currentNode.Next;
                    }
                    //curListenerNode = m_NextTriggerHandlers[e];
                }
                m_EventTriggerContextStack.Pop();
                //m_NextTriggerHandlers.Remove(e);
            }
        }

        private void EnqueueEvent(Event e)
        {
            if (false == (m_LaterListenerCountDict.IsValueCreated && m_LaterListenerCountDict.Value.TryGetValue(e.EventId, out var count) && count > 0))
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

        private EventSubscription SubscribeImpl(int eventId, Delegate listener, int order, bool isLateEvent)
        {
            //TODO 未进行线程保护
            if (!m_EventHandlerDict.TryGetValue(eventId, out var collections))
            {
                collections = new EventCollections();
                m_EventHandlerDict.Add(eventId, collections);
            }
            //var eventList = isLateEvent ? collections.m_LateEvents : collections.m_Events;
            //eventList.AcquiredValue.Add(order, listener);
            var eventList = collections.AcquireValue(isLateEvent);
            eventList.Add(order, listener);
            if (isLateEvent)
            {
                m_LaterListenerCountDict.Value[eventId] = eventList.Count;
            }
            //Log.Debug(eventId + " eventList created: " + eventList.IsCreated);
            return new EventSubscription(this, eventId, listener, order, isLateEvent);
        }

        //TODO 进行事件触发时取消订阅的测试
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
                if (m_EventTriggerContextStack.Count > 0)
                {
                    //正在触发事件
                    var targetNode = eventList.GetNode(order, listener);
                    if (targetNode == null)
                    {
                        return;
                    }
                    foreach (var context in m_EventTriggerContextStack)
                    {
                        if (context.currentNode == targetNode)
                        {
                            context.currentNode = targetNode.Previous;
                        }
                    }
                    eventList.Remove(targetNode);
                }
                else
                {
                    eventList.Remove(order, listener);
                }
                m_LaterListenerCountDict.Value[eventId] = eventList.Count;
                if (eventList.Count == 0)
                {
                    collections.ReleaseValue(isLateEvent);
                    m_EventHandlerDict.Remove(eventId);
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
            //m_NextTriggerHandlers.Clear();
            m_EventTriggerContextStackReferencePool.Clear();
            m_EventTriggerContextStack.Clear();
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

        void ISingletonLifecycle.OnCreateAsSingleton()
        {
            LogicUpdater.Attach(this);
        }

        void ISingletonLifecycle.OnDestroyAsSingleton()
        {
            LogicUpdater.Detach(this, true);
            Clear();
        }

        void ILogicUpdateHandler<LogicUpdater.Default>.OnUpdate(float deltaTime)
        {
            Update();
        }
    }
}
