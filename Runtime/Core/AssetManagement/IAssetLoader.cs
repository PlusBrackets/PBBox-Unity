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
        /// 资源是否有效
        /// </summary>
        /// <returns></returns>
        bool IsVaild();

        /// <summary>
        /// 直接获取资源对象，不会加载资源和增加引用计数。
        /// </summary>
        /// <returns></returns>
        object GetAssetDirect();
        /// <summary>
        /// 直接获取资源对象，不会加载资源和增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <returns></returns>
        TAsset GetAssetDirect<TAsset>() where TAsset : class;
        /// <summary>
        /// 直接获取资源对象数组，不会加载资源和增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <returns></returns>
        IList<TAsset> GetAssetsDirect<TAsset>() where TAsset : class;

        /// <summary>
        /// 加载一次资源对象，并增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <returns></returns>
        TAsset Load<TAsset>() where TAsset : class;
        /// <summary>
        /// 异步加载一次资源对象，并增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <returns></returns>
        Task<TAsset> LoadAsync<TAsset>(Action<TAsset> onLoadFinish) where TAsset : class;
        /// <summary>
        /// 加载一次资源对象，并增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="callback"></param>
        /// <returns></returns>
        IList<TAsset> LoadAll<TAsset>() where TAsset : class;
        /// <summary>
        /// 加载所有资源对象，并增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="callback"></param>
        /// <returns></returns>
        Task<IList<TAsset>> LoadAllAsync<TAsset>(Action<IList<TAsset>> onLoadFinish, Action<TAsset> onLoadOne) where TAsset : class;

        /// <summary>
        /// 释放资源对象的引用计数
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="asset"></param>
        void Release();

    }
}