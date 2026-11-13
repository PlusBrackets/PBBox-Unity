#if UNITY_5_3_OR_NEWER
/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

namespace PBBox.AssetManagement
{
    [AssetLoaderAssign(AssetLoadType.Addressable, -1)]
    public class AddressableAssetLoader : AssetLoaderBase
    {
        private AsyncOperationHandle m_Handler;
        protected override string LogTag => "AddressableLoader";
        
        public AddressableAssetLoader(string key) : base(key)
        {
        }

        protected override TAsset LoadImpl<TAsset>()
        {
            if (!m_Handler.IsValid())
            {
                m_Handler = Addressables.LoadAssetAsync<TAsset>(Key);
            }
            return m_Handler.WaitForCompletion() as TAsset;
        }

        protected override async Task<TAsset> LoadImplAsync<TAsset>()
        {
            if (!m_Handler.IsValid())
            {
                m_Handler = Addressables.LoadAssetAsync<TAsset>(Key);
            }
            var task = await m_Handler.Task as TAsset;
            return task;
        }

        protected override IList<TAsset> LoadAllImpl<TAsset>()
        {
            if (!m_Handler.IsValid())
            {
                m_Handler = Addressables.LoadAssetsAsync<TAsset>(Key, null);
            }
            return m_Handler.WaitForCompletion() as IList<TAsset>;
        }

        protected override async Task<IList<TAsset>> LoadAllImplAsync<TAsset>(System.Action<TAsset> onLoadOne)
        {
            if (!m_Handler.IsValid())
            {
                m_Handler = Addressables.LoadAssetsAsync<TAsset>(Key, onLoadOne);
            }
            return await m_Handler.Task as IList<TAsset>;
        }

        protected override bool UnloadImpl()
        {
            if (m_Handler.IsValid())
            {
                Addressables.Release(m_Handler);
                m_Handler = default;
            }
            return true;
        }

        public override bool IsAssetVaild()
        {
            return base.IsAssetVaild() && IsAssetVaildUnity();
        }
    }
}
#endif