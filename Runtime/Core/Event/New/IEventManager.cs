/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    public delegate void ActionIn<T>(in T eventData);

    public interface IEventManager : ISingleton<IEventManager>
    {
        EventSubscription Subscribe<TEvent>(Action<TEvent> handler, int order = 0, bool isLater = false) where TEvent : IEvent;
        void Unsubscribe<TEvent>(Action<TEvent> handler, int order = 0, bool isLater = false) where TEvent : IEvent;
        void Publish<TEvent>(TEvent eventData) where TEvent : IEvent;

        EventSubscription Subscribe<TEvent>(ActionIn<TEvent> handler, int order = 0, bool isLater = false) where TEvent : struct, IEvent;
        void Unsubscribe<TEvent>(ActionIn<TEvent> handler, int order = 0, bool isLater = false) where TEvent : struct, IEvent;
        void Publish<TEvent>(in TEvent eventData) where TEvent : struct, IEvent;

        EventSubscription SubscribeGeneric(int eventId, Delegate handler, int order = 0, bool isLater = false);
        void UnsubscribeGeneric(int eventId, Delegate handler, int order = 0, bool isLater = false);
        void Publish<TData>(int eventId, TData eventData);
        void Publish<TData>(int eventId, in TData eventData) where TData : struct;
        void Publish(int eventId);


        EventSubscription Subscribe<TData>(int eventId, Action<TData> handler, int order = 0, bool isLater = false) => SubscribeGeneric(eventId, handler, order, isLater);
        EventSubscription Subscribe<TData>(int eventId, ActionIn<TData> handler, int order = 0, bool isLater = false) where TData : struct => SubscribeGeneric(eventId, handler, order, isLater);
        EventSubscription Subscribe<TData>(int eventId, Action handler, int order = 0, bool isLater = false) => SubscribeGeneric(eventId, handler, order, isLater);
        void Unsubscribe<TData>(int eventId, Action<TData> handler, int order = 0, bool isLater = false) => UnsubscribeGeneric(eventId, handler, order, isLater);
        void Unsubscribe<TData>(int eventId, ActionIn<TData> handler, int order = 0, bool isLater = false) where TData : struct => UnsubscribeGeneric(eventId, handler, order, isLater);
        void Unsubscribe(int eventId, Action handler, int order = 0, bool isLater = false) => UnsubscribeGeneric(eventId, handler, order, isLater);
    }
}