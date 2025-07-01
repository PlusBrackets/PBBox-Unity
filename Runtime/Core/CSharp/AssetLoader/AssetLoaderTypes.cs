/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.04
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    /// <summary>
    /// 资源加载器类型
    /// </summary>
    public enum AssetLoaderTypes
    {
        /// <summary>
        /// 使用默认的资源加载器类型，可在PBBoxSettings.AssetLoaderDefaultId中设置。
        /// <para>默认使用Resources加载器</para>
        /// </summary>
        Default = 0,
        /// <summary>
        /// Unity从Resources文件夹加载资源
        /// </summary>
        Resources = 1,
        Addressable = 2,
        //AssetBundle = 3,
    }

    public static partial class PBExtensions
    {

        /// <summary>
        /// 获取资源加载器类型枚举
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AssetLoaderTypes GetEnumFromLoaderType(Type type)
        {
            if (IAssetLoader.TryGetIdFromLoaderType(type, out int id))
            {
                return (AssetLoaderTypes)id;
            }
            else
            {
                return AssetLoaderTypes.Default;
            }
        }
    }
}