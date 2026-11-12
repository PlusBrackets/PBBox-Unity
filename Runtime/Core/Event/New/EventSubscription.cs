/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using PBBox.Event;

namespace PBBox
{
    /// <summary>
    /// 事件订阅结构体，可以用于管理事件订阅的生命周期
    /// </summary>
    public struct EventSubscriptionNew : IDisposable
    {
        public static readonly EventSubscriptionNew Empty = new EventSubscriptionNew(null, null, 0);

        private IEventSubject _subject;
        private Delegate _handler;
        private readonly int _order;
        public bool IsValid => _subject != null && _handler != null;

        public EventSubscriptionNew(IEventSubject subject, Delegate handler, int order)
        {
            _subject = subject;
            _handler = handler;
            _order = order;
        }

        public void Dispose()
        {
            _subject?.Unsubscribe(_handler, _order);
            _subject = null;
            _handler = null;
            // Implement disposal logic here
        }
    }
}