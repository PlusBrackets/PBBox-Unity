/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.12.12
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    /// <summary>
    /// 自定义单例的销毁
    /// </summary>
    public abstract class CustomSingletonDestroyerAttribute : Attribute
    {
        public abstract void DestroyInstance<T>(T instance);
    }
}