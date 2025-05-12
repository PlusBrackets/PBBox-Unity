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
    /// 创建Unity脚本的的单例
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class MonoSingletonCreatorAttribute : CustomSingletonCreatorAttribute
    {
        /// <summary>
        /// 是否在场景中寻找脚本作为单例
        /// </summary>
        /// <value></value>
        public bool FindInScene { get; set; } = false;
        /// <summary>
        /// 若无法在场景中找到脚本，是否可以自动创建，默认为true
        /// </summary>
        /// <value></value>
        public bool AutoCreateIfNotFound { get; set; } = true;
        public bool DontDestroyOnLoad { get; set; } = true;
        public HideFlags HideFlags { get; set; } = HideFlags.None;
        /// <summary>
        /// 自动创建时使用Resources.Load从设置的路径加载单例预制体
        /// </summary>
        /// <value></value>
        public string LoadFromResourcesPath { get; set; } = null;
        /// <summary>
        /// 自动创建时使用Addressable从设置的路径加载单例预制体
        /// </summary>
        /// <value></value>
        public string LoadFromAddressableKey { get; set; } = null;

        public override T CreateInstance<T>(Type instanceType)
        {
            T component = null;
            GameObject go = null;
            if (FindInScene)//在场景中寻找脚本作为单例
            {
#if UNITY_6000_0_OR_NEWER
                component = GameObject.FindFirstObjectByType(instanceType) as T;
#else
                component = GameObject.FindObjectOfType(instanceType) as T;
#endif
                if (component != null)
                {
                    go = (component as Component).gameObject;
                }
                else if (!AutoCreateIfNotFound)
                {
                    Log.Error(
                        "Failed to find any [" + typeof(T).Name + "] in scene, and it will not be automatically created.",
                        typeof(T).Name,
                        Log.PBBoxLoggerName
                        );
                    return null;
                }
            }
            if (go == null)
            {
                //TODO 资源加载接入资源管理类
                if (!string.IsNullOrEmpty(LoadFromResourcesPath))//使用Resources加载单例
                {
                    go = Resources.Load<GameObject>(LoadFromResourcesPath);
                    if (go == null)
                    {
                        Log.Error(
                            "Failed to load prefab from path [" + LoadFromResourcesPath + "]",
                            typeof(T).Name,
                            Log.PBBoxLoggerName
                            );
                        return null;
                    }
                    go = GameObject.Instantiate(go);
                    component = go.GetComponent(instanceType) as T;
                }
                else if (!string.IsNullOrEmpty(LoadFromAddressableKey))//使用Addressable加载单例
                {
                    go = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(LoadFromAddressableKey).WaitForCompletion();
                    if (go == null)
                    {
                        Log.Error(
                            "Failed to load prefab from path [" + LoadFromAddressableKey + "]",
                            typeof(T).Name,
                            Log.PBBoxLoggerName
                            );
                        return null;
                    }
                    go = GameObject.Instantiate(go);
                    component = go.GetComponent(instanceType) as T;
                }
                else//自动创建单例
                {
                    go = new GameObject("Singleton_" + typeof(T).Name);
                    component = go.AddComponent(instanceType) as T;
                }
            }
            if (DontDestroyOnLoad)
            {
                GameObject.DontDestroyOnLoad(go);
            }
            go.hideFlags = HideFlags;
            return component;
        }
    }
}
