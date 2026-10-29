/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.02
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PBBox
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

        public TAsset GetAsset<TAsset>() where TAsset : class
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

        public IList<TAsset> GetAssets<TAsset>() where TAsset : class
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

        public async Task<TAsset> LoadAsync<TAsset>() where TAsset : class
        {
            ReferenceCount++;
            if (IsVaild())
            {
                return m_Asset as TAsset;
            }
            else if (m_LoadingTask != null)
            {
                if (m_LoadingTask is Task<TAsset> assetTask)
                {
                    return await assetTask;
                }
                else
                {
                    Log.Error($"Loading task is not of type Task<{nameof(TAsset)}>.", LogTag, Log.PBBoxLoggerName);
                }
            }
#if PB_TEST_LOG
            Log.Debug($"Starting async load for asset: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            m_LoadingTask = DoLoadAsync<TAsset>();
            m_Asset = await (m_LoadingTask as Task<TAsset>);
            m_LoadingTask = null;
#if PB_TEST_LOG
            Log.Debug($"Async asset loaded: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            return m_Asset as TAsset;
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

        public async Task<IList<TAsset>> LoadAllAsync<TAsset>() where TAsset : class
        {
            ReferenceCount++;
            if (IsVaild())
            {
                return m_Asset as IList<TAsset>;
            }
            else if (m_LoadingTask != null)
            {
                if (m_LoadingTask is Task<IList<TAsset>> assetTask)
                {
                    return await assetTask;
                }
                else
                {
                    Log.Error($"Loading task is not of type Task< IList<{nameof(TAsset)}>.", LogTag, Log.PBBoxLoggerName);
                }
            }
#if PB_TEST_LOG
            Log.Debug($"Starting async load for assets: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            m_LoadingTask = DoLoadsAsync<TAsset>();
            m_Asset = await (m_LoadingTask as Task<IList<TAsset>>);
            m_LoadingTask = null;
#if PB_TEST_LOG
            Log.Debug($"Async assets loaded: {Key}, Type: {typeof(TAsset)}", LogTag, Log.PBBoxLoggerName);
#endif
            return m_Asset as IList<TAsset>;
        }

        public void Release()
        {
            ReferenceCount--;
            ReferenceCount = Math.Max(ReferenceCount, 0);
            if (ReferenceCount <= 0)
            {
                if (m_LoadingTask != null && !m_LoadingTask.IsCompleted)
                {
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
        protected abstract Task<IList<TAsset>> DoLoadsAsync<TAsset>() where TAsset : class;
        protected abstract void OnReleaseAsset();
    }
}