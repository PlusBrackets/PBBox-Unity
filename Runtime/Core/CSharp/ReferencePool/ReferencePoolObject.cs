/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.24
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;

namespace PBBox
{
    /// <summary>
    /// 引用池对象，简化引用池的使用，访问Reference时会自动创建对象，Release时会自动释放对象，使用时要注意结构体的值传递特性，避免再外部Acquire或Release
    /// </summary>
    public struct RefPoolObject<TItemType> where TItemType : class, new()
    {
        private TItemType value;

        /// <summary>
        /// 不会自动创建对象
        /// </summary>
        public TItemType Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// 如果没有引用对象，则自动获得一个并返回
        /// </summary>
        public TItemType AcquiredValue
        {
            get
            {
                TryAcquire();
                return value;
            }
        }

        public bool IsCreated => value != null;

        public void Release()
        {
            if (value != null)
            {
                ReferencePool.Release(value);
                value = null;
            }
        }

        /// <summary>
        /// 如果没有引用对象，则获得一个
        /// </summary>
        public void TryAcquire()
        {
            if (value == null)
            {
                value = ReferencePool.Acquire<TItemType>();
            }
        }
    }
}
