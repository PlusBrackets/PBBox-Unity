/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.24
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    /// <summary>
    /// 订阅事件的返回值，Disposse方法可以取消订阅
    /// </summary>
    public struct EventSubscription : IDisposable
    {
        private readonly ISubscriptionHandler m_Handler;
        private readonly int m_EventId;
        private readonly Delegate m_Listener;
        private readonly int m_Order;
        private readonly bool m_IsLateEvent;

        public int EventId => m_EventId;

        internal EventSubscription(ISubscriptionHandler handler, int eventId, Delegate listener, int order, bool isLateEvent)
        {
            m_Handler = handler;
            m_EventId = eventId;
            m_Listener = listener;
            m_Order = order;
            m_IsLateEvent = isLateEvent;
        }

        public void Dispose()
        {
            if (m_Handler != null)
            {
                m_Handler.Unsubscripe(m_EventId, m_Listener, m_Order, m_IsLateEvent);
            }
        }
    }
}