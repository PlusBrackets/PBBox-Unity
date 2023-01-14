/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.12.12
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    /// <summary>
    /// 自定义单例的创建
    /// </summary>
    public abstract class CustomSingletonCreatorAttribute : Attribute
    {
        public abstract T CreateInstance<T>() where T : class, ISingleton, new();
    }

}