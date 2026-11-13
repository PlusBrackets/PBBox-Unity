/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.10
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using PBBox.Event;

namespace PBBox
{
    /// <summary>
    /// <para>事件主题，用于处理事件的订阅和发布</para>
    /// <para>可以用不同参数类型的Handler进行订阅，但只有匹配的参数类型才能被正确触发（注意：此情况下依旧会遍历所有handler）</para>
    /// <para>有较好且稳定的订阅和取消订阅性能，发布遍历性能比多播委托稍慢(10000+事件时为多播委托的2倍左右)</para>
    /// <para>Order数量建议不要过多，以避免性能下降</para>
    /// </summary>
    public sealed class EventSubject : EventSubjectBase<Delegate>, IEventSubject, IEventSubjectGeneric
    {
        protected override string LogTag => "EventSubject";
        protected override IEventSubject Subject => this;

        /// <summary>
        /// 订阅无参数事件
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <returns>事件订阅对象，用于管理订阅的生命周期</returns>
        public EventSubscription Subscribe(Action handler, int order = 0) => SubscribeInternal(handler, order);
        /// <summary>
        /// 订阅带参数事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <returns>事件订阅对象，用于管理订阅的生命周期</returns>
        public EventSubscription Subscribe<T>(Action<T> handler, int order = 0) => SubscribeInternal(handler, order);
        /// <summary>
        /// 订阅带参数事件，一般用于大型结构体参数，避免值类型的拷贝开销
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <returns>事件订阅对象，用于管理订阅的生命周期</returns>
        public EventSubscription Subscribe<T>(ActionIn<T> handler, int order = 0) => SubscribeInternal(handler, order);

        /// <summary>
        /// 取消订阅无参数事件
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序，需要与订阅时保持一致</param>
        public void Unsubscribe(Action handler, int order = 0) => UnsubscribeInternal(handler, order);
        /// <summary>
        /// 取消订阅带参数事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序，需要与订阅时保持一致</param>
        public void Unsubscribe<T>(Action<T> handler, int order = 0) => SubscribeInternal(handler, order);
        /// <summary>
        /// 取消订阅带参数事件，一般用于大型结构体参数，避免值类型的拷贝开销
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序，需要与订阅时保持一致</param>
        public void Unsubscribe<T>(ActionIn<T> handler, int order = 0) => SubscribeInternal(handler, order);

        EventSubscription IEventSubject.Subscribe(Delegate handler, int order) => SubscribeInternal(handler, order);
        void IEventSubject.Unsubscribe(Delegate handler, int order) => UnsubscribeInternal(handler, order);

        public void Publish<T>(T eventData)
        {
            if (_handlersCollection == null)
            {
                return;
            }
            foreach (var kvp in _handlersCollection)
            {
                _handlersCollection.SetIterating(true, kvp.Key);
                var rawHandlers = kvp.Value.RawItems;
                var rawHandlerCount = rawHandlers.Count;
                for (int j = 0; j < rawHandlerCount; j++)
                {
                    var handler = rawHandlers[j];
                    if (handler == null)
                    {
                        continue;
                    }
                    try
                    {
                        if (handler is Action<T> action)
                        {
                            action.Invoke(eventData);
                        }
                        else if (handler is ActionIn<T> actionIn)
                        {
                            actionIn.Invoke(in eventData);
                        }
                        else if (handler is Action actionNonParam)
                        {
                            actionNonParam.Invoke();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception occurred while invoking handler: {ex}", LogTag, Log.PBBoxLoggerName);
                    }
                }
            }
            _handlersCollection.SetIterating(false, 0);
            _handlersCollection.ApplyPendingOperations();
        }

        public void Publish<T>(in T eventData)
        {
            if (_handlersCollection == null)
            {
                return;
            }
            foreach (var kvp in _handlersCollection)
            {
                _handlersCollection.SetIterating(true, kvp.Key);
                var rawHandlers = kvp.Value.RawItems;
                var rawHandlerCount = rawHandlers.Count;
                for (int j = 0; j < rawHandlerCount; j++)
                {
                    var handler = rawHandlers[j];
                    if (handler == null)
                    {
                        continue;
                    }
                    try
                    {
                        if (handler is ActionIn<T> actionIn)
                        {
                            actionIn.Invoke(in eventData);
                        }
                        else if (handler is Action<T> action)
                        {
                            action.Invoke(eventData);
                        }
                        else if (handler is Action actionNonParam)
                        {
                            actionNonParam.Invoke();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception occurred while invoking handler: {ex}", LogTag, Log.PBBoxLoggerName);
                    }
                }
            }
            _handlersCollection.SetIterating(false, 0);
            _handlersCollection.ApplyPendingOperations();
        }

        public void Publish()
        {
            if (_handlersCollection == null)
            {
                return;
            }
            foreach (var kvp in _handlersCollection)
            {
                _handlersCollection.SetIterating(true, kvp.Key);
                var rawHandlers = kvp.Value.RawItems;
                var rawHandlerCount = rawHandlers.Count;
                for (int j = 0; j < rawHandlerCount; j++)
                {
                    var handler = rawHandlers[j];
                    if (handler == null)
                    {
                        continue;
                    }
                    try
                    {
                        if (handler is Action action)
                        {
                            action.Invoke();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception occurred while invoking handler: {ex}", LogTag, Log.PBBoxLoggerName);
                    }
                }
            }
            _handlersCollection.SetIterating(false, 0);
            _handlersCollection.ApplyPendingOperations();
        }

    }


    /// <summary>
    /// <para>特定事件类型的事件主题，用于处理事件的订阅和发布</para>
    /// <para>限定事件参数类型为TEvent，可用无参Handler订阅</para>
    /// <para>有较好且稳定的订阅和取消订阅性能，发布遍历性能比多播委托稍慢(10000+事件时为多播委托的2倍左右)</para>
    /// <para>Order数量建议不要过多，以避免性能下降</para>
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public sealed class EventSubject<TEvent> : EventSubjectBase<Delegate>, IEventSubject, IEventSubject<TEvent>, IEventSubjectIn<TEvent>
    {
        private static readonly string _logTag = $"EventSubject<{nameof(TEvent)}>";
        protected override string LogTag => _logTag;
        protected override IEventSubject Subject => this;

        public EventSubscription Subscribe(Action<TEvent> handler, int order = 0) => SubscribeInternal(handler, order);
        public EventSubscription Subscribe(ActionIn<TEvent> handler, int order = 0) => SubscribeInternal(handler, order);

        public void Unsubscribe(Action<TEvent> handler, int order = 0) => UnsubscribeInternal(handler, order);
        public void Unsubscribe(ActionIn<TEvent> handler, int order = 0) => UnsubscribeInternal(handler, order);

        EventSubscription IEventSubject.Subscribe(Delegate handler, int order) => SubscribeInternal(handler as Action<TEvent>, order);
        void IEventSubject.Unsubscribe(Delegate handler, int order) => UnsubscribeInternal(handler as Action<TEvent>, order);

        public void Publish(TEvent eventData)
        {
            if (_handlersCollection == null)
            {
                return;
            }
            foreach (var kvp in _handlersCollection)
            {
                _handlersCollection.SetIterating(true, kvp.Key);
                var rawHandlers = kvp.Value.RawItems;
                var rawHandlerCount = rawHandlers.Count;
                for (int j = 0; j < rawHandlerCount; j++)
                {
                    var handler = rawHandlers[j];
                    if (handler == null)
                    {
                        continue;
                    }
                    try
                    {
                        if (handler is Action<TEvent> action)
                        {
                            action.Invoke(eventData);
                        }
                        else if (handler is ActionIn<TEvent> actionIn)
                        {
                            actionIn.Invoke(in eventData);
                        }
                        else if (handler is Action actionNoParam)
                        {
                            actionNoParam.Invoke();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception occurred while invoking handler: {ex}", LogTag, Log.PBBoxLoggerName);
                    }
                }
            }
            _handlersCollection.SetIterating(false, 0);
            _handlersCollection.ApplyPendingOperations();
        }

        public void Publish(in TEvent eventData)
        {
            if (_handlersCollection == null)
            {
                return;
            }
            foreach (var kvp in _handlersCollection)
            {
                _handlersCollection.SetIterating(true, kvp.Key);
                var rawHandlers = kvp.Value.RawItems;
                var rawHandlerCount = rawHandlers.Count;
                for (int j = 0; j < rawHandlerCount; j++)
                {
                    var handler = rawHandlers[j];
                    if (handler == null)
                    {
                        continue;
                    }
                    try
                    {
                        if (handler is ActionIn<TEvent> action)
                        {
                            action.Invoke(in eventData);
                        }
                        else if (handler is Action<TEvent> actionIn)
                        {
                            actionIn.Invoke(eventData);
                        }
                        else if (handler is Action actionNoParam)
                        {
                            actionNoParam.Invoke();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception occurred while invoking handler: {ex}", LogTag, Log.PBBoxLoggerName);
                    }
                }
            }
            _handlersCollection.SetIterating(false, 0);
            _handlersCollection.ApplyPendingOperations();
        }
    }
}