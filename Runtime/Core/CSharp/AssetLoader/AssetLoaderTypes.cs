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
        Default = 0,
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