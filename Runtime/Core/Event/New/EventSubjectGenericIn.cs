///*--------------------------------------------------------
// *Copyright (c) 2016-2026 PlusBrackets
// *@update: 2026.11.10
// *@author: PlusBrackets
// --------------------------------------------------------*/
//using System;
//
//namespace PBBox.Event
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <typeparam name="TEvent"></typeparam>
//    public class EventSubjectIn<TEvent> :EventSubjectBase<EventHandler<TEvent>>, IEventSubject
//    {
//        private static string _logTag = $"EventSubjectIn<{nameof(TEvent)}>";
//        protected override string LogTag => _logTag;
//        protected override IEventSubject Subject => this;
//
//        public void Publish(in TEvent eventData)
//        {
//            if(_handlers == null)
//            {
//                return;
//            }
//            for(int i = 0; i < _handlers.Keys.Count; i++)
//            {
//                var order = _handlers.Keys[i];
//                var handlers = _handlers.Values[i];
//                _handlers.SetIterating(true, order);
//                foreach (var handler in handlers)
//                {
//                    try
//                    {
//                        handler?.Invoke(in eventData);
//                    }
//                    catch (Exception ex)
//                    {
//                        Log.Error($"Exception occurred while invoking handler: {ex}", LogTag, Log.PBBoxLoggerName);
//                    }
//                }
//            }
//            _handlers.SetIterating(false, 0);
//            _handlers.ApplyPendingOperations();
//        }
//
//        void IEventSubject.Publish<T>(in T eventData)
//        {
//            if (eventData is TEvent typedEvent)
//            {
//                Publish(in typedEvent);
//            }
//            else
//            {
//                Log.Error($"Event data is not of type {typeof(TEvent).Name}", LogTag, Log.PBBoxLoggerName);
//            }
//        }
//
//        void IEventSubject.Publish()
//        {
//            Log.Error($"Publish called without event data on EventSubjectIn<{nameof(TEvent)}>", LogTag, Log.PBBoxLoggerName);
//        }
//
//        EventSubscriptionNew IEventSubject.Subscribe(Delegate handler, int order)
//        {
//            if (handler is EventHandler<TEvent> typedHandler)
//            {
//                return Subscribe(typedHandler, order);
//            }
//            else
//            {
//                Log.Error($"Handler is not of type EventHandler<{typeof(TEvent).Name}>", LogTag, Log.PBBoxLoggerName);
//                return default;
//            }
//        }
//
//        void IEventSubject.Unsubscribe(Delegate handler, int order)
//        {
//            if (handler is EventHandler<TEvent> typedHandler)
//            {
//                Unsubscribe(typedHandler, order);
//            }
//            else
//            {
//                Log.Error($"Handler is not of type EventHandler<{typeof(TEvent).Name}>", LogTag, Log.PBBoxLoggerName);
//            }
//        }
//    }
//}