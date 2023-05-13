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
    public abstract class EventArgsBase : IReferencePoolItem
    {
        bool IReferencePoolItem.IsUsing { get; set; } = true;
        public bool IsUsing => ((IReferencePoolItem)this).IsUsing;

        void IReferencePoolItem.OnReferenceAcquire()
        {
            OnReferenceAcquireImpl();
        }

        void IReferencePoolItem.OnReferenceRelease()
        {
            OnReferenceReleaseImpl();
        }

        protected virtual void OnReferenceAcquireImpl() { }

        protected virtual void OnReferenceReleaseImpl() { }
    }
}