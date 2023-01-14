/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;

namespace PBBox
{
    public abstract class ReferenceCacheBase<TContent> : IReferenceCacheBase, IReferencePoolItem where TContent : class
    {
        protected Queue<TContent> m_References;
        public Type ReferenceType { get; protected set; }
        public int UsingCount { get; protected set; } = 0;
        public int CachedCount => m_References.Count;
        bool IReferencePoolItem.IsUsing { get; set; } = true;

        public ReferenceCacheBase() : this(typeof(TContent)) { }

        public ReferenceCacheBase(Type referenceType)
        {
            ReferenceType = referenceType;
            m_References = new Queue<TContent>();
        }

        protected abstract TContent CreateInstanceFromContentType();

        protected void TryCallPoolItemAcquire(TContent reference)
        {
            if (reference is IReferencePoolItem _reference)
            {
                _reference.IsUsing = true;
                _reference.OnReferenceAcquire();
            }
        }

        protected void TryCallPoolItemRelease(TContent reference){
            if (reference is IReferencePoolItem _reference)
            {
                _reference.IsUsing = false;
                _reference.OnReferenceRelease();
            }
        }

        object IReferenceCacheBase.Acquire()
        {
            TContent _reference = null;
#if !PB_THREAD_UNSAFE
            //加锁会消耗性能，使用宏定义判定是否需要保持线程安全
            lock (m_References)
            {
#endif
                if (m_References.Count > 0)
                {
                    _reference = m_References.Dequeue();
                }
#if !PB_THREAD_UNSAFE
            }
#endif
            _reference = _reference ?? CreateInstanceFromContentType();
            if (_reference == null)
            {
                return null;
            }
            UsingCount++;
            TryCallPoolItemAcquire(_reference);
            return _reference;
        }

        public TContent Acquire() => (TContent)((IReferenceCacheBase)this).Acquire();

        public void Release(TContent reference)
        {
            if (reference.GetType() != ReferenceType)
            {
                throw new Log.FetalErrorException(
                    $"Type[ {reference.GetType()} ] and ContentType[ {ReferenceType} ] do not match! Can not release.",
                    "ReferencePool",
                    Log.PBBoxLoggerName);
            }
            TryCallPoolItemRelease(reference);
#if !PB_THREAD_UNSAFE
            lock (m_References)
            {
#endif
                if (ReferencePool.EnableCheckMode && m_References.Contains(reference))
                {
                    Log.Error($"The reference of Type[ {ReferenceType} ] has been released!", "ReferencePool", Log.PBBoxLoggerName);
                    return;
                }
            m_References.Enqueue(reference);
#if !PB_THREAD_UNSAFE
            }
#endif
            UsingCount--;
        }

        void IReferenceCacheBase.Release(object reference)
        {
            if (reference is TContent _reference)
            {
                this.Release(_reference);
            }
            else
            {
                throw new Log.FetalErrorException(
                    $"Type[ {reference.GetType()} ] and ContentType[ {ReferenceType} ] do not match! Can not release.",
                    "ReferencePool",
                    Log.PBBoxLoggerName);
            }
        }

        public virtual void Remove(int count)
        {
            lock (m_References)
            {
                count = Math.Clamp(count, 0, m_References.Count);
                while (count-- > 0)
                {
                    m_References.Dequeue();
                }
            }
        }

        public virtual void Clear()
        {
            lock (m_References)
            {
                m_References.Clear();
            }
        }

        public virtual void TrimTo(int size)
        {
            Remove(m_References.Count - size);
        }

        void IReferencePoolItem.OnReferenceAcquire()
        {
            OnReferenceAcquireImpl();
        }

        void IReferencePoolItem.OnReferenceRelease()
        {
            OnReferenceReleaseImpl();
        }

        protected virtual void OnReferenceAcquireImpl() { }

        protected virtual void OnReferenceReleaseImpl()
        {
            Clear();
        }
    }
}