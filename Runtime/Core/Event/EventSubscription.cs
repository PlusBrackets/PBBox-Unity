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
    /// <para>事件订阅结构体，可以用于管理事件订阅的生命周期</para>
    /// <para>长期持有时要注意避免强引用导致的内存泄漏，可以通过AsWeak方法将目标IEventSubject的引用转换为弱引用</para>
    /// </summary>
    public struct EventSubscription : IDisposable
    {
        public static readonly EventSubscription Empty = new EventSubscription(null, null, 0);

        private IEventSubject _subject;
        private WeakReference<IEventSubject> _weakSubject;
        private Delegate _handler;
        private readonly int _order;
        /// <summary>
        /// 该订阅结构体是否有效
        /// </summary>
        public bool IsValid => _handler != null && (_subject != null || (_weakSubject != null));

        public EventSubscription(IEventSubject subject, Delegate handler, int order)
        {
            _subject = subject;
            _handler = handler;
            _order = order;
            _weakSubject = null;
        }

        /// <summary>
        /// 将持有的事件主题引用转换为弱引用，避免强引用subject导致的内存泄漏
        /// </summary>
        /// <returns></returns>
        public EventSubscription AsWeak()
        {
            if (_weakSubject == null && _subject != null)
            {
                _weakSubject = new WeakReference<IEventSubject>(_subject);
                _subject = null;
            }
            return this;
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        public void Dispose()
        {
            if (_weakSubject != null && _weakSubject.TryGetTarget(out var weakSubject))
            {
                weakSubject?.Unsubscribe(_handler, _order);
            }
            else
            {
                _subject?.Unsubscribe(_handler, _order);
            }
            _subject = null;
            _weakSubject = null;
            _handler = null;
            // Implement disposal logic here
        }
    }
}