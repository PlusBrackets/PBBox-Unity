/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.12.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PBBox.Properties
{
    /// <summary>
    /// 多重锁，用于多个锁同时锁定一个物体时，只有所有锁都解锁时才能解锁物体
    /// </summary>
    public class MultiLock
    {
        private static readonly Lazy<Dictionary<int, MultiLock>> s_GlobalLocks = new Lazy<Dictionary<int, MultiLock>>();

        /// <summary>
        /// 获取全局锁
        /// </summary>
        /// <param name="lockId"></param>
        /// <returns></returns>
        public static MultiLock GetGlobalLock(int lockId)
        {
            if (!s_GlobalLocks.Value.ContainsKey(lockId))
            {
                s_GlobalLocks.Value.Add(lockId, new MultiLock());
            }
            return s_GlobalLocks.Value[lockId];
        }

        public static MultiLock GetGlobalLock(string lockId)
        {
            return GetGlobalLock(lockId.GetHashCode());
        }

        private uint m_LockCount = 0;
        private Lazy<HashSet<int>> m_LockSet = new Lazy<HashSet<int>>();
        public event Action<bool> OnStateChanged;
        public bool IsLocked
        {
            get
            {
                return m_LockCount > 0;
            }
        }

        public void AddLock()
        {
            m_LockCount++;
            if (m_LockCount == 1)
            {
                OnStateChanged?.Invoke(true);
            }
        }

        public void RemoveLock()
        {
            if (m_LockCount == 0)
            {
                Log.Error("Can't remove lock when lock count is 0", "MultiLock", Log.PBBoxLoggerName);
                return;
            }
            m_LockCount--;
            if (m_LockCount == 0)
            {
                OnStateChanged?.Invoke(false);
            }
        }

        public void AddLock(int key)
        {
            if (m_LockSet.Value.Add(key))
            {
                AddLock();
            }
        }

        public void AddLock(string key)
        {
            AddLock(key.GetHashCode());
        }

        public void RemoveLock(int key)
        {
            if (!m_LockSet.IsValueCreated)
            {
                return;
            }
            if (m_LockSet.Value.Remove(key))
            {
                RemoveLock();
            }
        }

        public void RemoveLock(string key)
        {
            RemoveLock(key.GetHashCode());
        }
    }
}