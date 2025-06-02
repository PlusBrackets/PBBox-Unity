/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.01
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PBBox
{

    public interface IAssetLoader
    {
        string Key { get; }
        int ReferenceCount { get; }
        //event Action<IAssetLoader> OnLoadCompleted;

        bool IsVaild();

        /// <summary>
        /// 直接获取资源对象，不会加载资源和增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <returns></returns>
        TAsset GetAsset<TAsset>() where TAsset : class;
        /// <summary>
        /// 直接获取资源对象数组，不会加载资源和增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <returns></returns>
        IList<TAsset> GetAssets<TAsset>() where TAsset : class;

        /// <summary>
        /// 加载一次资源对象，并增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <returns></returns>
        TAsset Load<TAsset>()  where TAsset : class;
        /// <summary>
        /// 异步加载一次资源对象，并增加引用计数。
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <returns></returns>
        Task<TAsset> LoadAsync<TAsset>() where TAsset : class;
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
        Task<IList<TAsset>> LoadAllAsync<TAsset>() where TAsset : class;

        /// <summary>
        /// 释放资源对象的引用计数
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="asset"></param>
        void Release();
    }

}