///*--------------------------------------------------------
// *Copyright (c) 2016-2025 PlusBrackets
// *@update: 2025.06.02
// *@author: PlusBrackets
// --------------------------------------------------------*/
//using System;
//
//namespace PBBox
//{
//    public struct AssetLoadHandle<TLoader, TObject> : IDisposable where TObject : class where TLoader : IAssetLoader
//    {
//        private IAssetLoader m_Loader;
//        public string Key { get; private set; }
//        public TObject Asset => m_Loader?.GetAsset<TObject>();
//        public TObject[] Assets => m_Loader?.GetAssets<TObject>();
//
//        public AssetLoadHandle(string key)
//        {
//            Key = key;
//            m_Loader = null;
//        }
//
//        private IAssetLoader GetAssetLoader()
//        {
//            if (m_Loader != null)
//            {
//                return m_Loader;
//            }
//            var manager = IAssetManager.Instance;
//            if (manager == null)
//            {
//                Log.Error("AssetManager is not initialized.", "AssetLoadHandle", Log.PBBoxLoggerName);
//                return null;
//            }
//            m_Loader = manager.GetLoader<TLoader>(Key);
//            if (m_Loader == null)
//            {
//                Log.Error($"AssetLoader '{Key}' not found.", "AssetLoadHandle", Log.PBBoxLoggerName);
//                return null;
//            }
//            return m_Loader;
//        }
//
//        public void Release()
//        {
//            if (m_Loader == null)
//            {
//                return;
//            }
//            m_Loader.Release();
//        }
//
//        public void Dispose()
//        {
//            Release();
//        }
//    }
//}