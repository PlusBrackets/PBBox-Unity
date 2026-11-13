/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox.Event
{
    public interface IEventSubject
    {
        EventSubscription Subscribe(Delegate handler, int order = 0);
        void Unsubscribe(Delegate handler, int order = 0);
        void Publish();
        void Publish<T>(T eventData);
        void Publish<T>(in T eventData);
    }

//    public interface IEventSubject<TEvent> : IEventSubject
//    {
//        EventSubscription Subscribe(Action<TEvent> handler, int order = 0);
//        void Unsubscribe(Action<TEvent> handler, int order = 0);
//        void Publish(TEvent eventData);
//    }
//
//    public interface IEventSubjectIn<TEvent> : IEventSubject
//    {
//        EventSubscription Subscribe(ActionIn<TEvent> handler, int order = 0);
//        void Unsubscribe(ActionIn<TEvent> handler, int order = 0);
//        void Publish(in TEvent eventData);
//    }
}