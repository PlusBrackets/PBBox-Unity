#if UNITY_5_3_OR_NEWER
/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace PBBox.AssetManagement
{
    [AssetLoaderAssign(AssetLoadType.Resources, -1)]
    public class ResourcesAssetLoader : AssetLoaderBase
    {
        //[AssetLoaderCreateMethod]
        //public static IAssetLoader Create(string key)
        //{
        //    return new ResourcesAssetLoader(key);
        //}

        protected override string LogTag => "ResourcesLoader";

        public ResourcesAssetLoader(string key) : base(key)
        {
        }

        protected override TAsset LoadImpl<TAsset>()
        {
            return Resources.Load(Key, typeof(TAsset)) as TAsset;
        }

        protected override async Task<TAsset> LoadImplAsync<TAsset>()
        {
            var tcs = new TaskCompletionSource<TAsset>();
            Resources.LoadAsync(Key, typeof(TAsset)).completed += (op) =>
            {
                if (op is ResourceRequest request)
                {
                    tcs.SetResult(request.asset as TAsset);
                }
                else
                {
                    tcs.SetException(new System.Exception("Failed to load asset asynchronously."));
                }
            };
            return await tcs.Task;
        }

        protected override IList<TAsset> LoadAllImpl<TAsset>()
        {
            var array = Resources.LoadAll(Key, typeof(TAsset));
            return ConvertToList<TAsset>(array);
        }

        protected override Task<IList<TAsset>> LoadAllImplAsync<TAsset>(System.Action<TAsset> onLoadOne)
        {
            // Unity's Resources API does not support async loading for multiple assets.
            var result = LoadAllImpl<TAsset>();
            if (onLoadOne != null)
            {
                foreach (var asset in result)
                {
                    onLoadOne.Invoke(asset);
                }
            }
            return Task.FromResult(result);
        }

        protected override bool UnloadImpl()
        {
            if (IsAssetVaild())
            {
                if (m_Asset is Object asset)
                {
                    TryUnloadAsset(asset);
                }
                else if (m_Asset is IList assetList)
                {
                    foreach (var item in assetList)
                    {
                        TryUnloadAsset(item);
                    }
                }
            }
            return true;
        }

        protected void TryUnloadAsset(object asset)
        {
            if (asset is GameObject || asset is Component)
            {
                return;
            }
            else if (asset is Object unityObject)
            {
                Resources.UnloadAsset(unityObject);
            }
        }

        public override bool IsAssetVaild()
        {
            return base.IsAssetVaild() && IsAssetVaildUnity();
        }
    }
}
#endif