/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.02
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    /// <summary>
    /// 单例。可以继承使用，也可以直接Singleton<Class>.XXX使用
    /// 可以使用CustomSingletonCreatorAttribute等特性来自定义创建实例。
    /// </summary>
    /// <typeparam name="T">单例类型</typeparam>
    public abstract class Singleton<T> : ISingleton<T> where T : class, ISingleton, new()
    {
        public static T Instance => ISingleton<T>.Instance;

        /// <summary>
        /// 是否已有单例实例
        /// </summary>
        /// <value></value>
        public static bool HasInstance() => ISingleton<T>.HasInstance();
        /// <summary>
        /// 创建单例，若已有单例实例，则会创建失败
        /// </summary>
        public static void Create() => ISingleton<T>.Create();

        /// <summary>
        /// 销毁单例，若需要做特殊处理，可以使用ISingletonLifecycle或CustomSingletonDestroyerAttribute进行处理。
        /// </summary>
        public static void Destroy() => ISingleton<T>.Destroy();

        /// <summary>
        /// 直接设置单例，若已有单例实例，则会先销毁先前的实例再设置
        /// </summary>
        /// <param name="newInstance"></param>
        public static void SetInstance(T newInstance) => ISingleton<T>.SetInstance(newInstance);

    }
}