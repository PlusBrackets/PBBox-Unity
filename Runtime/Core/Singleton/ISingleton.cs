/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.06
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PBBox
{
    /// <summary>
    /// 单例
    /// </summary>
    public interface ISingleton
    {
        protected static bool s_IsInitialized = false;
        private static readonly object s_Locker_Init = new object();

        /// <summary>
        /// 通过反射注册各接口型单例的具体实现类型
        /// </summary>
#if UNITY_5_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void TryAutoInstanceTypes()
        {
            InitInstaceTypes();
        }
#endif

        /// <summary>
        /// 获取types中继承了ISingleton<T>且T不等于该type或者相等且T为可实例化的类型，并返回type和T的集合列表
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        internal static Dictionary<Type, (Type instanceType, int priority)> GetRegisterMapInTypes(IEnumerable<Type> types)
        {
            var typeMap = types
                .SelectMany(type => type.GetInterfaces()
                    .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ISingleton<>)
                    //TODO 有其他情况未判定，例如interface ITestB:ISingleton<ITestA>之类的情况
                        && (t.GetGenericArguments()[0] != type || (!type.IsInterface && !type.IsAbstract))) //单例t的泛型T与type不一致或者type不是接口不是抽象类
                    .Select(t => (type, t.GetGenericArguments()[0])));
            Dictionary<Type, (Type instanceType, int priority)> tempMap = new Dictionary<Type, (Type instanceType, int priority)>();
            //选定最高优先级的instanceType
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
            return tempMap;
        }

        protected static void InitInstaceTypes()
        {
            lock (s_Locker_Init)
            {
                if (s_IsInitialized)
                {
                    Log.Warning($"Singletons has already been initialized!", nameof(ISingleton), Log.PBBoxLoggerName);
                    return;
                }
                s_IsInitialized = true;
            }
            Log.Info("Register singleton types using reflection", nameof(ISingleton), Log.PBBoxLoggerName);
#if PB_TEST_LOG || UNITY_EDITOR
            var test = new System.Diagnostics.Stopwatch();
            test.Start();
#endif
            var assemblyNames = PBBoxSettings.CommonInitReflectAssemblies.Union(PBBoxSettings.InitReflectAssemblies_Singleton);
            //获得所有继承了ISingleton的具体类
            var types = typeof(ISingleton).GetAllChildClass(moreDeep: true, containAbstract: false, assemblyNames: assemblyNames);


            //获取其中继承了ISingleton<T>且T不等于该type或者相等且T为可实例化的类型，并返回type和T的集合列表
            var tempMap = GetRegisterMapInTypes(types);
            
            foreach (var (interfaceType, (instanceType, priority)) in tempMap)
            {
                SetInstanceType(interfaceType, instanceType);
            }

#if PB_TEST_LOG || UNITY_EDITOR
            test.Stop();
            System.Text.StringBuilder logs = new System.Text.StringBuilder();
            logs.AppendLine($"单例反射绑定完成，耗时:{test.Elapsed.TotalMilliseconds}ms");
            foreach (var (interfaceType, (instanceType, priority)) in tempMap)
            {
                logs.AppendLine($"{interfaceType.FullName}-->{instanceType.FullName}，优先级：{priority}");
            }
            Log.Debug(logs.ToString(), nameof(ISingleton), Log.PBBoxLoggerName);
#endif
        }

        private readonly static Type[] _setInstanceTypeParameterType = new Type[] { typeof(Type) };
        private readonly static object[] _setInstanceTypeParameters = new object[1];

        /// <summary>
        /// 设置单例类型，若已有单例实例，则需要先销毁先前的实例再设置，否则设置失败
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="instanceType"></param>
        public static void SetInstanceType(Type interfaceType, Type instanceType)
        {
            // ISingleton<InterfaceType>.SetInstanceType(instanceType);
            // 获取具体的泛型接口类型
            Type genericInterfaceType = typeof(ISingleton<>).MakeGenericType(interfaceType);

            // 调用泛型接口的SetInstanceType静态方法
            MethodInfo setTypeMethod = genericInterfaceType.GetMethod(
                "SetInstanceType",
                BindingFlags.Public | BindingFlags.Static,
                null,
                _setInstanceTypeParameterType,
                null);

            if (setTypeMethod != null)
            {
                _setInstanceTypeParameters[0] = instanceType;
                setTypeMethod.Invoke(null, _setInstanceTypeParameters);
            }
            else
            {
                Log.Error($"SetInstanceType method not found in {genericInterfaceType.Name}.", nameof(ISingleton), Log.PBBoxLoggerName);
            }
        }

        /// <summary>
        /// 使用反射重新初始化单例类型映射
        /// </summary>
        public static void ReInitInstaceTypes()
        {
            lock (s_Locker_Init)
            {
                s_IsInitialized = false;
            }
            InitInstaceTypes();
        }

        /// <summary>
        /// 标记已经初始化完成
        /// </summary>
        public static void MarkInitialized()
        {
            lock (s_Locker_Init)
            {
                s_IsInitialized = true;
            }
        }
    }
}