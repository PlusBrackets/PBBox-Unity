/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox
{
    /// <summary>
    /// 事件参数基类
    /// </summary>
    public abstract class EventArgsBase : IEventArgs, IReferencePoolItem
    {
        /// <summary>
        /// 事件状态
        /// </summary>
        /// <returns></returns>
        public EventArgsState EventState => ((IEventArgs)this).EventState;
        EventArgsState IEventArgs.EventState { get; set; }
        protected virtual bool IsAutoRelease { get; } = true;
        bool IReferencePoolItem.IsUsing { get; set; } = true;
        public bool IsUsing => ((IReferencePoolItem)this).IsUsing;

        void IReferencePoolItem.OnReferenceAcquire()
        {
            ((IEventArgs)this).EventState = EventArgsState.Unused;
            OnReferenceAcquireImpl();
        }

        void IReferencePoolItem.OnReferenceRelease()
        {
            OnReferenceReleaseImpl();
        }

        void IEventArgs.Release()
        {
            if (IsAutoRelease && IsUsing)
            {
                ReleaseImpl();
            }
        }

        public void Release()
        {
            if (IsUsing)
            {
                ReleaseImpl();
            }
        }

        protected virtual void OnReferenceAcquireImpl() { }

        protected virtual void OnReferenceReleaseImpl() { }

        protected virtual void ReleaseImpl()
        {
            ReferencePool.Release(this);
        }
    }
}