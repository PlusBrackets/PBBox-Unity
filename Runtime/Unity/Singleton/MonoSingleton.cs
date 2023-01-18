using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox
{
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>, new()
    {
        public static T Instance => Singleton<T>.Instance;
        public static bool HasInstance => Singleton<T>.HasInstance;
        public static void Create() => Singleton<T>.Create();
        public static void Destroy() => Singleton<T>.Destroy();
        public static void SetInstance(T newInstance) => Singleton<T>.SetInstance(newInstance);
    }
}