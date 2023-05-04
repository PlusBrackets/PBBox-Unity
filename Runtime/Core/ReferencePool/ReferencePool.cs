/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;

namespace PBBox
{
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
            var _type = reference.GetType();
            CheckTypeLegality(_type);
            GetCache(_type).Release(reference);
        }

        public static IReferenceCache GetCache<T>() where T : class, new()
        {
            IReferenceCache _collection = null;
            var _type = typeof(T);

#if !PB_THREAD_UNSAFE
            lock (s_Caches)
            {
#endif
                if (!s_Caches.TryGetValue(_type, out _collection))
                {
                    _collection = new ReferenceCache<T>();
                    s_Caches.Add(_type, _collection);
                }

#if !PB_THREAD_UNSAFE
            }
#endif
            return _collection;
        }

        public static IReferenceCache GetCache(Type type)
        {
            IReferenceCache _collection = null;

#if !PB_THREAD_UNSAFE
            lock (s_Caches)
            {
#endif
                if (!s_Caches.TryGetValue(type, out _collection))
                {
                    _collection = new ReferenceCache<object>(type);
                    s_Caches.Add(type, _collection);
                }

#if !PB_THREAD_UNSAFE
            }
#endif
            return _collection;
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
                if (s_Caches.TryGetValue(referenceType, out var _collection))
                {
                    _collection.Remove(count);
                }
            }
        }

        public static void TrimTo(Type referenceType, int size)
        {
            lock (s_Caches)
            {
                if (s_Caches.TryGetValue(referenceType, out var _collection))
                {
                    _collection.Remove(_collection.CachedCount - size);
                }
            }
        }

        public static void Clear(Type referenceType)
        {
            lock (s_Caches)
            {
                if (s_Caches.TryGetValue(referenceType, out var _collection))
                {
                    _collection.Clear();
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