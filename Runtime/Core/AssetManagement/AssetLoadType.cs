/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.04
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PBBox
{
    /// <summary>
    /// 资源加载类型
    /// </summary>
    public enum AssetLoadType
    {
        /// <summary>
        /// 使用默认的资源加载类型，可在PBBoxSettings.AssetLoaderDefaultId中设置。
        /// <para>默认使用Resources加载器</para>
        /// </summary>
        Default = 0,
#if UNITY_5_3_OR_NEWER
        /// <summary>
        /// Unity从Resources文件夹加载资源
        /// </summary>
        Resources = 1,
        Addressable = 2,
        //AssetBundle = 3,
#endif
    }

    public static partial class PBExtensions
    {
        /// <summary>
        /// 获取Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        public static TAsset AcquireAsset<TAsset>(this IAssetManager manager, string key, AssetLoadType loadType) where TAsset : class
            => manager.AcquireAsset<TAsset>(key, (int)loadType);

        /// <summary>
        /// 获取Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        public static TAsset LoadAsset<TAsset>(this IAssetManager manager, string key, AssetLoadType loadType) where TAsset : class
            => manager.AcquireAsset<TAsset>(key, (int)loadType);

        /// <summary>
        /// 获取key下的多个Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        public static IList<TAsset> AcquireAssets<TAsset>(this IAssetManager manager, string key, AssetLoadType loadType) where TAsset : class
            => manager.AcquireAssets<TAsset>(key, (int)loadType);

        /// <summary>
        /// 异步获取Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        public static Task<TAsset> AcquireAssetAsync<TAsset>(this IAssetManager manager, string key, AssetLoadType loadType, Action<TAsset> callBack = null) where TAsset : class
            => manager.AcquireAssetAsync(key, (int)loadType, callBack);

        /// <summary>
        /// 异步获取Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        public static Task<TAsset> LoadAssetAsync<TAsset>(this IAssetManager manager, string key, AssetLoadType loadType, Action<TAsset> onFinish = null) where TAsset : class
            => manager.AcquireAssetAsync(key, (int)loadType, onFinish);

        /// <summary>
        /// 异步获取key下的多个Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        public static Task<IList<TAsset>> AcquireAssetsAsync<TAsset>(this IAssetManager manager, string key, AssetLoadType loadType, Action<IList<TAsset>> onFinish = null, Action<TAsset> onLoadOne = null) where TAsset : class
            => manager.AcquireAssetsAsync(key, (int)loadType, onFinish, onLoadOne);

        /// <summary>
        /// 释放资源对象的引用计数，如果引用计数为0，则根据卸载策略进行资源卸载
        /// </summary>
        public static void ReleaseAsset<TAsset>(this IAssetManager manager, string key, ref TAsset asset, AssetLoadType loadType) where TAsset : class
            => manager.ReleaseAsset(key, ref asset, (int)loadType);

        /// <summary>
        /// 释放资源对象的引用计数，如果引用计数为0，则根据卸载策略进行资源卸载
        /// </summary>
        public static void ReleaseAsset(this IAssetManager manager, string key, AssetLoadType loadType)
            => manager.ReleaseAsset(key, (int)loadType);

        /// <summary>
        /// 手动卸载指定key的资源对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <returns></returns>
        public static bool TryUnloadUnusedAsset(this IAssetManager manager, string key, AssetLoadType loadType)
            => manager.TryUnloadUnusedAsset(key, (int)loadType);
    }
}