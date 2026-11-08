/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.01
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace PBBox.AssetManagement
{

    public interface IAssetLoader
    {
        /// <summary>
        /// 当前资源Key
        /// </summary>
        string Key { get; }
        /// <summary>
        /// 引用计数
        /// </summary>
        int ReferenceCount { get; }
        /// <summary>
        /// 最后一次释放时间（毫秒）
        /// </summary>
        long LastReleaseTimeMs { get; }
        /// <summary>
        /// 是否正在加载
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// 资源是否有效
        /// </summary>
        /// <returns></returns>
        bool IsAssetVaild();

        /// <summary>
        /// 直接获取资源对象，不会加载资源和增加引用计数。
        /// </summary>
        /// <returns></returns>
        object GetCachedAsset();

        /// <summary>
        /// 获取资源对象，并增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <returns></returns>
        TAsset AcquireAsset<TAsset>() where TAsset : class;
        /// <summary>
        /// 异步获取资源对象，并增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <returns></returns>
        Task<TAsset> AcquireAssetAsync<TAsset>(Action<TAsset> onLoadFinish) where TAsset : class;
        /// <summary>
        /// 获取资源对象，并增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="callback"></param>
        /// <returns></returns>
        IList<TAsset> AcquireAssets<TAsset>() where TAsset : class;
        /// <summary>
        /// 获取所有资源对象，并增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="callback"></param>
        /// <returns></returns>
        Task<IList<TAsset>> AcquireAssetsAsync<TAsset>(Action<IList<TAsset>> onLoadFinish, Action<TAsset> onLoadOne) where TAsset : class;

        /// <summary>
        /// 释放资源对象的引用计数
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="asset"></param>
        void ReleaseAsset();
        /// <summary>
        /// 释放资源
        /// </summary>
        bool TryUnloadAsset();

    }
}