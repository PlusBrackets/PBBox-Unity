﻿/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;

namespace PBBox
{
    /// <summary>
    /// 简单的单例,不会自动创建，用于方便访问
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SimpleSingleBehaviour<T> : MonoBehaviour where T : SimpleSingleBehaviour<T>
    {
        public static T Current { get; private set; }

        protected virtual void Awake()
        {
            if (Current == null)
            {
                Current = this as T;
                Init();
            }
            else
                Destroy(this);
        }

        protected virtual void OnDestroy()
        {
            if (Current == this)
                Current = null;
        }

        protected virtual void Init()
        {

        }

    }

    /// <summary>
    /// 单例脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingleBehaviour<T> : MonoBehaviour where T : SingleBehaviour<T>
    {
        private static T _instance = null;
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
        /// 在重复的时候删除gameObject,若false则仅删除脚本
        /// </summary>
        /// <value></value>        
        protected virtual bool destroyObjectOnDuplicate{get;} = false;
        protected virtual bool hideInHierarchy{get;} = false;
        /// <summary>
        /// 切换场景时会被自动销毁
        /// </summary>
        public virtual bool autoDestroy { get; } = false;

        /// <summary>
        /// 手动实例化单例
        /// </summary>
        public static void Create()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    GameObject mountObj = new GameObject();
                    mountObj.transform.position = new Vector3(0, 0, 0);
                    mountObj.name = "Single_" + typeof(T).Name;
                    mountObj.AddComponent<T>();
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = this as T;
                        if (_instance != null)
                        {
                            if (!autoDestroy)
                                DontDestroyOnLoad(_instance);
                            if(!hideInHierarchy)
                                hideFlags |= HideFlags.HideInHierarchy;
                            _instance.Init();
                        }
                    }
                }
            }
            if (_instance != this)
            {
                // DebugUtils.Internal.LogWarning($"已经存在{typeof(T).FullName}的单例");
                if(destroyObjectOnDuplicate){
                    Destroy(gameObject);
                }
                else{
                    Destroy(this);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        protected virtual void Init()
        {

        }
    }
}
