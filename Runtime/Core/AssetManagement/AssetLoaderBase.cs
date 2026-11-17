/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.02
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PBBox.AssetManagement
{
    public abstract partial class AssetLoaderBase : IAssetLoader
    {
        public string Key { get; protected set; } = null;
        public int ReferenceCount { get; protected set; } = 0;
        public long LastReleaseTimeMs { get; protected set; } = 0;
        //public event Action<IAssetLoader> OnLoadCompleted;

        protected object _asset = null;
        protected Task _loadingTask = null;
        protected abstract string LogTag { get; }
        public bool IsLoading => _loadingTask != null;


        public AssetLoaderBase(string key)
        {
            Key = key;
        }
        
        public object GetCachedAsset()
        {
            return _asset;
        }

        public virtual bool IsAssetVaild()
        {
            return _asset != null;
        }

        public TAsset AcquireAsset<TAsset>() where TAsset : class
        {
            ReferenceCount++;
            if (IsAssetVaild())
            {
                return _asset as TAsset;
            }
            _asset = LoadImpl<TAsset>();
#if PB_TEST_LOG
            Log.Debug($"Asset loaded: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            return _asset as TAsset;
        }

        public async Task<TAsset> AcquireAssetAsync<TAsset>(Action<TAsset> onFinish = null) where TAsset : class
        {
            ReferenceCount++;
            TAsset asset = null;
            if (IsAssetVaild())
            {
                asset = _asset as TAsset;
            }
            else if (_loadingTask != null)
            {
                // 等待之前的加载完成
                if (_loadingTask is Task<TAsset> assetTask)
                {
                    asset = await assetTask;
                }
                else
                {
                    Log.Error($"Loading task is not of type Task<{nameof(TAsset)}>.", LogTag, Log.PBBoxLoggerName);
                }
            }
            if (asset != null)
            {
                onFinish?.Invoke(asset);
                return asset;
            }
            // 开始加载
#if PB_TEST_LOG
            Log.Debug($"Starting async load for asset: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            _loadingTask = LoadImplAsync<TAsset>();
            asset = await (_loadingTask as Task<TAsset>);
            _asset = asset;
            _loadingTask = null;
#if PB_TEST_LOG
            Log.Debug($"Async asset loaded: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            onFinish?.Invoke(asset);
            return asset;
        }

        public IList<TAsset> AcquireAssets<TAsset>() where TAsset : class
        {
            ReferenceCount++;
            if (IsAssetVaild())
            {
                return _asset as IList<TAsset>;
            }
            _asset = LoadAllImpl<TAsset>();
#if PB_TEST_LOG
            Log.Debug($"Assets loaded: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            return _asset as IList<TAsset>;
        }

        public async Task<IList<TAsset>> AcquireAssetsAsync<TAsset>(Action<IList<TAsset>> onFinish = null, Action<TAsset> onLoadOne = null) where TAsset : class
        {
            ReferenceCount++;
            IList<TAsset> assets = null;
            if (IsAssetVaild())
            {
                assets = _asset as IList<TAsset>;
            }
            else if (_loadingTask != null)
            {
                // 等待之前的加载完成
                if (_loadingTask is Task<IList<TAsset>> assetTask)
                {
                    assets = await assetTask;
                }
                else
                {
                    Log.Error($"Loading task is not of type Task< IList<{nameof(TAsset)}>.", LogTag, Log.PBBoxLoggerName);
                }
            }
            if (assets != null)
            {
                foreach (var asset in assets)
                {
                    onLoadOne?.Invoke(asset);
                }
                onFinish?.Invoke(assets);
                return assets;
            }
            // 开始加载
#if PB_TEST_LOG
            Log.Debug($"Starting async load for assets: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            _loadingTask = LoadAllImplAsync<TAsset>(onLoadOne);
            assets = await (_loadingTask as Task<IList<TAsset>>);
            _loadingTask = null;
            _asset = assets;
#if PB_TEST_LOG
            Log.Debug($"Async assets loaded: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            onFinish?.Invoke(assets);
            return assets;
        }

        public void ReleaseAsset()
        {
            ReferenceCount--;
            ReferenceCount = Math.Max(ReferenceCount, 0);
            LastReleaseTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            //            if (ReferenceCount <= 0)
            //            {
            //                if (m_LoadingTask != null && !m_LoadingTask.IsCompleted)
            //                {
            //                    //TODO 有问题，后续在AssetManager实现按释放策略时进行处理
            //                    ReferenceCount = 1; // 将引用计数恢复为1，等待加载完成后再释放
            //                    m_LoadingTask.ContinueWith(t =>
            //                    {
            //                        ReleaseReference();
            //                    });
            //                }
            //                else
            //                {
            //#if PB_TEST_LOG
            //                    Log.Debug($"Asset released: {Key}", LogTag, Log.PBBoxLoggerName);
            //#endif
            //                    ReleaseAssetImpl();
            //                    m_Asset = null;
            //                }
            //            }
        }

        public bool TryUnloadAsset()
        {
            if (ReferenceCount > 0)
            {
                Log.Error($"Cannot release asset '{Key}' because its reference count is greater than zero.", LogTag, Log.PBBoxLoggerName);
                return false;
            }
            if (IsLoading)
            {
                Log.Error($"Asset '{Key}' is still loading. Cannot release it now.", LogTag, Log.PBBoxLoggerName);
                return false;
            }
            if (UnloadImpl())
            {
                _asset = null;
#if PB_TEST_LOG || UNITY_EDITOR
                Log.Debug($"Asset unloaded: {Key}", LogTag, Log.PBBoxLoggerName);
#endif
                return true;
            }
            return false;
        }

        protected abstract TAsset LoadImpl<TAsset>() where TAsset : class;
        protected abstract Task<TAsset> LoadImplAsync<TAsset>() where TAsset : class;
        protected abstract IList<TAsset> LoadAllImpl<TAsset>() where TAsset : class;
        protected abstract Task<IList<TAsset>> LoadAllImplAsync<TAsset>(Action<TAsset> onLoadOne) where TAsset : class;
        protected abstract bool UnloadImpl();
    }
}