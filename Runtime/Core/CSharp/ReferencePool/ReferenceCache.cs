/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    /// <summary>
    /// 引用缓存
    /// </summary>
    public sealed class ReferenceCache<TContent> : ReferenceCacheBase<TContent>, IReferenceCache, IReferenceCacheBase, IReferencePoolItem where TContent : class, new()
    {
        public ReferenceCache() : base(){}

        public ReferenceCache(Type referenceType) : base(referenceType){}

        /// <summary>
        /// 获得一个实例，若缓存中没有，则使用new T()创建，typeof(T)需要于ReferenceType一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T Acquire<T>() where T : class, new()
        {
            if (typeof(T) != ReferenceType)
            {
                throw new Log.FetalErrorException(
                    $"Type[ {typeof(T)} ] and ContentType[ {ReferenceType} ] do not match! Can not acquire.", 
                    "ReferencePool", 
                    Log.PBBoxLoggerName);
            }

            T _reference = null;
#if !PB_THREAD_UNSAFE
            lock (m_References)
            {
#endif
                if (m_References.Count > 0)
                {
                    _reference = m_References.Dequeue() as T;
                }
#if !PB_THREAD_UNSAFE
            }
#endif
            _reference = _reference ?? new T();
            UsingCount++;

            if (_reference is IReferencePoolItem __reference)
            {
                __reference.IsUsing = true;
                __reference.OnReferenceAcquire();
            }
            return _reference;
        }

        protected override TContent CreateInstanceFromContentType()
        {
            return (typeof(TContent) == ReferenceType
                ? new TContent()
                : (TContent)Activator.CreateInstance(ReferenceType));
        }

        public void PreCreate(int count)
        {
            lock (m_References)
            {
                if (ReferenceType == typeof(TContent))
                {
                    while (count-- > 0)
                    {
                        m_References.Enqueue(new TContent());
                    }
                }
                else
                {
                    while (count-- > 0)
                    {
                        m_References.Enqueue((TContent)Activator.CreateInstance(ReferenceType));
                    }
                }
            }
        }

        public void PreCreate<T>(int count) where T : class, new()
        {
            if (typeof(T) != ReferenceType)
            {
                throw new Log.FetalErrorException(
                    $"Type[ {typeof(T)} ] and ContentType[ {ReferenceType} ] do not match! Can not create.", 
                    "ReferencePool", 
                    Log.PBBoxLoggerName);
            }
            lock (m_References)
            {
                while (count-- > 0)
                {
                    m_References.Enqueue(new T() as TContent);
                }
            }
        }

    }
}