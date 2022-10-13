
/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace PBBox
{
    /// <summary>
    /// c#反射工具
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// 获取T的所有子类
        /// </summary>
        /// <param name="moreDeep"></param>
        /// <param name="containAbstract">是否包括抽象类</param>
        /// <param name="domain"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllChildClass<T>(bool moreDeep = true, bool containAbstract = false, AppDomain domain = null, string[] assemblyNames = null)
        {
            return GetAllChildClass(typeof(T), moreDeep, containAbstract, domain, assemblyNames);
        }

        /// <summary>
        /// 获取target的所有子类
        /// </summary>
        /// <param name="target"></param>
        /// <param name="moreDeep"></param>
        /// <param name="containAbstract"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllChildClass(this Type target, bool moreDeep = true, bool containAbstract = false, AppDomain domain = null, string[] assemblyNames = null)
        {
#if GAME_TEST
            var test = new System.Diagnostics.Stopwatch();
            test.Start();
#endif
            if (domain == null)
                domain = AppDomain.CurrentDomain;
            Type type = target;
            IEnumerable<Type> result = null;
            if (moreDeep)
            {
                if (type.IsClass)
                    result = domain.GetAssemblies().Where(a => assemblyNames == null || assemblyNames.Length == 0 || assemblyNames.Contains(a.GetName().Name))
                        .SelectMany(a => a.GetTypes().Where(t => (containAbstract || !t.IsAbstract) && t.IsSubclassOf(type)));
                else if (type.IsInterface)
                    result = domain.GetAssemblies().Where(a => assemblyNames == null || assemblyNames.Length == 0 || assemblyNames.Contains(a.GetName().Name))
                        .SelectMany(a => a.GetTypes().Where(t => (containAbstract || !t.IsAbstract) && type.IsAssignableFrom(t)));
            }
            else
            {
                if (type.IsClass)
                    result = domain.GetAssemblies().Where(a => assemblyNames == null || assemblyNames.Length == 0 || assemblyNames.Contains(a.GetName().Name))
                        .SelectMany(a => a.GetTypes().Where(t => (containAbstract || !t.IsAbstract) && t.BaseType == type));
                else if (type.IsInterface)
                    result = domain.GetAssemblies().Where(a => assemblyNames == null || assemblyNames.Length == 0 || assemblyNames.Contains(a.GetName().Name))
                        .SelectMany(a => a.GetTypes().Where(t => (containAbstract || !t.IsAbstract) && t.GetInterfaces().Contains(type)));
            }
#if GAME_TEST

            test.Stop();
            DebugUtils.Test.Log($"反射获取{target}的子类(count:{result.Count()}),用时(ms):{test.Elapsed.TotalMilliseconds}");
#endif
            return result;
        }

        /// <summary>
        /// 获取所有带有给定Attribute的类
        /// </summary>
        /// <param name="containAbstract"></param>
        /// <param name="inherit"></param>
        /// <param name="domain"></param>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllClassWithAttribute<TAttribute>(bool containAbstract = false, bool inherit = true, AppDomain domain = null, string[] assemblyNames = null) where TAttribute : Attribute
        {
#if GAME_TEST
            var test = new System.Diagnostics.Stopwatch();
            test.Start();
#endif
            if (domain == null)
                domain = AppDomain.CurrentDomain;
            var result = domain.GetAssemblies().Where(a => assemblyNames == null || assemblyNames.Length == 0 || assemblyNames.Contains(a.GetName().Name))
                .SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(TAttribute), inherit)));
#if GAME_TEST
            test.Stop();
            DebugUtils.Test.Log($"反射获取带有{typeof(TAttribute)}特性的类型(count:{result.Count()}),用时(ms):{test.Elapsed.TotalMilliseconds}");
#endif
            return result;
        }

        /// <summary>
        /// 获得所有带有给定Attribute的子类
        /// </summary>
        /// <param name="target"></param>
        /// <param name="inherit"></param>
        /// <param name="moreDeep"></param>
        /// <param name="containAbstract"></param>
        /// <param name="domain"></param>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllChildClassWithAttribute<TAttribute>(this Type target, bool inherit = false, bool moreDeep = true, bool containAbstract = false, AppDomain domain = null, string[] assemblyNames = null) where TAttribute : Attribute
        {
            return GetAllChildClass(target, moreDeep, containAbstract, domain, assemblyNames).Where(t => t.IsDefined(typeof(TAttribute), inherit));
        }

        /// <summary>
        /// 获得所有带有给定Attribute的子类
        /// </summary>
        /// <param name="inherit"></param>
        /// <param name="moreDeep"></param>
        /// <param name="containAbstract"></param>
        /// <param name="domain"></param>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllChildClassWithAttribute<TType, TAttribute>(bool inherit = false, bool moreDeep = true, bool containAbstract = false, AppDomain domain = null, string[] assemblyNames = null) where TAttribute : Attribute
        {
            return GetAllChildClassWithAttribute<TAttribute>(typeof(TType), inherit, moreDeep, containAbstract, domain, assemblyNames);
        }

        /// <summary>
        /// 获取所有带有给定Attribute的方法
        /// </summary>
        /// <param name="inherit"></param>
        /// <param name="domain"></param>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetAllMethodWithAtturbute<TAttribute>(BindingFlags bindingFlags = BindingFlags.Default, bool inherit = false, AppDomain domain = null, string[] assemblyNames = null)
        {
#if GAME_TEST
            var test = new System.Diagnostics.Stopwatch();
            test.Start();
#endif
            IEnumerable<MethodInfo> result = null;
            if (domain == null)
            {
                domain = AppDomain.CurrentDomain;
            }

            result = domain.GetAssemblies().Where(a => assemblyNames == null || assemblyNames.Length == 0 || assemblyNames.Contains(a.GetName().Name))
                .SelectMany(a => a.GetTypes().SelectMany(t => t.GetMethods(bindingFlags).Where(m => m.IsDefined(typeof(TAttribute), inherit))));
#if GAME_TEST
            test.Stop();
            DebugUtils.Test.Log($"反射获取带有{typeof(TAttribute)}特性的方法(count:{result.Count()}),用时(ms):{test.Elapsed.TotalMilliseconds}");
#endif
            return result;
        }

        /// <summary>
        /// 获取所有带有给定Attribute的方法
        /// </summary>
        /// <param name="inherit"></param>
        /// <param name="domain"></param>
        /// <typeparam name="TMethodAttribute"></typeparam>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetAllMethodWithAtturbute<TClassAttribute, TMethodAttribute>(BindingFlags bindingFlags = BindingFlags.Default, bool inherit = false, AppDomain domain = null, string[] assemblyNames = null)
        {
#if GAME_TEST
            var test = new System.Diagnostics.Stopwatch();
            test.Start();
#endif
            IEnumerable<MethodInfo> result = null;
            if (domain == null)
            {
                domain = AppDomain.CurrentDomain;
            }

            result = domain.GetAssemblies().Where(a => assemblyNames == null || assemblyNames.Length == 0 || assemblyNames.Contains(a.GetName().Name))
                .SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(TClassAttribute), inherit)).SelectMany(t => t.GetMethods(bindingFlags).Where(m => m.IsDefined(typeof(TMethodAttribute), inherit))));
#if GAME_TEST
            test.Stop();
            DebugUtils.Test.Log($"反射获取{typeof(TClassAttribute)}的class中带有{typeof(TMethodAttribute)}特性的方法(count:{result.Count()}),用时(ms):{test.Elapsed.TotalMilliseconds}");
#endif
            return result;
        }


    }
}