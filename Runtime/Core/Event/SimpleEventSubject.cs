/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.17
 *@author: PlusBrackets
 --------------------------------------------------------*/

using System;
using PBBox.Event;

namespace PBBox
{
    /// <summary>
    /// 简单的事件主题，使用 Action<T> 进行事件订阅和发布，Order参数在此实现中被忽略。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SimpleEventSubject<T> : IEventSubject, IEventSubject<T>, IEventPublisher<T>
    {
        private Action<T> _event;

        public void Publish(T eventData)
        {
            _event?.Invoke(eventData);
        }

        void IEventPublisher<T>.Publish(in T eventData)
        {
            _event?.Invoke(eventData);
        }

        public EventSubscription Subscribe(Action<T> handler)
        {
            _event += handler;
            return new EventSubscription(this, handler, 0);
        }

        public void Unsubscribe(Action<T> handler)
        {
            _event -= handler;
        }

        EventSubscription IEventSubject<T>.Subscribe(Action<T> handler, int order) => Subscribe(handler);

        void IEventSubject<T>.Unsubscribe(Action<T> handler, int order) => Unsubscribe(handler);

        EventSubscription IEventSubject.Subscribe(Delegate handler, int order)
        {
            if (handler is Action<T> action)
            {
                return Subscribe(action);
            }
            else
            {
#if UNITY_EDITOR || PB_TEST_LOG
                Log.Error($"Failed to add handler to SimpleEventSubject<{nameof(T)}>, handler: {handler}", "SimpleEventSubject", Log.PBBoxLoggerName);
#endif
                return EventSubscription.Empty;
            }
        }

        void IEventSubject.Unsubscribe(Delegate handler, int order)
        {
            if (handler is Action<T> action)
            {
                Unsubscribe(action);
            }
        }
    }
    
    /// <summary>
    /// 简单的事件主题，使用 ActionIn<T> 进行事件订阅和发布，Order参数在此实现中被忽略。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SimpleEventSubjectIn<T> : IEventSubject, IEventSubjectIn<T>, IEventPublisher<T>
    {
        private ActionIn<T> _event;

        public void Publish(in T eventData)
        {
            _event?.Invoke(eventData);
        }

        void IEventPublisher<T>.Publish(T eventData)
        {
            _event?.Invoke(in eventData);
        }

        public EventSubscription Subscribe(ActionIn<T> handler)
        {
            _event += handler;
            return new EventSubscription(this, handler, 0);
        }

        public void Unsubscribe(ActionIn<T> handler)
        {
            _event -= handler;
        }

        EventSubscription IEventSubjectIn<T>.Subscribe(ActionIn<T> handler, int order) => Subscribe(handler);
        void IEventSubjectIn<T>.Unsubscribe(ActionIn<T> handler, int order) => Unsubscribe(handler);

        EventSubscription IEventSubject.Subscribe(Delegate handler, int order)
        {
            if (handler is ActionIn<T> action)
            {
                return Subscribe(action);
            }
            else
            {
#if UNITY_EDITOR || PB_TEST_LOG
                Log.Error($"Failed to add handler to SimpleEventSubjectIn<{nameof(T)}>, handler: {handler}", "SimpleEventSubjectIn", Log.PBBoxLoggerName);
#endif
                return EventSubscription.Empty;
            }
        }

        void IEventSubject.Unsubscribe(Delegate handler, int order)
        {
            if (handler is ActionIn<T> action)
            {
                Unsubscribe(action);
            }
        }

    }
}