/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.12.12
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
    public abstract class Singleton<T> : ISingleton where T : class, ISingleton, new()
    {
        private static T s_Instance;
        private static readonly object s_Locker = new object();
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    Create();
                }
                return s_Instance;
            }
        }

        /// <summary>
        /// 是否已有单例实例
        /// </summary>
        /// <value></value>
        public static bool HasInstance
        {
            get
            {
                return s_Instance != null;
            }
        }

        public static void Create()
        {
            lock (s_Locker)
            {
                if (s_Instance == null)
                {
                    if (typeof(T).IsDefined(typeof(CustomSingletonCreatorAttribute), false))
                    {
                        //使用特性自定义创建Instance，方便统一处理MonoBehaviour等无法直接使用new来创建的单例，性能大约为直接创建的1/4
                        CustomSingletonCreatorAttribute attribute = null;
                        attribute = Attribute.GetCustomAttribute(typeof(T), typeof(CustomSingletonCreatorAttribute), false) as CustomSingletonCreatorAttribute;
                        s_Instance = attribute.CreateInstance<T>();
                    }
                    else
                    {
                        s_Instance = new T();
                    }
                    if (s_Instance != null && s_Instance is ISingletonLifecycle __instance)
                    {
                        __instance.OnCreateAsSingleton();
                    }
                }
            }
        }

        /// <summary>
        /// 销毁单例，若需要做特殊处理，可以使用ISingletonLifecycle或CustomSingletonDestroyerAttribute进行处理。
        /// </summary>
        public static void Destroy()
        {
            lock (s_Locker)
            {
                if (s_Instance != null)
                {
                    if (s_Instance is ISingletonLifecycle __instance)
                    {
                        __instance.OnDestroyAsSingleton();
                    }
                    if (typeof(T).IsDefined(typeof(CustomSingletonDestroyerAttribute), false))
                    {
                        var attribute = Attribute.GetCustomAttribute(typeof(T), typeof(CustomSingletonDestroyerAttribute), false) as CustomSingletonDestroyerAttribute;
                        attribute.DestroyInstance(s_Instance);
                    }
                }
                s_Instance = null;
            }
        }

        /// <summary>
        /// 直接设置单例，若已有单例实例，则会先销毁先前的实例再设置
        /// </summary>
        /// <param name="newInstance"></param>
        public static void SetInstance(T newInstance)
        {
            lock (s_Locker)
            {
                if (newInstance == s_Instance)
                {
                    return;
                }
                if (HasInstance)
                {
                    Destroy();
                }
                s_Instance = newInstance;
                if (s_Instance != null && s_Instance is ISingletonLifecycle __instance)
                {
                    __instance.OnCreateAsSingleton();
                }
            }
        }

    }
}