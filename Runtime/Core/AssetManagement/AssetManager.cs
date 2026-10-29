/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PBBox
{
    [SingletonPriority(-1)]
    internal sealed partial class AssetManager2 : IAssetManager, ISingletonLifecycle
    {
        private Lazy<Dictionary<Type, LoadersContainer>> m_LoaderContainers = new Lazy<Dictionary<Type, LoadersContainer>>();

        public Type DefaultLoaderType
        {
            get => IAssetLoader.GetLoaderTypeFromId(PBBoxSettings.AssetLoaderDefaultId);
        }

        public int DefaultLoaderTypeId
        {
            get => PBBoxSettings.AssetLoaderDefaultId;
            //set
            //{
            //    if (IAssetLoader.GetLoaderTypeFromId(value) == null)
            //    {
            //        Log.Error($"Invalid loader type ID: {value}.", nameof(IAssetManager), Log.PBBoxLoggerName);
            //        return;
            //    }
            //    m_DefaultLoaderTypeId = value;
            //}
        }

//        /// <summary>
//        /// 注册自定义加载器工厂。
//        /// </summary>
//        /// <param name="loaderType"></param>
//        /// <param name="factory"></param>
//        private void RegisterCustomLoaderFactory(Type loaderType, Func<string, IAssetLoader> factory)
//        {
//            if (m_LoaderContainers.Value.TryGetValue(loaderType, out LoadersContainer container))
//            {
//                Log.Warning($"Custom loader factory for {loaderType.Name} already registered.", nameof(IAssetManager), Log.PBBoxLoggerName);
//                container.SetLoaderFactory(factory, loaderType);
//            }
//            else
//            {
//                container = new LoadersContainer(factory, loaderType);
//                m_LoaderContainers.Value[loaderType] = container;
//#if PB_TEST_LOG
//                Log.Info($"Custom loader factory for {loaderType.Name} registered.", nameof(IAssetManager), Log.PBBoxLoggerName);
//#endif
//            }
//        }

        public TLoader GetLoader<TLoader>(string key) where TLoader : IAssetLoader
        {
            var loaderType = typeof(TLoader);
            return (TLoader)GetLoader(key, loaderType);
        }

        public IAssetLoader GetLoader(string key, int loaderTypeId)
        {
            var loaderType = IAssetLoader.GetLoaderTypeFromId(loaderTypeId);
            if (loaderType == null)
            {
                Log.Error($"Invalid loader type ID: {loaderTypeId}.", nameof(IAssetManager), Log.PBBoxLoggerName);
                return null;
            }
            return GetLoader(key, loaderType);
        }

        public IAssetLoader GetLoader(string key, Type loaderType = null)
        {
            if (loaderType == null)
            {
                loaderType = DefaultLoaderType;
            }
            if (loaderType == null)
            {
                Log.Error("Default loader type is not set.", nameof(IAssetManager), Log.PBBoxLoggerName);
                return null;
            }

            if (string.IsNullOrEmpty(key))
            {
                Log.Error("AssetLoader key cannot be null or empty.", nameof(IAssetManager), Log.PBBoxLoggerName);
                return null;
            }
            if (!m_LoaderContainers.Value.TryGetValue(loaderType, out LoadersContainer container))
            {
                container = new LoadersContainer(loaderType);
                m_LoaderContainers.Value[loaderType] = container;
            }
            //if (!m_LoaderContainers.IsValueCreated || !m_LoaderContainers.Value.ContainsKey(loaderType))
            //{
            //    RegisterCustomLoaderFactory(loaderType, null);
            //}
            //var container = m_LoaderContainers.Value[loaderType];
            if (container == null)
            {
                Log.Error($"No loader container found for type {loaderType.Name}.", nameof(IAssetManager), Log.PBBoxLoggerName);
                return null;
            }
            return container.GetLoader(key);
        }

        public void OnCreateAsSingleton()
        {
            // 预热加载器类型Map，确保在第一次使用时不会有性能损耗
            IAssetLoader.WarmupLoaderTypeMap();
        }

        public void OnDestroyAsSingleton()
        {

        }


        /// <summary>
        /// 容器类，用于存储和管理加载器实例。
        /// </summary>
        private class LoadersContainer
        {
            private readonly Type m_LoaderType = null;
            private Dictionary<string, IAssetLoader> m_Loaders = null;
            private Func<string, IAssetLoader> m_Factory = null;

            public LoadersContainer(Type loaderType)
            {
                m_LoaderType = loaderType;
                TryRegisterCreatorFunc();
            }

            //public void SetLoaderFactory(Func<string, IAssetLoader> factory)
            //{
            //    m_Factory = factory;
            //    TryRegisterCreatorFunc();
            //}

            private void TryRegisterCreatorFunc()
            {
                if (m_Factory != null)
                {
                    return;
                }
                // new AssetLoader(string key)的构造函数
                var constructor = m_LoaderType.GetConstructor(new[] { typeof(string) });
                if (constructor == null)
                {
                    Log.Error($"No constructor found for loader type {m_LoaderType.Name} with string parameter. Cannot create loader instances.", nameof(IAssetManager), Log.PBBoxLoggerName);
                    return;
                }
                var keyParam = Expression.Parameter(typeof(string), "key");
                var newExpression = Expression.New(constructor, keyParam);
                m_Factory = Expression.Lambda<Func<string, IAssetLoader>>(newExpression, keyParam).Compile();
#if PB_TEST_LOG
                Log.Debug($"Loader factory for {m_LoaderType.Name} registered successfully.", nameof(IAssetManager), Log.PBBoxLoggerName);
#endif
            }

            public IAssetLoader GetLoader(string key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    Log.Error("AssetLoader key cannot be null or empty.", nameof(IAssetManager), Log.PBBoxLoggerName);
                    return null;
                }
                if (m_Loaders == null)
                {
                    m_Loaders = new Dictionary<string, IAssetLoader>();
                }
                if (!m_Loaders.TryGetValue(key, out IAssetLoader loader))
                {
                    try
                    {
                        if (m_Factory == null)
                        {
                            Log.Warning($"No factory registered for loader type {m_LoaderType.Name}. Attempting to create instance directly.", nameof(IAssetManager), Log.PBBoxLoggerName);
                            loader = Activator.CreateInstance(m_LoaderType, key) as IAssetLoader;
                        }
                        else
                        {
                            loader = m_Factory(key);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Failed to create loader for key '{key}': {ex.Message}", nameof(IAssetManager), Log.PBBoxLoggerName);
                        return null;
                    }
                    if (loader != null)
                    {
                        m_Loaders[key] = loader;
                    }
                }
                return loader;
            }
        }
    }
}