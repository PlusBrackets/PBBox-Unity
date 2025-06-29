/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Collections;
using System.Collections.Generic;

namespace PBBox
{
    [AssetLoaderAssign(AssetLoaderTypes.Addressable, -1)]
    public class AddressableAssetLoader : AssetLoaderBase
    {
        //[AssetLoaderCreateMethod]
        //public static IAssetLoader Create(string key)
        //{
        //    return new AddressableAssetLoader(key);
        //}

        private AsyncOperationHandle m_Handler;
        protected override string LogTag => "AddressableLoader";
        
        public AddressableAssetLoader(string key) : base(key)
        {
        }

        protected override TAsset DoLoad<TAsset>()
        {
            if (!m_Handler.IsValid())
            {
                m_Handler = Addressables.LoadAssetAsync<TAsset>(Key);
            }
            return m_Handler.WaitForCompletion() as TAsset;
        }

        protected override async Task<TAsset> DoLoadAsync<TAsset>()
        {
            if (!m_Handler.IsValid())
            {
                m_Handler = Addressables.LoadAssetAsync<TAsset>(Key);
            }
            return await m_Handler.Task as TAsset;
        }

        protected override IList<TAsset> DoLoads<TAsset>()
        {
            if (!m_Handler.IsValid())
            {
                m_Handler = Addressables.LoadAssetsAsync<TAsset>(Key, null);
            }
            return m_Handler.WaitForCompletion() as IList<TAsset>;
        }

        protected override async Task<IList<TAsset>> DoLoadsAsync<TAsset>()
        {
            if (!m_Handler.IsValid())
            {
                m_Handler = Addressables.LoadAssetsAsync<TAsset>(Key, null);
            }
            return await m_Handler.Task as IList<TAsset>;
        }

        protected override void OnReleaseAsset()
        {
            if (m_Handler.IsValid())
            {
                Addressables.Release(m_Handler);
                m_Handler = default;
            }
        }


        public override bool IsVaild()
        {
            return base.IsVaild() && IsVaildUnity();
        }
    }
}