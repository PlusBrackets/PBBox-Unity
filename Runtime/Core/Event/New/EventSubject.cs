/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.10
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using PBBox.Collections;

namespace PBBox.Event
{
    public class EventSubject : EventSubjectBase<Delegate>, IEventSubject
    {
        private static string _logTag = $"EventSubject";
        protected override string LogTag => _logTag;
        protected override IEventSubject Subject => this;

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
                        if (handler is Action<T> action)
                        {
                            action.Invoke(eventData);
                        }
                        else if (handler is EventHandler<T> eventHandler)
                        {
                            eventHandler.Invoke(eventData);
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
                        if (handler is not Action action)
                        {
                            continue;
                        }
                        action.Invoke();
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
}