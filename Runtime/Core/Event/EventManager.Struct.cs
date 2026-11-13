/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using PBBox.Event;

namespace PBBox
{
    public sealed partial class EventManager
    {
        private class TypedSubjectContainer
        {
            public IEventSubject DefaultSubject;
            public IEventSubject DefaultSubjectLater;

            public EventSubject<T> GetOrCreateSubject<T>(bool isLater) where T : IEvent
            {
                if (isLater)
                {
                    return (DefaultSubjectLater ?? (DefaultSubjectLater = new EventSubject<T>())) as EventSubject<T>;
                }
                else
                {
                    return (DefaultSubject ?? (DefaultSubject = new EventSubject<T>())) as EventSubject<T>;
                }
            }
        }

        private class IdSubjectContainer
        {
            public EventSubject Subject;
            public EventSubject SubjectLater;

            public EventSubject GetOrCreateSubject(bool isLater)
            {
                if (isLater)
                {
                    return SubjectLater ?? (SubjectLater = new EventSubject());
                }
                else
                {
                    return Subject ?? (Subject = new EventSubject());
                }
            }
        }


        //TODO 视情况决定是否需要将PendingEvent进行池化处理
        private abstract class PendingEventBase
        {
            public abstract void Publish();
        }

        private class PendingIdEvent : PendingEventBase
        {
            public int EventId;
            public EventSubject Subject;

            public PendingIdEvent(EventSubject subject, int eventId)
            {
                Subject = subject;
                EventId = eventId;
            }

            public override void Publish()
            {
                Subject?.Publish();
            }
        }

        private class PendingIdEvent<TData> : PendingIdEvent
        {
            private TData _eventData;
            private bool _isInParamEvent;

            public PendingIdEvent(EventSubject subject, int eventId, TData eventData, bool isInParamEvent)
                : base(subject, eventId)
            {
                _eventData = eventData;
                _isInParamEvent = isInParamEvent;
            }

            public override void Publish()
            {
                if (_isInParamEvent)
                {
                    Subject?.Publish(in _eventData);
                }
                else
                {
                    Subject?.Publish(_eventData);
                }
            }
        }

        private class PendingTypedEvent<TEvent> : PendingEventBase where TEvent : IEvent
        {
            private EventSubject<TEvent> Subject;
            private TEvent _event;
            private bool _isInParamEvent;

            public PendingTypedEvent(EventSubject<TEvent> subject, TEvent ev, bool isInParamEvent)
            {
                Subject = subject;
                _event = ev;
                _isInParamEvent = isInParamEvent;
            }

            public override void Publish()
            {
                if (_isInParamEvent)
                {
                    Subject?.Publish(in _event);
                }
                else
                {
                    Subject?.Publish(_event);
                }
            }
        }

    }
}