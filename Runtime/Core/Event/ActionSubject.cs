/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.14
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using PBBox.Event;

namespace PBBox
{
    /// <summary>
    /// 封装Action，方便使用EventSubscription，Publish用ActionSubject.Event?.Invoke().代替
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ActionSubject<T> : IEventSubject where T : Delegate
    {
        public T Event { get; private set; }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="handler"></param>
        /// <returns>返回事件订阅结构体，用于管理订阅的生命周期</returns>
        public EventSubscription Subscribe(T handler)
        {
            Event = Delegate.Combine(Event, handler) as T;
            return new EventSubscription(this, handler, 0);
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="handler"></param>
        public void Unsubscribe(T handler)
        {
            Event = Delegate.Remove(Event, handler) as T;
        }

        EventSubscription IEventSubject.Subscribe(Delegate handler, int order)
        {
            if (handler is T typedHandler)
            {
                return Subscribe(typedHandler);
            }
            return EventSubscription.Empty;
        }

        void IEventSubject.Unsubscribe(Delegate handler, int order)
        {
            if (handler is T typedHandler)
            {
                Unsubscribe(typedHandler);
            }
        }
    }
}