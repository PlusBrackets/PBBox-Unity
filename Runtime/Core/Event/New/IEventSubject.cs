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
        EventSubscriptionNew Subscribe(Delegate handler, int order = 0);
        void Unsubscribe(Delegate handler, int order = 0);
        void Publish<T>(in T eventData);
        void Publish();
    }
}