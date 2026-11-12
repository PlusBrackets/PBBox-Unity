/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.10
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PBBox.Collections;
using PBBox.Event;

namespace PBBox
{
    public class EventSubject : EventSubjectBase<Delegate>, IEventSubject
    {
        private static string _logTag = $"EventSubject";
        protected override string LogTag => _logTag;
        protected override IEventSubject Subject => this;

        public EventSubscriptionNew Subscribe(Action handler, int order = 0) => SubscribeInternal(handler, order);
        public EventSubscriptionNew Subscribe<T>(Action<T> handler, int order = 0) => SubscribeInternal(handler, order);
        public EventSubscriptionNew Subscribe<T>(ActionIn<T> handler, int order = 0) => SubscribeInternal(handler, order);

        public void Unsbscribe(Action handler, int order = 0) => UnsubscribeInternal(handler, order);
        public void Unsbscribe<T>(Action<T> handler, int order = 0) => SubscribeInternal(handler, order);
        public void Unsbscribe<T>(ActionIn<T> handler, int order = 0) => SubscribeInternal(handler, order);

        EventSubscriptionNew IEventSubject.Subscribe(Delegate handler, int order) => SubscribeInternal(handler, order);

        void IEventSubject.Unsubscribe(Delegate handler, int order) => UnsubscribeInternal(handler, order);

        public void Publish<T>(in T eventData)
        {
            if (_handlers == null)
            {
                return;
            }
            for (int i = 0; i < _handlers.Keys.Count; i++)
            {
                var order = _handlers.Keys[i];
                var handlers = _handlers.Values[i];
                _handlers.SetIterating(true, order);
                foreach (var handler in handlers)
                {
                    try
                    {
                        if (handler is ActionIn<T> actionIn)
                        {
                            actionIn.Invoke(eventData);
                        }
                        else if (handler is Action actionNonParam)
                        {
                            actionNonParam.Invoke();
                        }
                        else if (handler is Action<T> action)
                        {
                            action.Invoke(eventData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception occurred while invoking handler: {ex}", LogTag, Log.PBBoxLoggerName);
                    }
                }
            }
            _handlers.SetIterating(false, 0);
            _handlers.ApplyPendingOperations();
        }

        public void Publish()
        {
            if (_handlers == null)
            {
                return;
            }
            for (int i = 0; i < _handlers.Keys.Count; i++)
            {
                var order = _handlers.Keys[i];
                var handlers = _handlers.Values[i];
                _handlers.SetIterating(true, order);
                foreach (var handler in handlers)
                {
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
            _handlers.SetIterating(false, 0);
            _handlers.ApplyPendingOperations();
        }

        public void Publish<T>(T eventData)
        {
            if (_handlers == null)
            {
                return;
            }
            for (int i = 0; i < _handlers.Keys.Count; i++)
            {
                var order = _handlers.Keys[i];
                var handlers = _handlers.Values[i];
                _handlers.SetIterating(true, order);
                foreach (var handler in handlers)
                {
                    try
                    {
                        if (handler is Action<T> action)
                        {
                            action.Invoke(eventData);
                        }
                        else if (handler is Action actionNonParam)
                        {
                            actionNonParam.Invoke();
                        }
                        else if (handler is ActionIn<T> actionIn)
                        {
                            actionIn.Invoke(eventData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception occurred while invoking handler: {ex}", LogTag, Log.PBBoxLoggerName);
                    }
                }
            }
            _handlers.SetIterating(false, 0);
            _handlers.ApplyPendingOperations();
        }
    }

    public class EventSubject<TEvent> : EventSubjectBase<Action<TEvent>>, IEventSubject, IEventSubject<TEvent>
    {
        private static string _logTag = $"EventSubject<{nameof(TEvent)}>";
        protected override string LogTag => _logTag;
        protected override IEventSubject Subject => this;

        public void Publish(TEvent eventData)
        {
            if (_handlers == null)
            {
                return;
            }
            for (int i = 0; i < _handlers.Keys.Count; i++)
            {
                var order = _handlers.Keys[i];
                var handlers = _handlers.Values[i];
                _handlers.SetIterating(true, order);
                foreach (var handler in handlers)
                {
                    try
                    {
                        handler?.Invoke(eventData);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception occurred while invoking handler: {ex}", LogTag, Log.PBBoxLoggerName);
                    }
                }
            }
            _handlers.SetIterating(false, 0);
            _handlers.ApplyPendingOperations();
        }

        public EventSubscriptionNew Subscribe(Action<TEvent> handler, int order = 0) => SubscribeInternal(handler, order);
        public void Unsubscribe(Action<TEvent> handler, int order = 0) => UnsubscribeInternal(handler, order);


        void IEventSubject.Publish<T>(T eventData)
        {
            if (eventData is TEvent typedEvent)
            {
                Publish(typedEvent);
            }
            else
            {
                Log.Error($"Event data is not of type {typeof(TEvent).Name}", LogTag, Log.PBBoxLoggerName);
            }
        }

        void IEventSubject.Publish<T>(in T eventData) => ((IEventSubject)this).Publish(eventData);

        void IEventSubject.Publish()
        {
            Log.Error($"Publish called without event data on EventSubject<{nameof(TEvent)}>", LogTag, Log.PBBoxLoggerName);
        }

        EventSubscriptionNew IEventSubject.Subscribe(Delegate handler, int order)
        {
            if (handler is Action<TEvent> typedHandler)
            {
                return SubscribeInternal(typedHandler, order);
            }
            else
            {
                Log.Error($"Handler is not of type Action<{typeof(TEvent).Name}>", LogTag, Log.PBBoxLoggerName);
                return default;
            }
        }

        void IEventSubject.Unsubscribe(Delegate handler, int order)
        {
            if (handler is Action<TEvent> typedHandler)
            {
                UnsubscribeInternal(typedHandler, order);
            }
            else
            {
                Log.Error($"Handler is not of type Action<{typeof(TEvent).Name}>", LogTag, Log.PBBoxLoggerName);
            }
        }
    }

    public class EventSubjectIn<TEvent> : EventSubjectBase<ActionIn<TEvent>>, IEventSubject, IEventSubjectIn<TEvent> where TEvent : struct
    {
        private static string _logTag = $"EventSubjectIn<{nameof(TEvent)}>";
        protected override string LogTag => _logTag;
        protected override IEventSubject Subject => this;

        public void Publish(in TEvent eventData)
        {
            if (_handlers == null)
            {
                return;
            }
            for (int i = 0; i < _handlers.Keys.Count; i++)
            {
                var order = _handlers.Keys[i];
                var handlers = _handlers.Values[i];
                _handlers.SetIterating(true, order);
                foreach (var handler in handlers)
                {
                    try
                    {
                        handler?.Invoke(in eventData);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception occurred while invoking handler: {ex}", LogTag, Log.PBBoxLoggerName);
                    }
                }
            }
            _handlers.SetIterating(false, 0);
            _handlers.ApplyPendingOperations();
        }
        public EventSubscriptionNew Subscribe(ActionIn<TEvent> handler, int order = 0) => SubscribeInternal(handler, order);
        public void Unsubscribe(ActionIn<TEvent> handler, int order = 0) => UnsubscribeInternal(handler, order);

        void IEventSubject.Publish<T>(in T eventData)
        {
            if (eventData is TEvent typedEvent)
            {
                Publish(in typedEvent);
            }
            else
            {
                Log.Error($"Event data is not of type {typeof(TEvent).Name}", LogTag, Log.PBBoxLoggerName);
            }
        }

        void IEventSubject.Publish()
        {
            Log.Error($"Publish called without event data on EventSubjectIn<{nameof(TEvent)}>", LogTag, Log.PBBoxLoggerName);
        }

        void IEventSubject.Publish<T>(T eventData) => ((IEventSubject)this).Publish(in eventData);

        EventSubscriptionNew IEventSubject.Subscribe(Delegate handler, int order)
        {
            if (handler is ActionIn<TEvent> typedHandler)
            {
                return SubscribeInternal(typedHandler, order);
            }
            else
            {
                Log.Error($"Handler is not of type EventHandler<{typeof(TEvent).Name}>", LogTag, Log.PBBoxLoggerName);
                return default;
            }
        }

        void IEventSubject.Unsubscribe(Delegate handler, int order)
        {
            if (handler is ActionIn<TEvent> typedHandler)
            {
                UnsubscribeInternal(typedHandler, order);
            }
            else
            {
                Log.Error($"Handler is not of type EventHandler<{typeof(TEvent).Name}>", LogTag, Log.PBBoxLoggerName);
            }
        }
    }
}