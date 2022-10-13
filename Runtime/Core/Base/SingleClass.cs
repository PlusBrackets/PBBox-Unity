/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Reflection;

namespace PBBox
{
    /// <summary>
    /// c#类单例
    /// </summary>
    public abstract class SingleClass<T> where T : SingleClass<T>, new()
    {
        protected static T _instance;
        private static object _lock = new object();//线程锁定

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    Create();
                }
                return _instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return _instance != null;
            }
        }


        /// <summary>
        /// 手动实例化单例
        /// </summary>
        public static void Create()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new T();// (T)Activator.CreateInstance(typeof(T));// _Create();
                    _instance.Init();
                }
            }

        }

        protected virtual void Init(){}

        /// <summary>
        /// 销毁单例，慎用
        /// </summary>
        public static void Destroy()
        {
            if (HasInstance)
            {
                var _temp = _instance;
                _instance = null;
                _temp.OnDestroy();
            }
        }

        protected virtual void OnDestroy()
        {
        }

        // static T _Create()
        // {
        //     Type type = typeof(T);
        //     return (T)Activator.CreateInstance(type);// (T)type.Assembly.CreateInstance(type.FullName, true, BindingFlags. | BindingFlags.Instance, null, null, null, null);
        // }
    }

}