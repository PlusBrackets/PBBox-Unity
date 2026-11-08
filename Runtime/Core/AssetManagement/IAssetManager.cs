/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.02
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PBBox.AssetManagement;

namespace PBBox
{
    /// <summary>
    /// 资源管理Manager
    /// </summary>
    public interface IAssetManager : ISingleton<IAssetManager>
    {
        /// <summary>
        /// 获取Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <returns></returns>
        TAsset AcquireAsset<TAsset>(string key, int loadType = (int)AssetLoadType.Default) where TAsset : class;
        /// <summary>
        /// 获取key下的多个Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <returns></returns>
        IList<TAsset> AcquireAssets<TAsset>(string key, int loadType = (int)AssetLoadType.Default) where TAsset : class;

        /// <summary>
        /// 异步获取Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        Task<TAsset> AcquireAssetAsync<TAsset>(string key, int loadType = (int)AssetLoadType.Default, Action<TAsset> callBack = null) where TAsset : class;
        /// <summary>
        /// 异步获取key下的多个Asset并增加引用次数，如果未加载则先加载再返回
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        Task<IList<TAsset>> AcquireAssetsAsync<TAsset>(string key, int loadType = (int)AssetLoadType.Default, Action<IList<TAsset>> onFinish = null, Action<TAsset> onLoadOne = null) where TAsset : class;


        /// <summary>
        /// 释放资源对象的引用计数，如果引用计数为0，则根据卸载策略进行资源卸载
        /// </summary>
        /// <param name="key"></param>
        /// <param name="asset">传入asset进行判定，成果release后将asset设置为null</param>
        /// <param name="loadType"></param>
        /// <returns></returns>
        void ReleaseAsset<TAsset>(string key, ref TAsset asset, int loadType = (int)AssetLoadType.Default) where TAsset : class;

        /// <summary>
        /// 释放资源对象的引用计数，如果引用计数为0，则根据卸载策略进行资源卸载
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        void ReleaseAsset(string key, int loadType = (int)AssetLoadType.Default);

        /// <summary>
        /// 设置所有加载类型的卸载策略
        /// </summary>
        /// <param name="key"></param>
        /// <param name="strategy"></param>
        void SetUnloadStrategy(AssetUnloadStrategy strategy);

        /// <summary>
        /// 手动卸载所有当前引用计数为0的资源对象
        /// </summary>
        void UnloadAllUnusedAssets();
        /// <summary>
        /// 手动卸载指定key的资源对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="loadTypeId"></param>
        bool TryUnloadUnusedAsset(string key, int loadType = (int)AssetLoadType.Default);
    }
}