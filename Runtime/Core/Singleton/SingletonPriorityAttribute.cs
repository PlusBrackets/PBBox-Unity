/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.02
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    /// <summary>
    /// 单例优先级特性
    /// 用于使用接口单例时，自动配置初始具体类型的优先级，不使用时默认为0，越大优先级越高。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SingletonPriorityAttribute : System.Attribute
    {
        /// <summary>
        /// 优先级,值越大优先级越高
        /// </summary>
        public short Priority { get; private set; }

        public SingletonPriorityAttribute(short priority)
        {
            Priority = priority;
        }

        public SingletonPriorityAttribute(Type singletonType, short priority)
        {
            Priority = priority;
        }
    }
}