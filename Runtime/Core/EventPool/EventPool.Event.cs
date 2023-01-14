/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    public sealed partial class EventPool<TKey>
    {
        private sealed class Event : IReferencePoolItem
        {
            bool IReferencePoolItem.IsUsing { get; set; } = true;
            public TKey EventId { get; private set; }
            public object Sender { get; private set; }
            private Action m_TriggerAction = null;

            internal static Event Create(TKey eventId, object sender)
            {
                var _event = ReferencePool.Acquire<Event>();
                _event.EventId = eventId;
                _event.Sender = sender;
                return _event;
            }

            public void Release()
            {
                ReferencePool.Release(this);
            }

            public void SetTriggerAction(Action action)
            {
                m_TriggerAction = action;
            }

            public void Emit()
            {
                m_TriggerAction?.Invoke();
            }

            void IReferencePoolItem.OnReferenceAcquire()
            {

            }

            void IReferencePoolItem.OnReferenceRelease()
            {
                m_TriggerAction = null;
                Sender = null;
                EventId = default(TKey);
            }
        }
    }
}