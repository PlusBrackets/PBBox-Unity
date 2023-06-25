using System.Net.NetworkInformation;
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
        /// <summary>
        /// 队列事件，带有一个泛型表示传入参数
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        private class Event<TEventArgs> : Event
        {
            public TEventArgs EventArgs { get; set; }
            public IReferenceCacheBase EventArgsReferencePool { get; set; } = null;

            protected override void OnReferenceReleaseImpl()
            {
                if (EventArgsReferencePool != null)
                {
                    if (!(EventArgsReferencePool is IReferencePoolItem _poolItem) || _poolItem.IsUsing)
                    {
                        EventArgsReferencePool.Release(EventArgs);
                    }
                }
                EventArgsReferencePool = null;
                EventArgs = default(TEventArgs);
            }

            public override bool Trigger(Delegate listener)
            {
                if (listener is Action<object, TEventArgs> _handler)
                {
                    _handler(Sender, EventArgs);
                }
                else if (listener is Func<object, TEventArgs, bool> _handler2)
                {
                    return _handler2(Sender, EventArgs);
                }
                else
                {
                    return base.Trigger(listener);
                }
                return true;
            }

            protected override void CanNotTriggerFallback(Delegate listener)
            {
                Log.Warning(
                    "Type mismatch, please check if the passed-in args and event handler match."
                    + $"\n( {EventId},  {typeof(TEventArgs)},  {listener.GetType()} )\n",
                    "EventPool",
                    Log.PBBoxLoggerName);
            }
        }

        /// <summary>
        /// 队列事件
        /// </summary>
        private class Event : IReferencePoolItem
        {
            bool IReferencePoolItem.IsUsing { get; set; } = true;
            public TKey EventId { get; set; }
            public object Sender { get; set; }

            public void Release()
            {
                ReferencePool.Release(this);
            }

            void IReferencePoolItem.OnReferenceAcquire() { }

            void IReferencePoolItem.OnReferenceRelease()
            {
                OnReferenceReleaseImpl();
                Sender = null;
                EventId = default(TKey);
            }

            protected virtual void OnReferenceReleaseImpl(){}

            public virtual bool Trigger(Delegate listener)
            {
                if (listener is Action<object> _handler)
                {
                    _handler(Sender);
                }
                else if (listener is Func<object, bool> _handler2)
                {
                    return _handler2(Sender);
                }
                else
                {
                    CanNotTriggerFallback(listener);
                }
                return true;
            }

            protected virtual void CanNotTriggerFallback(Delegate listener)
            {
                Log.Warning(
                    "Type mismatch, please check if the passed-in args and event handler match."
                    + $"\n( {EventId},  No args,  {listener.GetType()} )\n",
                    "EventPool",
                    Log.PBBoxLoggerName);
            }
        }
    }
}