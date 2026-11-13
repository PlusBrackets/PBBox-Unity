///*--------------------------------------------------------
// *Copyright (c) 2016-2026 PlusBrackets
// *@update: 2026.11.13
// *@author: PlusBrackets
// --------------------------------------------------------*/
//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using PBBox.Collections;
//using PBBox.Event;
//
//namespace PBBox
//{
//    public class RawEventSubject<TEvent> : IEventSubject
//    {
//        private SortedDictionary<int, Action<TEvent>> _handlersCollection = new SortedDictionary<int, Action<TEvent>>();
//
//        public void Publish(TEvent ev)
//        {
//            if (_handlersCollection == null)
//            {
//                return;
//            }
//            foreach (var kvp in _handlersCollection)
//            {
//                var handlers = kvp.Value;
//                handlers?.Invoke(ev);
//            }
//        }
//
//        public EventSubscription Subscribe(Action<TEvent> handler, int order = 0)
//        {
//            if (!_handlersCollection.TryGetValue(order, out var existingHandler))
//            {
//                existingHandler = null;
//            }
//            existingHandler += handler;
//            _handlersCollection[order] = existingHandler;
//            return new EventSubscription(this, handler, order);
//        }
//
//        public void Unsubscribe(Action<TEvent> handler, int order = 0)
//        {
//            if (_handlersCollection.TryGetValue(order, out var existingHandler))
//            {
//                existingHandler -= handler;
//                if (existingHandler == null)
//                {
//                    _handlersCollection.Remove(order);
//                }
//                else
//                {
//                    _handlersCollection[order] = existingHandler;
//                }
//            }
//        }
//
//        void IEventSubject.Publish()
//        {
//            Log.Error($"Publish called without event data for RawEventSubject<{typeof(TEvent).Name}>", "RawEventSubject", Log.PBBoxLoggerName);
//        }
//
//        void IEventSubject.Publish<T>(T eventData)
//        {
//            if (eventData is TEvent typedEventData)
//            {
//                Publish(typedEventData);
//            }
//        }
//
//        void IEventSubject.Publish<T>(in T eventData)
//        {
//            if (eventData is TEvent typedEventData)
//            {
//                Publish(typedEventData);
//            }
//        }
//
//        EventSubscription IEventSubject.Subscribe(Delegate handler, int order)
//        {
//            if (handler is Action<TEvent> typedHandler)
//            {
//                return Subscribe(typedHandler, order);
//            }
//            else
//            {
//                Log.Error($"Handler is not of type Action<{typeof(TEvent).Name}>", "RawEventSubject", Log.PBBoxLoggerName);
//                return EventSubscription.Empty;
//            }
//        }
//
//        void IEventSubject.Unsubscribe(Delegate handler, int order)
//        {
//            if (handler is Action<TEvent> typedHandler)
//            {
//                Unsubscribe(typedHandler, order);
//            }
//            else
//            {
//                Log.Error($"Handler is not of type Action<{typeof(TEvent).Name}>", "RawEventSubject", Log.PBBoxLoggerName);
//            }
//        }
//    }
//}