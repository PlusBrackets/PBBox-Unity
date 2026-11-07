/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.07
 *@author: PlusBrackets
 --------------------------------------------------------*/

using System;

namespace PBBox.AssetManagement
{
    /// <summary>
    /// 资源加载器管理器接口
    /// </summary>
    public interface IAssetLoaderManager
    {
        /// <summary>
        /// 注册资源加载器类型，会覆盖当前类型ID的加载器工厂方法。但不会影响已存在的实际Loader
        /// </summary>
        /// <param name="loadTypeId"></param>
        /// <param name="loaderFactory"></param>
        void RegisterLoaderFactory(int loadTypeId, Func<string, IAssetLoader> loaderFactory);
        /// <summary>
        /// 获取指定类型ID的资源加载器工厂方法
        /// </summary>
        /// <param name="loadTypeId"></param>
        /// <returns></returns>
        Func<string, IAssetLoader> GetLoaderFactory(int loadTypeId);
        /// <summary>
        /// 获取指定类型ID的资源加载器
        /// </summary>
        /// <param name="key"></param>
        /// <param name="loadTypeId"></param>
        /// <returns></returns>
        IAssetLoader GetLoader(string key, int loadTypeId = (int)AssetLoadType.Default);

    }
}