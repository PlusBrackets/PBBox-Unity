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
    /// 创建Unity 资源的的单例，例如ScriptableObject
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class AssetSingletonCreatorAttribute : CustomSingletonCreatorAttribute
    {
        /// <summary>
        /// 自动创建时使用Resources.Load从设置的路径加载资源
        /// </summary>
        /// <value></value>
        public string LoadFromResourcesPath { get; set; } = null;
        /// <summary>
        /// 自动创建时使用Addressable从设置的路径加载资源
        /// </summary>
        /// <value></value>
        public string LoadFromAddressableKey { get; set; } = null;

        public override T CreateInstance<T>()
        {
            T _asset = null;
            if (!string.IsNullOrEmpty(LoadFromResourcesPath))
            {
                _asset = Resources.Load(LoadFromResourcesPath, typeof(T)) as T;
            }
            else if (!string.IsNullOrEmpty(LoadFromAddressableKey))
            {
                //Log.Debug($"Load AssetSingleton from Addressable:{LoadFromAddressableKey} Start.");
                _asset = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(LoadFromAddressableKey).WaitForCompletion();
                //Log.Debug($"Load AssetSingleton from Addressable:{LoadFromAddressableKey} Done.");
            }
            return _asset;
        }
    }
}
