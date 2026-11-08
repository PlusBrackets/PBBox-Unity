/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PBBox.AssetManagement
{
    [SingletonPriority(-1)]
    internal sealed partial class AssetManager : IAssetManager, ISingletonLifecycle, ILogicUpdateHandler<LogicUpdater.Default>
    {
        // 当前资源管理器的卸载策略
        public AssetUnloadStrategy UnloadStrategy { get; private set; }
        // 待卸载的资源加载器列表
        private HashSet<IAssetLoader> m_PendingUnloadList = new HashSet<IAssetLoader>();
        private HashSet<IAssetLoader> m_RemovePendingUnloadList = new HashSet<IAssetLoader>();
        /// <summary>
        /// 当前帧已卸载的资源数量。
        /// </summary>
        private int m_CurUnloadedAssetCountOnFrame = 0;
        /// <summary>
        /// 上一次卸载更新的时间（毫秒）
        /// </summary>
        private long m_LastUnloadUpdateTimeMs = 0;

        LogicUpdater.Default ILogicUpdateHandler<LogicUpdater.Default>.CurrentUpdater { get; set; }
        int ILogicUpdateHandler<LogicUpdater.Default>.SortedOrder => 0;


        public void OnCreateAsSingleton()
        {
            // 预热加载器类型Map，确保在第一次使用时不会有性能损耗
            //WarmupLoaderTypeMap();
            //ReleaseStrategy = AssetReleaseStrategy.DefaultAuto;
            RegisterDefaultUnloadStrategy();
            RegisterLoaderFactories();
            LogicUpdater.Attach(this);
        }

        public void OnDestroyAsSingleton()
        {
            LogicUpdater.Detach(this);
        }

        partial void RegisterDefaultUnloadStrategy();
        partial void RegisterLoaderFactories();

        public TAsset AcquireAsset<TAsset>(string key, int loadType) where TAsset : class
        {
            var loader = GetLoader(key, loadType);
            if (loader != null)
            {
                return loader.AcquireAsset<TAsset>();
            }
            else
            {
                Log.Error($"AssetLoader '{key}' not found.", nameof(IAssetManager), Log.PBBoxLoggerName);
                return null;
            }
        }

        public IList<TAsset> AcquireAssets<TAsset>(string key, int loadType) where TAsset : class
        {
            var loader = GetLoader(key, loadType);
            if (loader != null)
            {
                return loader.AcquireAssets<TAsset>();
            }
            else
            {
                Log.Error($"AssetLoader '{key}' not found.", nameof(IAssetManager), Log.PBBoxLoggerName);
                return null;
            }
        }

        public Task<TAsset> AcquireAssetAsync<TAsset>(string key, int loadType, Action<TAsset> callBack = null) where TAsset : class
        {
            var loader = GetLoader(key, loadType);
            if (loader != null)
            {
                return loader.AcquireAssetAsync<TAsset>(callBack);
            }
            else
            {
                Log.Error($"AssetLoader '{key}' not found.", nameof(IAssetManager), Log.PBBoxLoggerName);
                return Task.FromResult<TAsset>(null);
            }
        }

        public Task<IList<TAsset>> AcquireAssetsAsync<TAsset>(string key, int loadType, Action<IList<TAsset>> onFinish = null, Action<TAsset> onLoadOne = null) where TAsset : class
        {
            var loader = GetLoader(key, loadType);
            if (loader != null)
            {
                return loader.AcquireAssetsAsync<TAsset>(onFinish, onLoadOne);
            }
            else
            {
                Log.Error($"AssetLoader '{key}' not found.", nameof(IAssetManager), Log.PBBoxLoggerName);
                return Task.FromResult<IList<TAsset>>(null);
            }
        }

        public void ReleaseAsset<TAsset>(string key, ref TAsset asset, int loadType) where TAsset : class
        {
            //TODO 进行释放
            var loader = GetLoader(key, loadType);
            if (loader != null)
            {
                // 检查传入的asset是否与loader中加载的资源一致
                if (loader.GetCachedAsset() as TAsset != asset)
                {
                    Log.Error($"The asset provided for release does not match the loaded asset for key '{key}'. Loader asset Type: {loader.GetCachedAsset()?.GetType().Name}, Provided asset Type: {asset?.GetType().Name}", nameof(IAssetManager), Log.PBBoxLoggerName);
                    return;
                }
                asset = null;
                loader.ReleaseAsset();
                //尝试根据释放策略卸载资源
                TryUnloadAssetWithStrategy(loader);
            }
            else
            {
                Log.Error($"AssetLoader '{key}' not found.", nameof(IAssetManager), Log.PBBoxLoggerName);
            }
        }

        public void ReleaseAsset(string key, int loadType)
        {
            var loader = GetLoader(key, loadType);
            if (loader != null)
            {
                loader.ReleaseAsset();
                //尝试根据释放策略卸载资源
                TryUnloadAssetWithStrategy(loader);
            }
            else
            {
                Log.Error($"AssetLoader '{key}' not found.", nameof(IAssetManager), Log.PBBoxLoggerName);
            }
        }

        private void TryUnloadAssetWithStrategy(IAssetLoader loader)
        {
            if (loader.ReferenceCount > 0 || UnloadStrategy.isManual)
            {
                //还有引用，或者是手动释放策略，不需要卸载
                return;
            }
            if (!loader.IsLoading)
            {
                if (UnloadStrategy.IsImmediate && !IsUnloadMaxCountReached())
                {
                    if (loader.TryUnloadAsset())
                    {
                        m_PendingUnloadList.Remove(loader);
                        return;
                    }
                }
            }
            //资源未能正确卸载或未到卸载时机，加入待卸载列表
            m_PendingUnloadList.Add(loader);
            return;
        }

        public void UnloadAllUnusedAssets()
        {
            foreach (var container in m_LoaderContainers)
            {
                foreach (var kvp in container.Loaders)
                {
                    var loader = kvp.Value;
                    if (loader != null && loader.ReferenceCount <= 0 && !loader.IsLoading)
                    {
                        if (loader.TryUnloadAsset())
                        {
                            m_PendingUnloadList.Remove(loader);
                        }
                    }
                }
            }
        }

        public bool TryUnloadUnusedAsset(string key, int loadType)
        {
            var loader = GetLoader(key, loadType);
            if (loader != null && loader.ReferenceCount <= 0 && !loader.IsLoading)
            {
                if (loader.TryUnloadAsset())
                {
                    m_PendingUnloadList.Remove(loader);
                    return true;
                }
            }
            return false;
        }

        public void SetUnloadStrategy(AssetUnloadStrategy strategy)
        {
            UnloadStrategy = strategy;
        }


        private bool IsUnloadMaxCountReached()
        {
            var maxUnloadCount = UnloadStrategy.maxUnloadCountPerFrame;
            return maxUnloadCount > 0 && m_CurUnloadedAssetCountOnFrame >= maxUnloadCount;
        }

        private void OnUpdate(float deltaTime)
        {
            var currentTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            m_CurUnloadedAssetCountOnFrame = 0;
            if (currentTimeMs - m_LastUnloadUpdateTimeMs < UnloadStrategy.intervalMs)
            {
                //未到更新间隔，跳过本次卸载
                return;
            }
            if (m_PendingUnloadList.Count > 0)
            {
                foreach (var loader in m_PendingUnloadList)
                {
                    if (loader.ReferenceCount > 0)
                    {
                        //还有引用，不需要卸载
                        m_RemovePendingUnloadList.Add(loader);
                        continue;
                    }
                    if (loader.IsLoading || loader.LastReleaseTimeMs + UnloadStrategy.delayMs > currentTimeMs)
                    {
                        //资源正在加载 或未到卸载时机，跳过卸载等待下次
                        continue;
                    }
                    if (!loader.TryUnloadAsset())
                    {
                        //资源卸载失败，等待下次
                        continue;
                    }
                    // 资源已成功卸载，加入移除列表
                    m_RemovePendingUnloadList.Add(loader);
                    m_CurUnloadedAssetCountOnFrame++;
                    if (IsUnloadMaxCountReached())
                    {
                        break;
                    }
                }
                // 移除已成功卸载的资源加载器
                m_PendingUnloadList.ExceptWith(m_RemovePendingUnloadList);
                m_RemovePendingUnloadList.Clear();
            }
            if (IsUnloadMaxCountReached() && m_PendingUnloadList.Count > 0)
            {
                //超过最大卸载数量，且仍有待卸载资源，使下次卸载在下一帧立即进行
                m_LastUnloadUpdateTimeMs = 0;
            }
            else
            {
                m_LastUnloadUpdateTimeMs = currentTimeMs;
            }
        }

        void ILogicUpdateHandler<LogicUpdater.Default>.OnUpdate(float deltaTime)
        {
            OnUpdate(deltaTime);
        }
    }
}