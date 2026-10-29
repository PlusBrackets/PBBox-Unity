/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.04
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{

    /// <summary>
    /// 资源加载器特性，用于初始化时注册为对应类型Id的加载器，如果有相同则按优先级处理，必须继承IAssetLoader
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class AssetLoaderAssignAttribute : Attribute
    {
        public readonly int LoaderTypeId;
        public readonly int Priority;
        public AssetLoaderAssignAttribute(int loaderTypeId, int priority = 0)
        {
            LoaderTypeId = loaderTypeId;
            Priority = priority;
        }

        public AssetLoaderAssignAttribute(AssetLoaderTypes loaderType, int priority = 0)
        {
            LoaderTypeId = (int)loaderType;
            Priority = priority;
        }
    }
}