/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.02.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;

namespace PBBox
{
    /// <summary>
    /// Unity脚本单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>, new()
    {
        public static T Instance => Singleton<T>.Instance;
        public static bool HasInstance => Singleton<T>.HasInstance;
        public static void Create() => Singleton<T>.Create();
        public static void Destroy() => Singleton<T>.Destroy();
        public static void SetInstance(T newInstance) => Singleton<T>.SetInstance(newInstance);
    }
}