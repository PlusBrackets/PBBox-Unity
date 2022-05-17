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
    public abstract class SingleClass<T> where T : SingleClass<T>
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
                    _instance = _Create();
                }
            }

        }

        /// <summary>
        /// 关闭单例，慎用
        /// </summary>
        public static void Dispose()
        {
            if (HasInstance)
            {
                _instance = null;
                _instance.OnDispose();
            }
        }

        protected virtual void OnDispose()
        {
        }

        static T _Create()
        {
            Type type = typeof(T);
            try
            {
                return (T)type.Assembly.CreateInstance(type.FullName, true, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
            }
            catch (MissingMethodException ex)
            {
                throw new Exception($"{ex.Message}[{type.Name}类 缺少private的构造函数]");
            }
        }
    }

}