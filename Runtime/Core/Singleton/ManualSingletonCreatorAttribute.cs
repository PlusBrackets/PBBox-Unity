/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    /// <summary>
    /// 手动创建单例，该特性会使单例类无法自动创建，需要手动使用SetInstance来设置实例
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ManualSingletonCreatorAttribute : CustomSingletonCreatorAttribute
    {
        public override T CreateInstance<T>()
        {
            Log.Error(
                $"This singleton must be set manually, please use Singleton<{typeof(T).Name}>.SetInstance()",
                typeof(Singleton<T>).Name,
                Log.PBBoxLoggerName
                );
            return null;
        }
    }
}