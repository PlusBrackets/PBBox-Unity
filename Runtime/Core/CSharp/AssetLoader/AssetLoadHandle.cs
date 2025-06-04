/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.02
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PBBox
{
    [System.Serializable]
    public partial struct AssetLoadHandle<TObject> : IDisposable, IAssetLoader where TObject : class
    {
        [NonSerialized]
        private IAssetLoader m_Loader;
#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField, UnityEngine.Tooltip("资源加载器的键，一般是资源的路径。")]
#endif
        private string m_Key;

#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField, UnityEngine.Tooltip("资源加载器的类型。")]
#endif
        private int m_LoaderTypeId;//TODO 编辑器GUI
        /// <summary>
        /// 资源加载器的键，一般是资源的路径。
        /// </summary>
        public string Key => m_Key;
        /// <summary>
        /// 引用计数
        /// </summary>
        public int ReferenceCount => m_Loader != null ? m_Loader.ReferenceCount : 0;

        public AssetLoadHandle(string key, AssetLoaderTypes loaderTypeId = AssetLoaderTypes.Default)
        {
            m_Key = key;
            m_Loader = null;
            m_LoaderTypeId = (int)loaderTypeId;
        }

        public AssetLoadHandle(string key, int loaderTypeId)
        {
            m_Key = key;
            m_Loader = null;
            m_LoaderTypeId = loaderTypeId;
        }

        public IAssetLoader GetAssetLoader()
        {
            if (m_Loader != null)
            {
                return m_Loader;
            }
            var manager = IAssetManager.Instance;
            if (manager == null)
            {
                Log.Error("AssetManager is not initialized.", "AssetLoadHandle", Log.PBBoxLoggerName);
                return null;
            }
            m_Loader = manager.GetLoader(Key, (int)m_LoaderTypeId);
            if (m_Loader == null)
            {
                Log.Error($"AssetLoader '{Key}' not found.", "AssetLoadHandle", Log.PBBoxLoggerName);
                return null;
            }
            return m_Loader;
        }

        public void Release()
        {
            if (m_Loader == null)
            {
                return;
            }
            m_Loader.Release();
        }

        public void Dispose()
        {
            Release();
        }

        public bool IsVaild()
        {
            return m_Loader != null && m_Loader.IsVaild();
        }

        public TAsset GetAsset<TAsset>() where TAsset : class
        {
            return GetAssetLoader()?.GetAsset<TAsset>();
        }

        public IList<TAsset> GetAssets<TAsset>() where TAsset : class
        {
            return GetAssetLoader()?.GetAssets<TAsset>();
        }

        public TAsset Load<TAsset>() where TAsset : class
        {
            var loader = GetAssetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.Load<TAsset>();
        }

        public Task<TAsset> LoadAsync<TAsset>() where TAsset : class
        {
            var loader = GetAssetLoader();
            if (loader == null)
            {
                return Task.FromResult<TAsset>(null);
            }
            return loader.LoadAsync<TAsset>();
        }

        public IList<TAsset> LoadAll<TAsset>() where TAsset : class
        {
            var loader = GetAssetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.LoadAll<TAsset>();
        }

        public Task<IList<TAsset>> LoadAllAsync<TAsset>() where TAsset : class
        {
            var loader = GetAssetLoader();
            if (loader == null)
            {
                return Task.FromResult<IList<TAsset>>(null);
            }
            return loader.LoadAllAsync<TAsset>();
        }
    }
}