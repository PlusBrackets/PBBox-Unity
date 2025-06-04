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
    /// 单例
    /// </summary>
    public interface ISingleton
    {
        protected static bool s_IsInitialized = false;
        /// <summary>
        /// 通过反射注册各接口型单例的具体实现类型
        /// </summary>
#if UNITY_5_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        internal static void InitInstaceTypes()
        {
            if (s_IsInitialized)
            {
                return;
            }
            s_IsInitialized = true;
            var assemblyNames = PBBoxSettings.CommonInitReflectAssemblies.Union(PBBoxSettings.InitReflectAssemblies_Singleton);
            //获得所有继承了ISingleton的具体类
            var types = typeof(ISingleton).GetAllChildClass(moreDeep: true, containAbstract: false, assemblyNames: assemblyNames);

#if PB_TEST_LOG || UNITY_EDITOR
            var test = new System.Diagnostics.Stopwatch();
            test.Start();
#endif
            //获取其中继承了ISingleton<T>且T不等于该type的类型，并返回type和T的集合列表
            var typeMap = types
                .SelectMany(type => type.GetInterfaces()
                    .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ISingleton<>) && t.GetGenericArguments()[0] != type)
                    .Select(t => (type, t.GetGenericArguments()[0])));
            Dictionary<Type, (Type instanceType, int priority)> tempMap = new Dictionary<Type, (Type instanceType, int priority)>();
            foreach (var (type, interfaceType) in typeMap)
            {
                int priority = 0;
                //检查instanceType是否有SingletonPriorityAttribute特性，若有则重设优先级
                var attr = type.GetCustomAttribute<SingletonPriorityAttribute>(false);
                if (attr != null)
                {
                    priority = attr.Priority;
                }
                if (tempMap.TryGetValue(interfaceType, out var oldValue))
                {
                    if (oldValue.priority < priority)
                    {
                        tempMap[interfaceType] = (type, priority);
                    }
                }
                else
                {
                    tempMap.Add(interfaceType, (type, priority));
                }
            }
            Type[] setInstanceTypeParameterType = new Type[] { typeof(Type) };
            foreach (var (interfaceType, (instanceType, priority)) in tempMap)
            {
                // 获取具体的泛型接口类型
                Type genericInterfaceType = typeof(ISingleton<>).MakeGenericType(interfaceType);

                // 调用泛型接口的SetInstanceType静态方法
                MethodInfo setTypeMethod = genericInterfaceType.GetMethod(
                    "SetInstanceType",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    setInstanceTypeParameterType,
                    null);

                if (setTypeMethod != null)
                {
                    setTypeMethod.Invoke(null, new object[] { instanceType });
                }
                else
                {
                    Log.Error($"SetInstanceType method not found in {genericInterfaceType.Name}.", "ISingleton", Log.PBBoxLoggerName);
                }
            }
#if PB_TEST_LOG || UNITY_EDITOR
            test.Stop();
            System.Text.StringBuilder logs = new System.Text.StringBuilder();
            logs.AppendLine($"单例间接绑定完成，耗时:{test.Elapsed.TotalMilliseconds}ms");
            foreach (var (interfaceType, (instanceType, priority)) in tempMap)
            {
                logs.AppendLine($"{interfaceType.Name}-->{instanceType.Name}，优先级：{priority}");
            }
            Log.Debug(logs.ToString(), "ISingleton", Log.PBBoxLoggerName);
#endif
        }

        /// <summary>
        /// 重新初始化单例类型映射
        /// </summary>
        internal static void ReInitInstaceTypes()
        {
            s_IsInitialized = false;
            InitInstaceTypes();
        }
    }

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


        public static Type InstanceType
        {
            get
            {
                if (s_InstanceType == null)
                {
                    s_InstanceType = typeof(T);
                }
                return s_InstanceType;
            }
        }

        public static T Instance => GetInstance();

        public static T GetInstance()
        {
            if (!HasInstance())
            {
                Create();
            }
            return s_Instance;
        }

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
                    Log.Warning($"Instance already exists. Use Destroy() to remove the existing instance before creating a new one.", typeof(T).Name, Log.PBBoxLoggerName);
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
                    Log.Error($"Instance already exists. Use Destroy() to remove the existing instance before setting a new type.", typeof(T).Name, Log.PBBoxLoggerName);
                    return;
                }
                if (!s_IsInitialized)
                {
                    InitInstaceTypes();
                }
                s_InstanceType = type;
            }
        }
    }
}