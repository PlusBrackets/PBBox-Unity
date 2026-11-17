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
        private TAsset _asset;
#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField, UnityEngine.Tooltip("资源加载器的键，一般是资源的路径。")]
#endif
        private string _key;

#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField, UnityEngine.Tooltip("资源加载器的类型。")]
#endif
        private AssetLoadType _loadType;

        /// <summary>
        /// 资源Key
        /// </summary>
        public string Key => _key;
        /// <summary>
        /// 资源加载类型
        /// </summary>
        public AssetLoadType LoadType => _loadType;
        public TAsset Asset => _asset;

        public AssetHandle(string key, AssetLoadType loaderTypeId = AssetLoadType.Default)
        {
            _key = key;
            _asset = null;
            _loadType = loaderTypeId;
        }

        public AssetHandle(string key, int loaderTypeId)
        {
            _key = key;
            _asset = null;
            _loadType = (AssetLoadType)loaderTypeId;
        }

        public AssetHandle(AssetHandle<TAsset> other)
        {
            _key = other._key;
            _asset = null;
            _loadType = other._loadType;
        }

        public bool IsVaild()
        {
            return !string.IsNullOrEmpty(_key) && _asset != null;
        }

        /// <summary>
        /// 加载资源，如果资源已加载，则返回已加载的资源。已加载时不会额外增加引用计数。
        /// </summary>
        /// <returns></returns>
        public TAsset GetOrLoad()
        {
            if (_asset == null)
            {
                _asset = IAssetManager.Instance.LoadAsset<TAsset>(Key, LoadType);
            }
#if PB_TEST_LOG || UNITY_EDITOR
            else
            {
                Log.Warning($"Asset '{Key}' is already loaded. Returning the cached asset.", nameof(AssetHandle<TAsset>), Log.PBBoxLoggerName);
            }
#endif
            return _asset;
        }

        /// <summary>
        /// 加载资源，如果资源已加载，则返回已加载的资源。已加载时不会额外增加引用计数。
        /// </summary>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        public async Task<TAsset> GetOrLoadAsync(Action<TAsset> onFinish = null)
        {
            if (_asset == null)
            {
                _asset = await IAssetManager.Instance.LoadAssetAsync<TAsset>(Key, LoadType, null);
            }
#if PB_TEST_LOG || UNITY_EDITOR
            else
            {
                Log.Warning($"Asset '{Key}' is already loaded. Returning the cached asset.", nameof(AssetHandle<TAsset>), Log.PBBoxLoggerName);
            }
#endif
            onFinish?.Invoke(_asset);
            return _asset;

        }

        public void Release()
        {
            if (IAssetManager.HasInstance())
            {
                var manager = IAssetManager.InstanceRaw;
                manager.ReleaseAsset(Key, ref _asset, (int)LoadType);
            }
            _asset = null;
        }

        public void Dispose()
        {
            Release();
            _key = null;
        }

        public override string ToString()
        {
            return $"AssetHandle<{nameof(TAsset)}>: Key='{Key}', LoadType='{LoadType}', Asset={(_asset != null ? _asset.ToString() : "null")}";
        }
    }
}