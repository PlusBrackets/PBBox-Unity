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
        //public event Action<IAssetLoader> OnLoadCompleted;

        protected object m_Asset = null;
        protected Task m_LoadingTask = null;
        protected abstract string LogTag { get; }


        public AssetLoaderBase(string key)
        {
            Key = key;
        }
        
        public object GetAssetDirect()
        {
            return m_Asset;
        }

        public TAsset GetAssetDirect<TAsset>() where TAsset : class
        {
            if (m_Asset is TAsset typedAsset)
            {
                return typedAsset;
            }
            else if (m_Asset is IList<TAsset> typedAssets && typedAssets.Count > 0)
            {
                return typedAssets[0];
            }
            return null;
        }

        public IList<TAsset> GetAssetsDirect<TAsset>() where TAsset : class
        {
            if (m_Asset is IList<TAsset> typedAssets)
            {
                return typedAssets;
            }
            return null;
        }

        public virtual bool IsVaild()
        {
            return m_Asset != null;
        }

        public TAsset Load<TAsset>() where TAsset : class
        {
            ReferenceCount++;
            if (IsVaild())
            {
                return m_Asset as TAsset;
            }
            m_Asset = DoLoad<TAsset>();
#if PB_TEST_LOG
            Log.Debug($"Asset loaded: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            return m_Asset as TAsset;
        }

        public async Task<TAsset> LoadAsync<TAsset>(Action<TAsset> onFinish = null) where TAsset : class
        {
            ReferenceCount++;
            TAsset asset = null;
            if (IsVaild())
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
            m_LoadingTask = DoLoadAsync<TAsset>();
            asset = await (m_LoadingTask as Task<TAsset>);
            m_Asset = asset;
            m_LoadingTask = null;
#if PB_TEST_LOG
            Log.Debug($"Async asset loaded: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            onFinish?.Invoke(asset);
            return asset;
        }

        public IList<TAsset> LoadAll<TAsset>() where TAsset : class
        {
            ReferenceCount++;
            if (IsVaild())
            {
                return m_Asset as IList<TAsset>;
            }
            m_Asset = DoLoads<TAsset>();
#if PB_TEST_LOG
            Log.Debug($"Assets loaded: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            return m_Asset as IList<TAsset>;
        }

        public async Task<IList<TAsset>> LoadAllAsync<TAsset>(Action<IList<TAsset>> onFinish = null, Action<TAsset> onLoadOne = null) where TAsset : class
        {
            ReferenceCount++;
            IList<TAsset> assets = null;
            if (IsVaild())
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
            m_LoadingTask = DoLoadsAsync<TAsset>(onLoadOne);
            assets = await (m_LoadingTask as Task<IList<TAsset>>);
            m_LoadingTask = null;
            m_Asset = assets;
#if PB_TEST_LOG
            Log.Debug($"Async assets loaded: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            onFinish?.Invoke(assets);
            return assets;
        }

        public void Release()
        {
            ReferenceCount--;
            ReferenceCount = Math.Max(ReferenceCount, 0);
            if (ReferenceCount <= 0)
            {
                if (m_LoadingTask != null && !m_LoadingTask.IsCompleted)
                {
                    //TODO 有问题，后续在AssetManager实现按释放策略时进行处理
                    ReferenceCount = 1; // 将引用计数恢复为1，等待加载完成后再释放
                    m_LoadingTask.ContinueWith(t =>
                    {
                        Release();
                    });
                }
                else
                {
#if PB_TEST_LOG
                    Log.Debug($"Asset released: {Key}", LogTag, Log.PBBoxLoggerName);
#endif
                    OnReleaseAsset();
                    m_Asset = null;
                }
            }
        }

        protected abstract TAsset DoLoad<TAsset>() where TAsset : class;
        protected abstract Task<TAsset> DoLoadAsync<TAsset>() where TAsset : class;
        protected abstract IList<TAsset> DoLoads<TAsset>() where TAsset : class;
        protected abstract Task<IList<TAsset>> DoLoadsAsync<TAsset>(Action<TAsset> onLoadOne) where TAsset : class;
        protected abstract void OnReleaseAsset();
    }
}