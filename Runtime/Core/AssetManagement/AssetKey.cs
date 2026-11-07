/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.08
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PBBox
{
    [System.Serializable]
    public struct AssetKey
    {
        public string Key;
        public AssetLoadType LoadType;

        public AssetKey(string key, AssetLoadType loadType)
        {
            Key = key;
            LoadType = loadType;
        }
    }

    public static partial class PBExtensions
    {
        /// <summary>
        /// 加载紫源，如果资源已加载，则增加引用计数并返回已加载的资源。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="manager"></param>
        /// <param name="assetKey"></param>
        /// <returns></returns>
        public static TAsset LoadAsset<TAsset>(this IAssetManager manager, AssetKey assetKey) where TAsset : class
        {
            return manager.LoadAsset<TAsset>(assetKey.Key, assetKey.LoadType);
        }

        /// <summary>
        /// 加载key路径的所有资源，如果资源已加载，则增加引用计数并返回已加载的资源。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="manager"></param>
        /// <param name="assetKey"></param>
        /// <returns></returns>
        public static IList<TAsset> LoadAssetAll<TAsset>(this IAssetManager manager, AssetKey assetKey) where TAsset : class
        {
            return manager.LoadAssetAll<TAsset>(assetKey.Key, assetKey.LoadType);
        }

        /// <summary>
        /// 异步加载资源，如果资源已加载，则增加引用计数并返回已加载的资源。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="manager"></param>
        /// <param name="assetKey"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static Task<TAsset> LoadAssetAsync<TAsset>(this IAssetManager manager, AssetKey assetKey, System.Action<TAsset> callBack = null) where TAsset : class
        {
            return manager.LoadAssetAsync<TAsset>(assetKey.Key, assetKey.LoadType, callBack);
        }

        /// <summary>
        /// 异步加载key路径的所有资源，如果资源已加载，则增加引用计数并返回已加载的资源。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="manager"></param>
        /// <param name="assetKey"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static Task<IList<TAsset>> LoadAssetAllAsync<TAsset>(this IAssetManager manager, AssetKey assetKey, System.Action<IList<TAsset>> callBack = null) where TAsset : class
        {
            return manager.LoadAssetAllAsync<TAsset>(assetKey.Key, assetKey.LoadType, callBack);
        }

        /// <summary>
        /// 释放资源对象的引用计数，如果引用计数为0，则根据卸载策略进行资源卸载
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="manager"></param>
        /// <param name="assetKey"></param>
        /// <param name="asset"></param>
        public static void ReleaseAsset<TAsset>(this IAssetManager manager, AssetKey assetKey, ref TAsset asset) where TAsset : class
        {
            manager.ReleaseAsset<TAsset>(assetKey.Key, ref asset, assetKey.LoadType);
        }

        /// <summary>
        /// 释放资源对象的引用计数，如果引用计数为0，则根据卸载策略进行资源卸载
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="assetKey"></param>
        public static void ReleaseAsset(this IAssetManager manager, AssetKey assetKey)
        {
            manager.ReleaseAsset(assetKey.Key, assetKey.LoadType);
        }
    }
}