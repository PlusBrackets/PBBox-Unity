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
    public sealed class EventManager : IEventManager, ISingletonLifecycle
    {
        private Lazy<Dictionary<Type, TypeSubjectContainer>> _defaultSubjects = new Lazy<Dictionary<Type, TypeSubjectContainer>>(() => new Dictionary<Type, TypeSubjectContainer>());
        private Lazy<Dictionary<int, IEventSubject>> _eventIdSubjects = new Lazy<Dictionary<int, IEventSubject>>(() => new Dictionary<int, IEventSubject>());
        private Lazy<Dictionary<int, IEventSubject>> _eventIdSubjectsLater = new Lazy<Dictionary<int, IEventSubject>>(() => new Dictionary<int, IEventSubject>());

        private class TypeSubjectContainer
        {
            public IEventSubject DefaultSubject;
            public IEventSubject DefaultSubjectLater;
            public IEventSubject InParamSubject;
            public IEventSubject InParamSubjectLater;
        }

        private EventSubscriptionNew _updateFrameSubscription;

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
        }

        public EventSubscriptionNew Subscripte<T>(Action<T> handler, int order = 0, bool isLater = false)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T>(Action<T> handler, int order = 0, bool isLater = false)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(T eventData)
        {
            throw new NotImplementedException();
        }

        public EventSubscriptionNew Subscripte<T>(EventHandler<T> handler, int order = 0, bool isLater = false)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T>(EventHandler<T> handler, int order = 0, bool isLater = false)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(in T eventData)
        {
            throw new NotImplementedException();
        }

        public EventSubscriptionNew Subscripte(int eventId, Delegate handler, int order = 0, bool isLater = false)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(int eventId, Delegate handler, int order = 0, bool isLater = false)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(int eventId, T eventData)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(int eventId, in T eventData)
        {
            throw new NotImplementedException();
        }

        public void Publish(int eventId)
        {
            throw new NotImplementedException();
        }
    }
}