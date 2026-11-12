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
    public sealed partial class EventManager : IEventManager, IEventSubjectManager, ISingletonLifecycle
    {
        private Lazy<Dictionary<Type, TypedSubjectContainer>> _typedSubjects = new Lazy<Dictionary<Type, TypedSubjectContainer>>(() => new Dictionary<Type, TypedSubjectContainer>());
        private Lazy<Dictionary<int, IdSubjectContainer>> _idSubjects = new Lazy<Dictionary<int, IdSubjectContainer>>(() => new Dictionary<int, IdSubjectContainer>());

        private EventSubscriptionNew _updateFrameSubscription;
        private List<PendingEventBase> _pendingEvents;
        private List<PendingEventBase> _pendingEventsTemp;

        public void OnCreateAsSingleton()
        {
            _updateFrameSubscription = UpdateFrameEvent.Subject.Subscribe(Update, 100);
        }

        public void OnDestroyAsSingleton()
        {
            _updateFrameSubscription.Dispose();
        }

        private void Update(in UpdateFrameEvent ev)
        {
            // publish later events;
            if (_pendingEvents != null && _pendingEvents.Count > 0)
            {
                var temp = _pendingEvents;
                _pendingEvents = _pendingEventsTemp;
                _pendingEventsTemp = temp;
                foreach (var pendingEvent in _pendingEventsTemp)
                {
                    pendingEvent.Publish();
                }
                _pendingEventsTemp.Clear();
            }
        }

        private void QueueLaterEvent(PendingEventBase pendingEvent)
        {
            if (_pendingEvents == null)
            {
                _pendingEvents = new List<PendingEventBase>();
            }
            _pendingEvents.Add(pendingEvent);
        }

        public IEventSubject GetSubject<TEvent>(bool isLater = false, bool isInParamHandler = false)
        {
            var type = typeof(TEvent);
            if (_typedSubjects.IsValueCreated && _typedSubjects.Value.TryGetValue(type, out var container))
            {
                if (isInParamHandler)
                {
                    return isLater ? container.InParamSubjectLater : container.InParamSubject;
                }
                else
                {
                    return isLater ? container.DefaultSubjectLater : container.DefaultSubject;
                }
            }
            return null;
        }

        public IEventSubject GetSubject(int eventId, bool isLater = false)
        {
            if (_idSubjects.IsValueCreated && _idSubjects.Value.TryGetValue(eventId, out var container))
            {
                return isLater ? container.SubjectLater : container.Subject;
            }
            return null;
        }


        public EventSubscriptionNew Subscripte<TEvent>(Action<TEvent> handler, int order = 0, bool isLater = false) where TEvent : IEvent
        {
            var type = typeof(TEvent);
            if (!_typedSubjects.Value.TryGetValue(type, out var container))
            {
                container = new TypedSubjectContainer();
                _typedSubjects.Value[type] = container;
            }
            if (isLater)
            {
                var subject = (container.DefaultSubjectLater ?? (container.DefaultSubjectLater = new EventSubject<TEvent>())) as EventSubject<TEvent>;
                return subject.Subscribe(handler, order);
            }
            else
            {
                var subject = (container.DefaultSubject ?? (container.DefaultSubject = new EventSubject<TEvent>())) as EventSubject<TEvent>;
                return subject.Subscribe(handler, order);
            }
            
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler, int order = 0, bool isLater = false) where TEvent : IEvent
        {
            var subject = GetSubject<TEvent>(isLater, false) as EventSubject<TEvent>;
            subject?.Unsubscribe(handler, order);
        }

        public void Publish<TEvent>(TEvent eventData) where TEvent : IEvent
        {
            var type = typeof(TEvent);
            if (!_typedSubjects.Value.TryGetValue(type, out var container))
            {
                return;
            }
            if (container.DefaultSubjectLater != null && container.DefaultSubjectLater is EventSubject<TEvent> laterSubject)
            {
                QueueLaterEvent(new PendingTypedEvent<TEvent>(laterSubject, eventData));
            }
            (container.DefaultSubject as EventSubject<TEvent>)?.Publish(eventData);
        }


        public EventSubscriptionNew Subscripte<TEvent>(ActionIn<TEvent> handler, int order = 0, bool isLater = false) where TEvent : struct, IEvent
        {
            var type = typeof(TEvent);
            if (!_typedSubjects.Value.TryGetValue(type, out var container))
            {
                container = new TypedSubjectContainer();
                _typedSubjects.Value[type] = container;
            }
            if (isLater)
            {
                var subject = (container.InParamSubjectLater ?? (container.InParamSubjectLater = new EventSubjectIn<TEvent>())) as EventSubjectIn<TEvent>;
                return subject.Subscribe(handler, order);
            }
            else
            {
                var subject = (container.InParamSubject ?? (container.InParamSubject = new EventSubjectIn<TEvent>())) as EventSubjectIn<TEvent>;
                return subject.Subscribe(handler, order);
            }
        }

        public void Unsubscribe<TEvent>(ActionIn<TEvent> handler, int order = 0, bool isLater = false) where TEvent : struct, IEvent
        {
            var subject = GetSubject<TEvent>(isLater, true) as EventSubjectIn<TEvent>;
            subject?.Unsubscribe(handler, order);
        }

        public void Publish<TEvent>(in TEvent eventData) where TEvent : struct, IEvent
        {
            var type = typeof(TEvent);
            if (!_typedSubjects.Value.TryGetValue(type, out var container))
            {
                return;
            }
            if (container.InParamSubjectLater != null && container.InParamSubjectLater is EventSubjectIn<TEvent> laterSubject)
            {
                QueueLaterEvent(new PendingTypedInParamEvent<TEvent>(laterSubject, eventData));
            }
            (container.InParamSubject as EventSubjectIn<TEvent>)?.Publish(in eventData);
        }

        public EventSubscriptionNew SubscripteGeneric(int eventId, Delegate handler, int order = 0, bool isLater = false)
        {
            if (!_idSubjects.Value.TryGetValue(eventId, out var container))
            {
                container = new IdSubjectContainer();
                _idSubjects.Value[eventId] = container;
            }
            if (isLater)
            {
                IEventSubject subject = container.SubjectLater ?? (container.SubjectLater = new EventSubject());
                return subject.Subscribe(handler, order);
            }
            else
            {
                IEventSubject subject = container.Subject ?? (container.Subject = new EventSubject());
                return subject.Subscribe(handler, order);
            }
        }

        public void UnsubscribeGeneric(int eventId, Delegate handler, int order = 0, bool isLater = false)
        {
            var subject = GetSubject(eventId, isLater);
            subject?.Unsubscribe(handler, order);
        }

        public void Publish<TData>(int eventId, TData eventData)
        {
            if (!_idSubjects.Value.TryGetValue(eventId, out var container))
            {
                return;
            }
            if (container.SubjectLater != null)
            {
                QueueLaterEvent(new PendingIdEvent<TData>(container.SubjectLater, eventId, eventData, false));
            }
            container.Subject?.Publish(eventData);
        }

        public void Publish<TData>(int eventId, in TData eventData) where TData : struct
        {
            if(!_idSubjects.Value.TryGetValue(eventId, out var container))
            {
                return;
            }
            if (container.SubjectLater != null)
            {
                QueueLaterEvent(new PendingIdEvent<TData>(container.SubjectLater, eventId, eventData, true));
            }
            container.Subject?.Publish(in eventData);
        }

        public void Publish(int eventId)
        {
            if (!_idSubjects.Value.TryGetValue(eventId, out var container))
            {
                return;
            }
            if (container.SubjectLater != null)
            {
                QueueLaterEvent(new PendingIdEvent(container.SubjectLater, eventId));
            }
            container.Subject?.Publish();
        }

    }
}