/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;

namespace PBBox
{

    /// <summary>
    /// 简单得游戏数据暂存，方便做一些通用的数据储存，尽量避免存入大量且需要频繁访问的数据
    /// </summary>
    public class SimpleDataCache : SingleClass<SimpleDataCache>
    {
        private Dictionary<string, Dictionary<string, object>> m_Caches;
        public const string DefaultCacheKey = "___DefautCache";

        public bool HasCache(string cacheKey)
        {
            if (m_Caches == null)
            {
                return false;
            }
            else
            {
                return m_Caches.ContainsKey(cacheKey);
            }
        }

        public Dictionary<string, object> GetCache(string cacheKey, bool autoCreate = false)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                cacheKey = DefaultCacheKey;
            }
            if (HasCache(cacheKey))
            {
                return m_Caches[cacheKey];
            }
            if (autoCreate)
            {
                if (m_Caches == null)
                {
                    m_Caches = new Dictionary<string, Dictionary<string, object>>();
                }
                Dictionary<string, object> cache = new Dictionary<string, object>();
                m_Caches.Add(cacheKey, cache);
                return cache;
            }
            return null;
        }

        public void ClearCache(string cacheKey = null)
        {
            if (m_Caches == null)
            {
                return;
            }
            var cache = GetCache(cacheKey);
            if (cache != null)
            {
                cache.Clear();
            }
        }

        public void ClearAllCaches()
        {
            if (m_Caches == null)
            {
                return;
            }
            else
            {
                m_Caches.Clear();
            }
        }

        public bool HasData(string dataKey)
        {
            return HasData(DefaultCacheKey, dataKey);
        }

        public bool HasData(string cacheKey, string dataKey)
        {
            var cache = GetCache(cacheKey);
            if (cache != null)
            {
                return cache.ContainsKey(dataKey);
            }
            return false;
        }

        public void SetData(string dataKey, object data)
        {
            SetData(DefaultCacheKey, dataKey);
        }

        public void SetData(string cacheKey, string dataKey, object data)
        {
            var cache = GetCache(cacheKey, true);
            if (cache.ContainsKey(dataKey))
            {
                cache[dataKey] = data;
            }
            else
            {
                cache.Add(dataKey, data);
            }
        }

        public T GetData<T>(string dataKey, T defaultData = default(T))
        {
            return GetData(DefaultCacheKey, dataKey, defaultData);
        }

        public T GetData<T>(string cacheKey, string dataKey, T defaultData = default(T))
        {
            var cache = GetCache(cacheKey);
            object temp;
            if (cache != null && cache.TryGetValue(dataKey, out temp))
            {
                return (T)temp;
            }
            return defaultData;
        }

        public void RemoveData(string dataKey)
        {
            RemoveData(DefaultCacheKey, dataKey);
        }

        public void RemoveData(string cacheKey, string dataKey)
        {
            var cache = GetCache(cacheKey);
            if (cache != null)
            {
                cache.Remove(dataKey);
            }
        }

    }

}