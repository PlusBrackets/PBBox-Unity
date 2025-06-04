/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace PBBox
{
    [SingletonPriority(-1)]
    public sealed partial class AssetManager2 : IAssetManager
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
            //        Log.Error($"Invalid loader type ID: {value}.", "AssetManager", Log.PBBoxLoggerName);
            //        return;
            //    }
            //    m_DefaultLoaderTypeId = value;
            //}
        }

        /// <summary>
        /// 注册自定义加载器工厂。
        /// </summary>
        /// <param name="loaderType"></param>
        /// <param name="factory"></param>
        private void RegisterCustomLoaderFactory(Type loaderType, Func<string, IAssetLoader> factory)
        {
            if (m_LoaderContainers.Value.TryGetValue(loaderType, out LoadersContainer container))
            {
                Log.Warning($"Custom loader factory for {loaderType.Name} already registered.", "AssetManager", Log.PBBoxLoggerName);
                container.SetLoaderFactory(factory, loaderType);
            }
            else
            {
                container = new LoadersContainer(factory, loaderType);
                m_LoaderContainers.Value[loaderType] = container;
#if PB_TEST_LOG
                Log.Info($"Custom loader factory for {loaderType.Name} registered.", "AssetManager", Log.PBBoxLoggerName);
#endif
            }
        }

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
                Log.Error($"Invalid loader type ID: {loaderTypeId}.", "AssetManager", Log.PBBoxLoggerName);
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
                Log.Error("Default loader type is not set.", "AssetManager", Log.PBBoxLoggerName);
                return null;
            }

            if (string.IsNullOrEmpty(key))
            {
                Log.Error("AssetLoader key cannot be null or empty.", "AssetManager", Log.PBBoxLoggerName);
                return null;
            }
            if (!m_LoaderContainers.IsValueCreated || !m_LoaderContainers.Value.ContainsKey(loaderType))
            {
                RegisterCustomLoaderFactory(loaderType, null);
            }
            var container = m_LoaderContainers.Value[loaderType];
            if (container == null)
            {
                Log.Error($"No loader container found for type {loaderType.Name}.", "AssetManager", Log.PBBoxLoggerName);
                return null;
            }
            return container.GetLoader(key);
        }


        /// <summary>
        /// 容器类，用于存储和管理加载器实例。
        /// </summary>
        private class LoadersContainer
        {
            private Func<string, IAssetLoader> m_Factory = null;
            private Dictionary<string, IAssetLoader> m_Loaders = null;
            private Type m_LoaderType = null;

            public LoadersContainer(Func<string, IAssetLoader> factory, Type loaderType)
            {
                m_Factory = factory;
                m_LoaderType = loaderType;
                TryRegisterCreatorFunc();
            }

            public void SetLoaderFactory(Func<string, IAssetLoader> factory, Type loaderType)
            {
                m_Factory = factory;
                m_LoaderType = loaderType;
                TryRegisterCreatorFunc();
            }

            private void TryRegisterCreatorFunc()
            {
                if (m_Factory != null)
                {
                    return;
                }
                var method = m_LoaderType.GetMethods(System.Reflection.BindingFlags.Static).FirstOrDefault(m => m.IsDefined(typeof(AssetLoaderCreateMethodAttribute), false));
                if (method != null)
                {
                    m_Factory = method.CreateDelegate(typeof(Func<string, IAssetLoader>)) as Func<string, IAssetLoader>;
                }
#if PB_TEST_LOG
                if (m_Factory != null)
                {
                    Log.Debug($"Custom loader factory for {m_LoaderType.Name} registered via method {method.Name}.", "AssetManager", Log.PBBoxLoggerName);
                }
                else
#endif
                if (method != null)
                {
                    Log.Error($"Failed to create loader factory for {m_LoaderType.Name}. Method {method.Name} does not match Func<string, IAssetLoader> signature.", "AssetManager", Log.PBBoxLoggerName);
                }
            }

            public IAssetLoader GetLoader(string key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    Log.Error("AssetLoader key cannot be null or empty.", "AssetManager", Log.PBBoxLoggerName);
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
                            loader = Activator.CreateInstance(m_LoaderType, key) as IAssetLoader;
                        }
                        else
                        {
                            loader = m_Factory(key);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Failed to create loader for key '{key}': {ex.Message}", "AssetManager", Log.PBBoxLoggerName);
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