/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.02
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{

    public interface IAssetManager : ISingleton<IAssetManager>
    {
        /// <summary>
        /// 获得指定类型的加载器
        /// </summary>
        /// <typeparam name="TLoader"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        TLoader GetLoader<TLoader>(string key) where TLoader : IAssetLoader;
        /// <summary>
        /// 获得指定类型的加载器，如果未指定类型，则使用默认加载器类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="loaderType"></param>
        /// <returns></returns>
        IAssetLoader GetLoader(string key, Type loaderType = null);
        /// <summary>
        /// 获得指定类型ID的加载器
        /// </summary>
        /// <param name="key"></param>
        /// <param name="loaderTypeId"></param>
        /// <returns></returns>
        IAssetLoader GetLoader(string key, int loaderTypeId);
    }
}