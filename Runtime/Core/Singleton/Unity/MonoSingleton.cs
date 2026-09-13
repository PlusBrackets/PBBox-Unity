/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.02.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;

namespace PBBox
{
    /// <summary>
    /// Unity脚本单例基类，需要搭配MonoSingletonCreatorAttribute和MonoSingletonDestroyAttribute使用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton<T> where T : MonoSingleton<T>
    {
        public static T Instance => ISingleton<T>.Instance;
        public static bool HasInstance() => ISingleton<T>.HasInstance();
        public static void Create() => ISingleton<T>.Create();
        public static void Destroy() => ISingleton<T>.Destroy();
        public static void SetInstance(T newInstance) => ISingleton<T>.SetInstance(newInstance);
    }
}