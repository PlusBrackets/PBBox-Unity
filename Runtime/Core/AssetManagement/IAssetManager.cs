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
        ///// <summary>
        ///// 获得指定的资源加载器
        ///// </summary>
        ///// <typeparam name="TLoader"></typeparam>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //TLoader GetLoader<TLoader>(string key) where TLoader : IAssetLoader;
        ///// <summary>
        ///// 获得指定的资源加载器，如果未指定类型，则使用默认加载器类型
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="loaderType"></param>
        ///// <returns></returns>
        //IAssetLoader GetLoader(string key, Type loaderType = null);
        ///// <summary>
        ///// 获得指定类型ID的加载器
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="loaderTypeId"></param>
        ///// <returns></returns>
        //IAssetLoader GetLoader(string key, int loaderTypeId);
        ///// <summary>
        ///// 获得指定类型ID的加载器
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="loaderType"></param>
        ///// <returns></returns>
        //IAssetLoader GetLoader(string key, AssetLoadTypes loaderType) => GetLoader(key, (int)loaderType);

        /// <summary>
        /// 加载紫源，如果资源已加载，则增加引用计数并返回已加载的资源。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <returns></returns>
        TAsset LoadAsset<TAsset>(string key, int loadType = (int)AssetLoadType.Default) where TAsset : class;
        /// <summary>
        /// 加载key路径的所有资源，如果资源已加载，则增加引用计数并返回已加载的资源。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <returns></returns>
        IList<TAsset> LoadAssetAll<TAsset>(string key, int loadType = (int)AssetLoadType.Default) where TAsset : class;

        /// <summary>
        /// 加载紫源，如果资源已加载，则增加引用计数并返回已加载的资源。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <returns></returns>
        TAsset LoadAsset<TAsset>(string key, AssetLoadType loadType) where TAsset : class => LoadAsset<TAsset>(key, (int)loadType);
        /// <summary>
        /// 加载key路径的所有资源，如果资源已加载，则增加引用计数并返回已加载的资源。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <returns></returns>
        IList<TAsset> LoadAssetAll<TAsset>(string key, AssetLoadType loadType) where TAsset : class => LoadAssetAll<TAsset>(key, (int)loadType);

        /// <summary>
        /// 异步加载资源，如果资源已加载，则增加引用计数并返回已加载的资源。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        Task<TAsset> LoadAssetAsync<TAsset>(string key, int loadType = (int)AssetLoadType.Default, Action<TAsset> callBack = null) where TAsset : class;
        /// <summary>
        /// 异步加载key路径的所有资源，如果资源已加载，则增加引用计数并返回已加载的资源。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        Task<IList<TAsset>> LoadAssetAllAsync<TAsset>(string key, int loadType = (int)AssetLoadType.Default, Action<IList<TAsset>> onFinish = null, Action<TAsset> onLoadOne = null) where TAsset : class;

        /// <summary>
        /// 异步加载资源，如果资源已加载，则增加引用计数并返回已加载的资源。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        Task<TAsset> LoadAssetAsync<TAsset>(string key, AssetLoadType loadType, Action<TAsset> onFinish = null) where TAsset : class => LoadAssetAsync(key, (int)loadType, onFinish);
        /// <summary>
        /// 异步加载key路径的所有资源，如果资源已加载，则增加引用计数并返回已加载的资源。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        Task<IList<TAsset>> LoadAssetAllAsync<TAsset>(string key, AssetLoadType loadType, Action<IList<TAsset>> onFinish = null, Action<TAsset> onLoadOne = null) where TAsset : class => LoadAssetAllAsync(key, (int)loadType, onFinish);

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
        /// <param name="key"></param>
        /// <param name="asset">传入asset进行判定，成果release后将asset设置为null</param>
        /// <param name="loadType"></param>
        /// <returns></returns>
        void ReleaseAsset<TAsset>(string key, ref TAsset asset, AssetLoadType loadType) where TAsset : class => ReleaseAsset(key, ref asset, (int)loadType);

        /// <summary>
        /// 释放资源对象的引用计数，如果引用计数为0，则根据卸载策略进行资源卸载
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        void ReleaseAsset(string key, int loadType = (int)AssetLoadType.Default);

        /// <summary>
        /// 释放资源对象的引用计数，如果引用计数为0，则根据卸载策略进行资源卸载
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        void ReleaseAsset(string key, AssetLoadType loadType) => ReleaseAsset(key, (int)loadType);

        /// <summary>
        /// 设置指定加载类型的卸载策略
        /// </summary>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <param name="strategy"></param>
        void SetReleaseStrategy(int loadType, AssetReleaseStrategy strategy);
        /// <summary>
        /// 设置所有加载类型的卸载策略
        /// </summary>
        /// <param name="key"></param>
        /// <param name="strategy"></param>
        void SetReleaseStrategy(AssetReleaseStrategy strategy);
        /// <summary>
        /// 设置指定加载类型的卸载策略
        /// </summary>
        /// <param name="key"></param>
        /// <param name="loadType"></param>
        /// <param name="strategy"></param>
        void SetReleaseStrategy(AssetLoadType loadType, AssetReleaseStrategy strategy) => SetReleaseStrategy((int)loadType, strategy);

        /// <summary>
        /// 手动释放所有当前引用计数为0的资源对象
        /// </summary>
        void ReleaseUnusedAssets();

    }
}