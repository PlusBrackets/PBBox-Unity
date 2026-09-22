/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.06
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.PlayerLoop;

namespace PBBox
{
    /// <summary>
    /// 单例接口，继承ISingleton<T>可以使用单例模式
    /// 若T是接口或抽像类型，则需要有具体的实现类型，在程序运行时调用进行初始化配置。
    /// 若有多个实现类型，可以使用SingletonPriorityAttribute来设置优先级，或是自行初始化设置
    /// 不支持同一个类继承多个ISingleton<T>，可能会有冲突
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public interface ISingleton<T> : ISingleton where T : class, ISingleton
    {
        protected static T s_Instance;
        protected static Type s_InstanceType;
        private static readonly object s_Locker = new object();

        public static event Action OnCreateEvent;
        public static event Action OnDestroyEvent;

        public static T Instance => GetInstance();
        public static T InstanceRaw => s_Instance;

        public static T GetInstance()
        {
            if (!HasInstance())
            {
                Create();
            }
            return s_Instance;
        }

        /// <summary>
        /// 获取单例的具体类型
        /// </summary>
        /// <returns></returns>
        public static Type GetInstaceType()
        {
            if (s_InstanceType == null)
            {
                s_InstanceType = typeof(T);
            }
            return s_InstanceType;
        }

        /// <summary>
        /// 检查是否有单例实例，不会触发创建
        /// </summary>
        /// <returns></returns>
        public static bool HasInstance()
        {
#if UNITY_5_3_OR_NEWER
            if (s_Instance is UnityEngine.Object uObj)
            {
                return uObj != null;
            }
#endif
            return s_Instance != null;
        }

        /// <summary>
        /// 创建单例，若已有单例实例，则会创建失败
        /// </summary>
        public static void Create()
        {
            lock (s_Locker)
            {
                if (HasInstance())
                {
                    Log.Warning($"Instance already exists. Use Destroy() to remove the existing instance before creating a new one.", nameof(T), Log.PBBoxLoggerName);
                    return;
                }
                if (!s_IsInitialized)
                {
                    InitInstaceTypes();
                }
                if (s_InstanceType == null)
                {
                    s_InstanceType = typeof(T);
                }
                if (s_InstanceType.IsDefined(typeof(CustomSingletonCreatorAttribute), false))
                {
                    CustomSingletonCreatorAttribute attribute = Attribute.GetCustomAttribute(s_InstanceType, typeof(CustomSingletonCreatorAttribute), false) as CustomSingletonCreatorAttribute;
                    s_Instance = attribute.CreateInstance<T>(s_InstanceType);
                }
                else
                {
                    if (typeof(T) != s_InstanceType)
                    {
                        s_Instance = Activator.CreateInstance(s_InstanceType) as T;
                    }
                    else
                    {
                        s_Instance = Activator.CreateInstance<T>();
                    }
                }
                if (s_Instance != null)
                {
                    if (s_Instance is ISingletonLifecycle lifecycle)
                    {
                        lifecycle.OnCreateAsSingleton();
                    }
                    OnCreateEvent?.Invoke();
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
                if (HasInstance())
                {
                    if (s_Instance is ISingletonLifecycle lifecycle)
                    {
                        lifecycle.OnDestroyAsSingleton();
                    }
                    OnDestroyEvent?.Invoke();
                    if (s_InstanceType.IsDefined(typeof(CustomSingletonDestroyerAttribute), false))
                    {
                        var attribute = Attribute.GetCustomAttribute(s_InstanceType, typeof(CustomSingletonDestroyerAttribute), false) as CustomSingletonDestroyerAttribute;
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
                if (HasInstance())
                {
                    Destroy();
                }
                // 刷新实例类型
                SetInstanceType(newInstance.GetType());
                s_Instance = newInstance;
                if (s_Instance != null)
                {
                    if (s_Instance is ISingletonLifecycle lifecycle)
                    {
                        lifecycle.OnCreateAsSingleton();
                    }
                    OnCreateEvent?.Invoke();
                }
            }
        }

        /// <summary>
        /// 设置单例类型，若已有单例实例，则需要先销毁先前的实例再设置，否则设置失败
        /// </summary>
        /// <param name="type"></param>
        public static void SetInstanceType<TType>() where TType : T
        {
            SetInstanceType(typeof(TType));
        }

        /// <summary>
        /// 设置单例类型，若已有单例实例，则需要先销毁先前的实例再设置，否则设置失败
        /// </summary>
        /// <param name="type"></param>
        public static void SetInstanceType(Type type)
        {
            lock (s_Locker)
            {
                if (s_InstanceType == type)
                {
                    return;
                }
                if (HasInstance())
                {
                    Log.Error($"Instance already exists. Use Destroy() to remove the existing instance before setting a new type.", nameof(T), Log.PBBoxLoggerName);
                    return;
                }
                s_InstanceType = type;
            }
        }
    }
}