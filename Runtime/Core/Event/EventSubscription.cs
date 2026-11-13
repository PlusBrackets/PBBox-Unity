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
    public struct EventSubscription : IDisposable
    {
        public static readonly EventSubscription Empty = new EventSubscription(null, null, 0);

        private IEventSubject _subject;
        private Delegate _handler;
        private readonly int _order;
        /// <summary>
        /// 该订阅结构体是否有效
        /// </summary>
        public bool IsValid => _subject != null && _handler != null;

        public EventSubscription(IEventSubject subject, Delegate handler, int order)
        {
            _subject = subject;
            _handler = handler;
            _order = order;
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        public void Dispose()
        {
            _subject?.Unsubscribe(_handler, _order);
            _subject = null;
            _handler = null;
            // Implement disposal logic here
        }
    }
}