/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.02
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Threading.Tasks;

namespace PBBox
{
    [System.Serializable]
    public class AssetHandle<TAsset> : IDisposable where TAsset : class
    {
        [NonSerialized]
        private TAsset m_Asset;
#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField, UnityEngine.Tooltip("资源加载器的键，一般是资源的路径。")]
#endif
        private string m_Key;

#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField, UnityEngine.Tooltip("资源加载器的类型。")]
#endif
        private AssetLoadType m_LoadType;

        /// <summary>
        /// 资源Key
        /// </summary>
        public string Key => m_Key;
        /// <summary>
        /// 资源加载类型
        /// </summary>
        public AssetLoadType LoadType => m_LoadType;
        public TAsset Asset => m_Asset;

        public AssetHandle(string key, AssetLoadType loaderTypeId = AssetLoadType.Default)
        {
            m_Key = key;
            m_Asset = null;
            m_LoadType = loaderTypeId;
        }

        public AssetHandle(string key, int loaderTypeId)
        {
            m_Key = key;
            m_Asset = null;
            m_LoadType = (AssetLoadType)loaderTypeId;
        }

        public AssetHandle(AssetHandle<TAsset> other)
        {
            m_Key = other.m_Key;
            m_Asset = null;
            m_LoadType = other.m_LoadType;
        }

        public bool IsVaild()
        {
            return !string.IsNullOrEmpty(m_Key) && m_Asset != null;
        }

        /// <summary>
        /// 加载资源，如果资源已加载，则返回已加载的资源。已加载时不会额外增加引用计数。
        /// </summary>
        /// <returns></returns>
        public TAsset GetOrLoad()
        {
            if (m_Asset == null)
            {
                m_Asset = IAssetManager.Instance.LoadAsset<TAsset>(Key, LoadType);
            }
#if PB_TEST_LOG || UNITY_EDITOR
            else
            {
                Log.Warning($"Asset '{Key}' is already loaded. Returning the cached asset.", nameof(AssetHandle<TAsset>), Log.PBBoxLoggerName);
            }
#endif
            return m_Asset;
        }

        /// <summary>
        /// 加载资源，如果资源已加载，则返回已加载的资源。已加载时不会额外增加引用计数。
        /// </summary>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        public async Task<TAsset> GetOrLoadAsync(Action<TAsset> onFinish = null)
        {
            if (m_Asset == null)
            {
                m_Asset = await IAssetManager.Instance.LoadAssetAsync<TAsset>(Key, LoadType, null);
            }
#if PB_TEST_LOG || UNITY_EDITOR
            else
            {
                Log.Warning($"Asset '{Key}' is already loaded. Returning the cached asset.", nameof(AssetHandle<TAsset>), Log.PBBoxLoggerName);
            }
#endif
            onFinish?.Invoke(m_Asset);
            return m_Asset;

        }

        public void Release()
        {
            if (IAssetManager.HasInstance())
            {
                var manager = IAssetManager.InstanceRaw;
                manager.ReleaseAsset(Key, ref m_Asset, (int)LoadType);
            }
            m_Asset = null;
        }

        public void Dispose()
        {
            Release();
            m_Key = null;
        }

        public override string ToString()
        {
            return $"AssetHandle<{nameof(TAsset)}>: Key='{Key}', LoadType='{LoadType}', Asset={(m_Asset != null ? m_Asset.ToString() : "null")}";
        }
    }
}