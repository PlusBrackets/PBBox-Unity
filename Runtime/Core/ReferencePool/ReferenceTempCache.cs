/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    /// <summary>
    /// 引用临时缓存，缓存内没有实例时不能创建实例，需要由外部放置。
    /// </summary>
    public sealed class ReferenceTempCache<TContent> : ReferenceCacheBase<TContent>, IReferenceCacheBase, IReferencePoolItem where TContent : class
    {
        public ReferenceTempCache() : base() { }

        private ReferenceTempCache(Type referenceType) : base(referenceType) { }

        public T Acquire<T>() where T : class, TContent
        {
            if (typeof(T) != ReferenceType)
            {
                throw new Log.FetalErrorException(
                    $"Type[ {typeof(T)} ] and ContentType[ {ReferenceType} ] do not match! Can not acquire.",
                    "ReferencePool",
                    Log.PBBoxLoggerName);
            }
            return CachedCount > 0 ? Acquire() as T : null;
        }

        public bool TryAcquire<T>(out T value) where T : class, TContent
        {
            value = CachedCount > 0 ? Acquire<T>() : null;
            return value != null;
        }

        public bool TryAcquire(out TContent value)
        {
            value = CachedCount > 0 ? Acquire() : null;
            return value != null;
        }

        protected override TContent CreateInstanceFromContentType()
        {
            return null;
        }
    }
}