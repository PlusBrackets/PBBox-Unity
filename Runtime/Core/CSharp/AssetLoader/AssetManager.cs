///*--------------------------------------------------------
// *Copyright (c) 2016-2025 PlusBrackets
// *@update: 2025.06.03
// *@author: PlusBrackets
// --------------------------------------------------------*/
//using System;
//using System.Collections.Generic;
//
//namespace PBBox
//{
//    [SingletonPriority(-1)]
//    public partial class AssetManager2 : IAssetManager
//    {
//        private class LoadersContainer
//        {
//            public Func<string, IAssetLoader> LoaderFactory = null;
//            public Dictionary<string, IAssetLoader> Loaders = new Dictionary<string, IAssetLoader>();
//        }
//
//        protected Type m_FallbackLoaderType = null;
//        protected Dictionary<
//
//        public void SetFallbackLoaderFactory<TLoader>() where TLoader : IAssetLoader
//        {
//            if (m_FallbackLoaderType != null)
//            {
//                Log.Warning($"Fallback loader factory already set to {m_FallbackLoaderType.Name}.", "AssetManager", Log.PBBoxLoggerName);
//            }
//            m_FallbackLoaderType = typeof(TLoader);
//            Log.Info($"Fallback loader factory set to {m_FallbackLoaderType.Name}.", "AssetManager", Log.PBBoxLoggerName);
//        }
//
//        public void RegisterCustomLoaderFactory<TLoader>(Func<string, IAssetLoader> factory) where TLoader : IAssetLoader
//        {
//            if (factory == null)
//            {
//                Log.Error("Custom loader factory cannot be null.", "AssetManager", Log.PBBoxLoggerName);
//                return;
//            }
//            var type = typeof(TLoader);
//            if (!m_CustomLoaderFactories.TryAdd(type, factory))
//            {
//                Log.Warning($"Custom loader factory for {type.Name} already registered.", "AssetManager", Log.PBBoxLoggerName);
//            }
//            m_CustomLoaderFactories[type] = factory;
//            Log.Info($"Custom loader factory for {type.Name} registered.", "AssetManager", Log.PBBoxLoggerName);
//        }
//
//        public void UnregisterCustomLoaderFactory<TLoader>() where TLoader : IAssetLoader
//        {
//            var type = typeof(TLoader);
//            if (m_CustomLoaderFactories.Remove(type))
//            {
//                Log.Info($"Custom loader factory for {type.Name} unregistered.", "AssetManager", Log.PBBoxLoggerName);
//            }
//            else
//            {
//                Log.Warning($"Custom loader factory for {type.Name} not found.", "AssetManager", Log.PBBoxLoggerName);
//            }
//        }
//
//        public TLoader GetLoader<TLoader>(string key) where TLoader : IAssetLoader
//        {
//            if (string.IsNullOrEmpty(key))
//            {
//                Log.Error("AssetLoader key cannot be null or empty.", "AssetManager", Log.PBBoxLoggerName);
//                return default(TLoader);
//            }
//            if (!m_Loaders.Value.TryGetValue(key, out IAssetLoader loader))
//            {
//
//            }
//            return (TLoader)loader;
//
//        }
//
//        IAssetLoader IAssetManager.GetLoader(string key, int loaderType = 0)
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}