using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Properties
{

    [System.Serializable]
    /// <summary>
    /// 键值锁
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KeyLock<T>
    {   
        [SerializeField]
        private List<T> locks => _locks.Value;
        private Lazy<List<T>> _locks = new Lazy<List<T>>();
        /// <summary>
        /// 是否解锁
        /// </summary>
        public bool Unlock
        {
            get
            {
                return locks.Count == 0;
            }
        }
        /// <summary>
        /// 锁的数量
        /// </summary>
        public int Count
        {
            get
            {
                return locks.Count;
            }
        }

        /// <summary>
        /// 当Unlock状态改变时调用，锁定时返回false，解锁时返回true
        /// </summary>
        public event UnityEngine.Events.UnityAction<bool> onUnlockStateChanged;

        /// <summary>
        /// 增加一个锁
        /// </summary>
        /// <param name="key"></param>
        public void AddLock(T key)
        {
            bool unlock = Unlock;
            if (!locks.Contains(key))
            {
                locks.Add(key);
            }
            if (unlock != Unlock)
            {
                onUnlockStateChanged?.Invoke(Unlock);
            }
        }

        /// <summary>
        /// 移除一个锁
        /// </summary>
        /// <param name="key"></param>
        public void RemoveLock(T key)
        {
            bool unlock = Unlock;
            if (locks.Contains(key))
            {
                locks.Remove(key);
            }
            if (unlock != Unlock)
            {
                onUnlockStateChanged?.Invoke(Unlock);
            }
        }

        /// <summary>
        /// 清除锁
        /// </summary>
        public void ClearLock()
        {
            bool unlock = Unlock;
            locks.Clear();
            if (unlock != Unlock)
            {
                onUnlockStateChanged?.Invoke(Unlock);
            }
        }

        /// <summary>
        /// 包含锁
        /// </summary>
        /// <param name="key"></param>
        public bool ContainsLock(T key)
        {
            return locks.Contains(key);
        }
    }
}