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

        protected object m_Asset = null;
        protected Task m_LoadingTask = null;
        protected abstract string LogTag { get; }
        public bool IsLoading => m_LoadingTask != null;


        public AssetLoaderBase(string key)
        {
            Key = key;
        }
        
        public object GetCachedAsset()
        {
            return m_Asset;
        }

        public virtual bool IsAssetVaild()
        {
            return m_Asset != null;
        }

        public TAsset AcquireAsset<TAsset>() where TAsset : class
        {
            ReferenceCount++;
            if (IsAssetVaild())
            {
                return m_Asset as TAsset;
            }
            m_Asset = LoadImpl<TAsset>();
#if PB_TEST_LOG
            Log.Debug($"Asset loaded: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            return m_Asset as TAsset;
        }

        public async Task<TAsset> AcquireAssetAsync<TAsset>(Action<TAsset> onFinish = null) where TAsset : class
        {
            ReferenceCount++;
            TAsset asset = null;
            if (IsAssetVaild())
            {
                asset = m_Asset as TAsset;
            }
            else if (m_LoadingTask != null)
            {
                // 等待之前的加载完成
                if (m_LoadingTask is Task<TAsset> assetTask)
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
            m_LoadingTask = LoadImplAsync<TAsset>();
            asset = await (m_LoadingTask as Task<TAsset>);
            m_Asset = asset;
            m_LoadingTask = null;
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
                return m_Asset as IList<TAsset>;
            }
            m_Asset = LoadAllImpl<TAsset>();
#if PB_TEST_LOG
            Log.Debug($"Assets loaded: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            return m_Asset as IList<TAsset>;
        }

        public async Task<IList<TAsset>> AcquireAssetsAsync<TAsset>(Action<IList<TAsset>> onFinish = null, Action<TAsset> onLoadOne = null) where TAsset : class
        {
            ReferenceCount++;
            IList<TAsset> assets = null;
            if (IsAssetVaild())
            {
                assets = m_Asset as IList<TAsset>;
            }
            else if (m_LoadingTask != null)
            {
                // 等待之前的加载完成
                if (m_LoadingTask is Task<IList<TAsset>> assetTask)
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
            m_LoadingTask = LoadAllImplAsync<TAsset>(onLoadOne);
            assets = await (m_LoadingTask as Task<IList<TAsset>>);
            m_LoadingTask = null;
            m_Asset = assets;
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
                m_Asset = null;
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