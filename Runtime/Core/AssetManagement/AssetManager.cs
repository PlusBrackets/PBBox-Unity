/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace PBBox.AssetManagement
{
    [SingletonPriority(-1)]
    internal sealed partial class AssetManager2 : IAssetManager, ISingletonLifecycle
    {
        public void OnCreateAsSingleton()
        {
            // 预热加载器类型Map，确保在第一次使用时不会有性能损耗
            WarmupLoaderTypeMap();
        }

        public void OnDestroyAsSingleton()
        {

        }

        public TAsset LoadAsset<TAsset>(string key, int loadType = (int)AssetLoadType.Default) where TAsset : class
        {
            var loader = GetLoader(key, loadType);
            if (loader != null)
            {
                return loader.Load<TAsset>();
            }
            else
            {
                Log.Error($"AssetLoader '{key}' not found.", nameof(IAssetManager), Log.PBBoxLoggerName);
                return null;
            }
        }

        public IList<TAsset> LoadAssetAll<TAsset>(string key, int loadType = (int)AssetLoadType.Default) where TAsset : class
        {
            var loader = GetLoader(key, loadType);
            if (loader != null)
            {
                return loader.LoadAll<TAsset>();
            }
            else
            {
                Log.Error($"AssetLoader '{key}' not found.", nameof(IAssetManager), Log.PBBoxLoggerName);
                return null;
            }
        }

        public Task<TAsset> LoadAssetAsync<TAsset>(string key, int loadType = (int)AssetLoadType.Default, Action<TAsset> callBack = null) where TAsset : class
        {
            var loader = GetLoader(key, loadType);
            if (loader != null)
            {
                return loader.LoadAsync<TAsset>(callBack);
            }
            else
            {
                Log.Error($"AssetLoader '{key}' not found.", nameof(IAssetManager), Log.PBBoxLoggerName);
                return Task.FromResult<TAsset>(null);
            }
        }

        public Task<IList<TAsset>> LoadAssetAllAsync<TAsset>(string key, int loadType = (int)AssetLoadType.Default, Action<IList<TAsset>> onFinish = null, Action<TAsset> onLoadOne = null) where TAsset : class
        {
            var loader = GetLoader(key, loadType);
            if (loader != null)
            {
                return loader.LoadAllAsync<TAsset>(onFinish, onLoadOne);
            }
            else
            {
                Log.Error($"AssetLoader '{key}' not found.", nameof(IAssetManager), Log.PBBoxLoggerName);
                return Task.FromResult<IList<TAsset>>(null);
            }
        }

        public void ReleaseAsset<TAsset>(string key, ref TAsset asset, int loadType = (int)AssetLoadType.Default) where TAsset : class
        {
            //TODO 进行释放
            var loader = GetLoader(key, loadType);
            if (loader != null)
            {
                // 检查传入的asset是否与loader中加载的资源一致
                if (loader.GetAssetDirect() as TAsset != asset)
                {
                    Log.Error($"The asset provided for release does not match the loaded asset for key '{key}'.", nameof(IAssetManager), Log.PBBoxLoggerName);
                    return;
                }
                asset = null;
                loader.Release();
            }
            else
            {
                Log.Error($"AssetLoader '{key}' not found.", nameof(IAssetManager), Log.PBBoxLoggerName);
            }
        }

        public void ReleaseAsset(string key, int loadType = (int)AssetLoadType.Default)
        {
            var loader = GetLoader(key, loadType);
            if (loader != null)
            {
                loader.Release();
            }
            else
            {
                Log.Error($"AssetLoader '{key}' not found.", nameof(IAssetManager), Log.PBBoxLoggerName);
            }
        }

        public void ReleaseUnusedAssets()
        {
            throw new NotImplementedException();
        }

        public void SetReleaseStrategy(int loadType, AssetReleaseStrategy strategy)
        {
            throw new NotImplementedException();
        }

        public void SetReleaseStrategy(AssetReleaseStrategy strategy)
        {
            throw new NotImplementedException();
        }

    }
}