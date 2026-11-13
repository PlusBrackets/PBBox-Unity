/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using NUnit.Framework;
using PBBox.Event;

namespace PBBox
{
    /// <summary>
    /// <para>默认事件管理器，用于管理和调度各种事件的订阅和发布</para>
    /// <para>支持按类型和按ID的事件订阅与发布，提供延迟发布机制</para>
    /// <para>有较好且稳定的订阅和取消订阅性能，发布遍历性能比多播委托稍慢(10000+事件时为多播委托的2倍左右)</para>
    /// </summary>
    [SingletonPriority(-1)]
    public sealed partial class EventManager : IEventManager, IEventSubjectManager, ISingletonLifecycle
    {
        private Lazy<Dictionary<Type, TypedSubjectContainer>> _typedSubjects = new Lazy<Dictionary<Type, TypedSubjectContainer>>(() => new Dictionary<Type, TypedSubjectContainer>());
        private Lazy<Dictionary<int, IdSubjectContainer>> _idSubjects = new Lazy<Dictionary<int, IdSubjectContainer>>(() => new Dictionary<int, IdSubjectContainer>());

        private EventSubscription _updateFrameSubscription;
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

        public IEventSubject GetOrCreateSubject<TEvent>(bool isLater = false) where TEvent : IEvent
        {
            var type = typeof(TEvent);
            if (!_typedSubjects.Value.TryGetValue(type, out var container))
            {
                container = new TypedSubjectContainer();
                _typedSubjects.Value[type] = container;
            }
            return container.GetOrCreateSubject<TEvent>(isLater);
        }

        public IEventSubject GetOrCreateSubject(int eventId, bool isLater = false)
        {
            if (!_idSubjects.Value.TryGetValue(eventId, out var container))
            {
                container = new IdSubjectContainer();
                _idSubjects.Value[eventId] = container;
            }
            return container.GetOrCreateSubject(isLater);
        }

        public IEventSubject GetSubject<TEvent>(bool isLater = false) where TEvent : IEvent
        {
            var type = typeof(TEvent);
            if (_typedSubjects.IsValueCreated && _typedSubjects.Value.TryGetValue(type, out var container))
            {
                return isLater ? container.DefaultSubjectLater : container.DefaultSubject;
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

        #region Typed Event Methods

        public EventSubscription Subscribe<TEvent>(Action<TEvent> handler, int order = 0, bool isLater = false) where TEvent : IEvent
        {
            var subject = GetOrCreateSubject<TEvent>(isLater) as EventSubject<TEvent>;
            return subject.Subscribe(handler, order);
            
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler, int order = 0, bool isLater = false) where TEvent : IEvent
        {
            var subject = GetSubject<TEvent>(isLater) as EventSubject<TEvent>;
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
                QueueLaterEvent(new PendingTypedEvent<TEvent>(laterSubject, eventData, false));
            }
            (container.DefaultSubject as EventSubject<TEvent>)?.Publish(eventData);
        }

        #endregion

        #region Typed Event Methods with In-Parameter

        public EventSubscription Subscribe<TEvent>(ActionIn<TEvent> handler, int order = 0, bool isLater = false) where TEvent : struct, IEvent
        {
            var subject = GetOrCreateSubject<TEvent>(isLater) as EventSubject<TEvent>;
            return subject.Subscribe(handler, order);
        }

        public void Unsubscribe<TEvent>(ActionIn<TEvent> handler, int order = 0, bool isLater = false) where TEvent : struct, IEvent
        {
            var subject = GetSubject<TEvent>(isLater) as EventSubject<TEvent>;
            subject?.Unsubscribe(handler, order);
        }

        public void Publish<TEvent>(in TEvent eventData) where TEvent : struct, IEvent
        {
            var type = typeof(TEvent);
            if (!_typedSubjects.Value.TryGetValue(type, out var container))
            {
                return;
            }
            if (container.DefaultSubjectLater != null && container.DefaultSubjectLater is EventSubject<TEvent> laterSubject)
            {
                QueueLaterEvent(new PendingTypedEvent<TEvent>(laterSubject, eventData, true));
            }
            (container.DefaultSubject as EventSubject<TEvent>)?.Publish(in eventData);
        }
        #endregion

        #region Generic Event Methods

        public EventSubscription SubscribeGeneric(int eventId, Delegate handler, int order = 0, bool isLater = false)
        {
            var subject = GetOrCreateSubject(eventId, isLater);
            return subject.Subscribe(handler, order);
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

        #endregion

    }
}