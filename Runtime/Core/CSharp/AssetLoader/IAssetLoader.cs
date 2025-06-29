/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.01
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        TAsset Load<TAsset>() where TAsset : class;
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

        /// <summary>
        /// 加载器类型Map，(类型Id, (优先级, 加载器类型))
        /// </summary>
        private static readonly Lazy<Dictionary<int, (int priority, Type loaderType)>> m_LoaderTypeMap = new Lazy<Dictionary<int, (int, Type)>>(CreateLoaderTypeMap);

        internal static void WarmupLoaderTypeMap()
        {
            // 预热加载器类型Map，确保在第一次使用时不会有性能损耗
            var _ = m_LoaderTypeMap.Value;
        }

        private static Dictionary<int, (int, Type)> CreateLoaderTypeMap()
        {
            var map = new Dictionary<int, (int priority, Type)>();
            //获取所有实现了IAssetLoader接口，且有AssetLoaderAssign特性的具体类
            var assemblyNames = PBBoxSettings.CommonInitReflectAssemblies.Union(PBBoxSettings.InitReflectAssemblies_AssetLoader);
            var types = typeof(IAssetLoader).GetAllChildClassWithAttribute<AssetLoaderAssignAttribute>(false, true, assemblyNames: assemblyNames);
            foreach (var type in types)
            {
                int priority = 0;
                var attr = type.GetCustomAttribute<AssetLoaderAssignAttribute>(false);
                if (attr != null)
                {
                    priority = attr.Priority;
                }
                if (map.TryGetValue(attr.LoaderTypeId, out var oldInfo))
                {
                    //如果已经存在，则比较优先级，取优先级高的
                    if (oldInfo.priority < priority)
                    {
                        map[attr.LoaderTypeId] = (priority, type);
                    }
                }
                else
                {
                    map.Add(attr.LoaderTypeId, (priority, type));
                }
            }
            return map;
        }

        /// <summary>
        /// 根据加载器类型Id获取对应的加载器类型
        /// </summary>
        /// <param name="loaderTypeId"></param>
        /// <returns></returns>
        public static Type GetLoaderTypeFromId(int loaderTypeId)
        {
            if (m_LoaderTypeMap.Value.TryGetValue(loaderTypeId, out var loaderInfo))
            {
                return loaderInfo.loaderType;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据加载器类型Id获取对应的加载器类型
        /// </summary>
        /// <param name="loaderType"></param>
        /// <returns></returns>
        public static Type GetLoaderTypeFromId(AssetLoaderTypes loaderType)
        {
            return GetLoaderTypeFromId((int)loaderType);
        }

        /// <summary>
        /// 根据加载器类型获取对应的加载器类型Id
        /// </summary>
        /// <param name="loaderType"></param>
        /// <param name="loaderTypeId"></param>
        /// <returns></returns>
        public static bool TryGetIdFromLoaderType(Type loaderType, out int loaderTypeId)
        {
            if (m_LoaderTypeMap.Value.Any(kv => kv.Value.loaderType == loaderType))
            {
                loaderTypeId = m_LoaderTypeMap.Value.First(kv => kv.Value.loaderType == loaderType).Key;
                return true;
            }
            else
            {
                loaderTypeId = 0;
                return false;
            }
        }
    }
}