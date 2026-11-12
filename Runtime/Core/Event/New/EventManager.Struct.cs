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
            public IEventSubject InParamSubject;
            public IEventSubject InParamSubjectLater;
        }

        private class IdSubjectContainer
        {
            public EventSubject Subject;
            public EventSubject SubjectLater;
        }

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
            private bool _isInParamHandler;

            public PendingIdEvent(EventSubject subject, int eventId, TData eventData, bool isInParamHandler)
                : base(subject, eventId)
            {
                _eventData = eventData;
                _isInParamHandler = isInParamHandler;
            }

            public override void Publish()
            {
                if (_isInParamHandler)
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

            public PendingTypedEvent(EventSubject<TEvent> subject, TEvent ev)
            {
                Subject = subject;
                _event = ev;
            }

            public override void Publish()
            {
                Subject?.Publish(_event);
            }
        }

        private class PendingTypedInParamEvent<TEvent> : PendingEventBase where TEvent : struct, IEvent
        {
            private EventSubjectIn<TEvent> Subject;
            private TEvent _event;

            public PendingTypedInParamEvent(EventSubjectIn<TEvent> subject, TEvent ev)
            {
                Subject = subject;
                _event = ev;
            }

            public override void Publish()
            {
                Subject?.Publish(in _event);
            }
        }


    }
}