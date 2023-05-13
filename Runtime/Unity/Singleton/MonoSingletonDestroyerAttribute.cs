/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.12.12
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using UnityEngine;

namespace PBBox
{
    /// <summary>
    /// 销毁Unity脚本的单例
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class MonoSingletonDestroyerAttribute : CustomSingletonDestroyerAttribute
    {
        public override void DestroyInstance<T>(T instance)
        {
            if (instance != null && instance is Component _instance)
            {
                if (_instance != null && _instance.gameObject != null)
                {
                    GameObject.Destroy(_instance.gameObject);
                }
            }
        }
    }
}