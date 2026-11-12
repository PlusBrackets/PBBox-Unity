/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using PBBox.Event;

namespace PBBox
{
    public delegate void EventHandler<T>(in T eventData);

    public interface IEventManager : ISingleton<IEventManager>
    {
        EventSubscriptionNew Subscripte<T>(Action<T> handler, int order = 0, bool isLater = false);
        void Unsubscribe<T>(Action<T> handler, int order = 0, bool isLater = false);
        void Publish<T>(T eventData);

        EventSubscriptionNew Subscripte<T>(EventHandler<T> handler, int order = 0, bool isLater = false);
        void Unsubscribe<T>(EventHandler<T> handler, int order = 0, bool isLater = false);
        void Publish<T>(in T eventData);

        EventSubscriptionNew Subscripte(int eventId, Delegate handler, int order = 0, bool isLater = false);
        void Unsubscribe(int eventId, Delegate handler, int order = 0, bool isLater = false);
        void Publish<T>(int eventId, T eventData);
        void Publish<T>(int eventId, in T eventData);
        void Publish(int eventId);
    }
}