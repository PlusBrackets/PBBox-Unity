/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Linq;
using System;

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
        public static Type[] GetAllChildClass<T>(bool moreDeep = true, bool containAbstract = false, AppDomain domain = null)
        {
            return typeof(T).GetAllChildClass(moreDeep, containAbstract, domain);
        }

        /// <summary>
        /// 获取target的所有子类
        /// </summary>
        /// <param name="target"></param>
        /// <param name="moreDeep"></param>
        /// <param name="containAbstract"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static Type[] GetAllChildClass(this Type target, bool moreDeep = true, bool containAbstract = false, AppDomain domain = null)
        {
            if (domain == null)
                domain = AppDomain.CurrentDomain;
            Type type = target;
            if (moreDeep)
            {
                if (type.IsClass)
                    return domain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => (containAbstract || !t.IsAbstract) && t.IsSubclassOf(type))).ToArray();
                else if (type.IsInterface)
                    return domain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => (containAbstract || !t.IsAbstract) && type.IsAssignableFrom(t))).ToArray();
            }
            else
            {
                if (type.IsClass)
                    return domain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => (containAbstract || !t.IsAbstract) && t.BaseType == type)).ToArray();
                else if (type.IsInterface)
                    return domain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => (containAbstract || !t.IsAbstract) && t.GetInterfaces().Contains(type))).ToArray();
            }

            return null;
        }

    }
}