/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;

namespace PBBox
{
    //TODO 优化-可以单独实例化一个ReferencePool，进行模块化管理
    /// <summary>
    /// 引用池
    /// </summary>
    public static partial class ReferencePool
    {
        private static Dictionary<Type, IReferenceCache> s_Caches = new Dictionary<Type, IReferenceCache>();
        /// <summary>
        /// 开启检查模式，会消耗额外的性能保证类型准确性
        /// </summary>
        /// <value></value>
        public static bool EnableCheckMode { get; set; } = false;

        public static T Acquire<T>() where T : class, new()
        {
            return GetCache<T>().Acquire<T>();
        }

        public static object Acquire(Type referenceType)
        {
            CheckTypeLegality(referenceType);
            return GetCache(referenceType).Acquire();
        }

        public static void Release(object reference)
        {
            if (reference == null)
            {
                return;
            }
            var type = reference.GetType();
            CheckTypeLegality(type);
            GetCache(type).Release(reference);
        }

        public static IReferenceCache GetCache<T>() where T : class, new()
        {
            IReferenceCache collection = null;
            var type = typeof(T);

#if PB_THREAD_SAFE
            lock (s_Caches)
            {
#endif
                if (!s_Caches.TryGetValue(type, out collection))
                {
                    collection = new ReferenceCache<T>();
                    s_Caches.Add(type, collection);
                }

#if PB_THREAD_SAFE
            }
#endif
            return collection;
        }

        public static IReferenceCache GetCache(Type type)
        {
            IReferenceCache cache = null;

#if PB_THREAD_SAFE
            lock (s_Caches)
            {
#endif
                if (!s_Caches.TryGetValue(type, out cache))
                {
                    cache = new ReferenceCache<object>(type);
                    s_Caches.Add(type, cache);
                }

#if PB_THREAD_SAFE
            }
#endif
            return cache;
        }

        public static void PreCreate<T>(int count) where T : class, new()
        {
            GetCache<T>().PreCreate<T>(count);
        }

        public static void PreCreate(Type referenceType, int count)
        {
            CheckTypeLegality(referenceType);
            GetCache(referenceType).PreCreate(count);
        }

        public static void Remove(Type referenceType, int count)
        {
            lock (s_Caches)
            {
                if (s_Caches.TryGetValue(referenceType, out var cache))
                {
                    cache.Remove(count);
                }
            }
        }

        public static void TrimTo(Type referenceType, int size)
        {
            lock (s_Caches)
            {
                if (s_Caches.TryGetValue(referenceType, out var cache))
                {
                    cache.Remove(cache.CachedCount - size);
                }
            }
        }

        public static void Clear(Type referenceType)
        {
            lock (s_Caches)
            {
                if (s_Caches.TryGetValue(referenceType, out var cache))
                {
                    cache.Clear();
                }
            }
        }

        public static void ClearAll()
        {
            lock (s_Caches)
            {
                foreach (var kvp in s_Caches)
                {
                    kvp.Value.Clear();
                }
                s_Caches.Clear();
            }
        }

        private static void CheckTypeLegality(Type type)
        {
            if (!EnableCheckMode)
            {
                return;
            }
            if (type == null)
            {
                throw new Log.FetalErrorException("Type is invild.", "ReferencePool", Log.PBBoxLoggerName);
            }
            if (!type.IsClass || type.IsAbstract)
            {
                throw new Log.FetalErrorException($"Type[ {type} ] is not class.", "ReferencePool", Log.PBBoxLoggerName);
            }
            // if (!typeof(IReferencePoolCallback).IsAssignableFrom(type))
            // {
            //     throw new Log.FetalErrorException($"Type [{type.FullName}] must implement interface: IReference", "ReferencePool", "PBBox");
            // }
        }
    }
}