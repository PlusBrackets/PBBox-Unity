/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.07
 *@author: PlusBrackets
 --------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PBBox.AssetManagement
{
    internal sealed partial class AssetManager : IAssetLoaderManager
    {
        /// <summary>
        /// 加载器容器列表
        /// ?种类不多，直接用List存储
        /// </summary>
        private List<Container> m_LoaderContainers = new List<Container>();


        public Func<string, IAssetLoader> GetLoaderFactory(int loadTypeId)
        {
            var container = GetContainer(loadTypeId);
            return container?.LoaderFactory;
        }

        public void RegisterLoaderFactory(int loadTypeId, Func<string, IAssetLoader> loaderFactory)
        {
            var container = GetContainer(loadTypeId);
            if (container == null)
            {
                container = new Container
                {
                    LoaderTypeId = loadTypeId
                };
                m_LoaderContainers.Add(container);
            }
            container.LoaderFactory = loaderFactory;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Container GetContainer(int loadTypeId)
        {
            loadTypeId = GetRealLoadTypeId(loadTypeId);
            return m_LoaderContainers.Find(c => c.LoaderTypeId == loadTypeId);
        }
        
        /// <summary>
        /// 获取实际的加载器类型ID，如果传入的是默认类型ID，则返回设置好的默认加载类型Id
        /// </summary>
        /// <param name="loadTypeId"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetRealLoadTypeId(int loadTypeId)
        {
            switch (loadTypeId)
            {
                case (int)AssetLoadType.Default:
                    return PBBoxSettings.AssetLoaderDefaultId;
                default:
                    return loadTypeId;
            }
        }

        private IAssetLoader GetLoader(string key, int loadTypeId)
        {
            var container = GetContainer(loadTypeId);
            if (container == null)
            {
                Log.Error($"Loader container for type ID {loadTypeId} not found. Please register a loader factory for this type ID first.", nameof(IAssetManager), Log.PBBoxLoggerName);
                return null;
            }
            return container.GetOrCreateLoader(key);
        }

        private class Container
        {
            public int LoaderTypeId { get; set; }
            public Dictionary<string, IAssetLoader> Loaders { get; } = new Dictionary<string, IAssetLoader>();
            public Func<string, IAssetLoader> LoaderFactory { get; set; } = null;

            public IAssetLoader GetOrCreateLoader(string key)
            {
                if (!Loaders.TryGetValue(key, out var loader))
                {
                    if (LoaderFactory != null)
                    {
                        loader = LoaderFactory(key);
                        if (loader != null)
                        {
                            Loaders[key] = loader;
                        }
                        else
                        {
                            Log.Error($"Loader factory failed to create loader for key '{key}' and type ID {LoaderTypeId}.", nameof(IAssetManager), Log.PBBoxLoggerName);
                        }
                    }
                    else
                    {
                        Log.Error($"Loader factory for type ID {LoaderTypeId} is not registered.", nameof(IAssetManager), Log.PBBoxLoggerName);
                    }
                }
                return loader;
            }
        }
    }
}