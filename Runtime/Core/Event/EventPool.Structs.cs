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
    public sealed partial class EventPool
    {
        private class EventCollections
        {
            //触发事件时立刻调用
            private RefPoolObject<SortedMutiLinkedList<Delegate>> m_Events;
            //update时调用
            private RefPoolObject<SortedMutiLinkedList<Delegate>> m_LateEvents;

            public EventCollections()
            {
                m_Events = new RefPoolObject<SortedMutiLinkedList<Delegate>>();
                m_LateEvents = new RefPoolObject<SortedMutiLinkedList<Delegate>>();
            }

            public void Clear()
            {
                m_Events.Release();
                m_LateEvents.Release();
            }

            public SortedMutiLinkedList<Delegate> GetValue(bool isLateEvent)
            {
                if (isLateEvent)
                {
                    return m_LateEvents.Value;
                }
                else
                {
                    return m_Events.Value;
                }
            }

            /// <summary>
            /// 获取事件列表，如果没有则创建一个新的
            /// </summary>
            /// <param name="isLateEvent"></param>
            /// <returns></returns>
            public SortedMutiLinkedList<Delegate> AcquireValue(bool isLateEvent)
            {
                if (isLateEvent)
                {
                    m_LateEvents.TryAcquire();
                    return m_LateEvents.Value;
                }
                else
                {
                    m_Events.TryAcquire();
                    return m_Events.Value;
                }
            }
            
            public void ReleaseValue(bool isLateEvent)
            {
                if (isLateEvent)
                {
                    m_LateEvents.Release();
                }
                else
                {
                    m_Events.Release();
                }
            }
        }

        private class EventTriggerContext
        {
            public LinkedListNode<KeyValueEntry<int, Delegate>> currentNode;
            //public LinkedListNode<KeyValueEntry<int, Delegate>> nextNode;
        }

        /// <summary>
        /// 队列事件，带有一个泛型表示传入参数
        /// </summary>
        /// <typeparam name="TArgs"></typeparam>
        private class Event<TArgs> : Event
        {
            public TArgs EventArgs { get; set; }
            public IReferenceCacheBase EventArgsReferencePool { get; set; } = null;

            protected override void OnReferenceReleaseImpl()
            {
                if (EventArgsReferencePool != null)
                {
                    if (!(EventArgsReferencePool is IReferencePoolItem poolItem) || poolItem.IsUsing)
                    {
                        EventArgsReferencePool.Release(EventArgs);
                    }
                }
                EventArgsReferencePool = null;
                EventArgs = default(TArgs);
            }

            public override bool Dispatch(Delegate listener)
            {
                if (listener is Action<object, TArgs> handler)
                {
                    handler(Sender, EventArgs);
                }
                else if (listener is Func<object, TArgs, bool> handler2)
                {
                    return handler2(Sender, EventArgs);
                }
                else
                {
                    return base.Dispatch(listener);
                }
                return true;
            }

            protected override void TriggerFallback(Delegate listener)
            {
#if UNITY_EDITOR
                Log.Warning(
                    "Type mismatch, please check if the passed-in args and event handler match."
                    + $"\n( {EventId},  {typeof(TArgs)},  {listener.GetType()} )\n",
                    "EventPool",
                    Log.PBBoxLoggerName);
#endif
            }
        }

        /// <summary>
        /// 队列事件
        /// </summary>
        private class Event : IReferencePoolItem
        {
            bool IReferencePoolItem.IsUsing { get; set; } = true;
            public int EventId { get; set; }
            public object Sender { get; set; }

            public void Release()
            {
                ReferencePool.Release(this);
            }

            void IReferencePoolItem.OnReferenceAcquire() { }

            void IReferencePoolItem.OnReferenceRelease()
            {
                OnReferenceReleaseImpl();
                Sender = null;
                EventId = default;
            }

            protected virtual void OnReferenceReleaseImpl() { }

            public virtual bool Dispatch(Delegate listener)
            {
                if (listener is Action<object> handler)
                {
                    handler(Sender);
                }
                else if (listener is Func<object, bool> handler2)
                {
                    return handler2(Sender);
                }
                else
                {
                    TriggerFallback(listener);
                }
                return true;
            }

            protected virtual void TriggerFallback(Delegate listener)
            {
#if UNITY_EDITOR
                Log.Warning(
                    "Type mismatch, please check if the passed-in args and event handler match."
                    + $"\n( {EventId},  No args,  {listener.GetType()} )\n",
                    "EventPool",
                    Log.PBBoxLoggerName);
#endif
            }
        }


    }
}
