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
#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        private string m_Key;
#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        private AssetLoadType m_LoadType;

        public string Key => m_Key;
        /// <summary>
        /// 加载方式
        /// </summary>
        public int LoadTypeId => (int)m_LoadType;
        /// <summary>
        /// 加载方式
        /// </summary>
        public AssetLoadType LoadType => m_LoadType;

        public AssetKey(string key, AssetLoadType loadType)
        {
            m_Key = key;
            m_LoadType = loadType;
        }

        public AssetKey(string key, int loadTypeId)
        {
            m_Key = key;
            m_LoadType = (AssetLoadType)loadTypeId;
        }

        public AssetKey(string key)
        {
            m_Key = key;
            m_LoadType = AssetLoadType.Default;
        }
    }

    public static partial class PBExtensions
    {
        /// <summary>
        /// 获取Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="manager"></param>
        /// <param name="assetKey"></param>
        /// <returns></returns>
        public static TAsset AcquireAsset<TAsset>(this IAssetManager manager, AssetKey assetKey) where TAsset : class
        {
            return manager.LoadAsset<TAsset>(assetKey.Key, assetKey.LoadType);
        }

        /// <summary>
        /// 获取key下的多个Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="manager"></param>
        /// <param name="assetKey"></param>
        /// <returns></returns>
        public static IList<TAsset> AcquireAssets<TAsset>(this IAssetManager manager, AssetKey assetKey) where TAsset : class
        {
            return manager.AcquireAssets<TAsset>(assetKey.Key, (int)assetKey.LoadType);
        }

        /// <summary>
        /// 异步获取Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="manager"></param>
        /// <param name="assetKey"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static Task<TAsset> AcquireAssetAsync<TAsset>(this IAssetManager manager, AssetKey assetKey, System.Action<TAsset> callBack = null) where TAsset : class
        {
            return manager.AcquireAssetAsync<TAsset>(assetKey.Key, (int)assetKey.LoadType, callBack);
        }

        /// <summary>
        /// 异步获取key下的多个Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="manager"></param>
        /// <param name="assetKey"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static Task<IList<TAsset>> AcquireAssetsAsync<TAsset>(this IAssetManager manager, AssetKey assetKey, System.Action<IList<TAsset>> callBack = null) where TAsset : class
        {
            return manager.AcquireAssetsAsync<TAsset>(assetKey.Key, assetKey.LoadType, callBack);
        }

        /// <summary>
        /// 释放资源对象的引用计数，如果引用计数为0，则根据卸载策略进行资源卸载
        /// </summary>
        /// <typeparam name="TAssetOrAssets"></typeparam>
        /// <param name="manager"></param>
        /// <param name="assetKey"></param>
        /// <param name="asset"></param>
        public static void ReleaseAsset<TAssetOrAssets>(this IAssetManager manager, AssetKey assetKey, ref TAssetOrAssets asset) where TAssetOrAssets : class
        {
            manager.ReleaseAsset<TAssetOrAssets>(assetKey.Key, ref asset, (int)assetKey.LoadType);
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