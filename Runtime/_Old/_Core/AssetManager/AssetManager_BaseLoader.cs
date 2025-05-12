using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PBBox
{
    public sealed partial class AssetManager : SingleClass<AssetManager>
    {

        internal abstract class ObjectLoader
        {
            public string key { get; protected set; } = null;
            public virtual Object asset { get; protected set; } = null;
            public virtual Object[] assets{get;protected set;} = null;
            public int assetRefCount { get; protected set; } = 0;

            public ObjectLoader(string key)
            {
                this.key = key;
            }

            public virtual bool IsValid(){
                return asset != null || assets != null;
            }

            public T Load<T>() where T : Object
            {
                if (!IsValid())
                {
                    asset = DoLoad<T>();
                }
                if (asset != null)
                {
                    assetRefCount++;
                }
                return asset as T;
            }

            public async Task<T> LoadAsync<T>() where T : Object
            {
                if (!IsValid())
                {
                    asset = await DoLoadAsync<T>();
                }
                if (asset != null)
                {
                    assetRefCount++;
                }
                return asset as T;
            }

            public T[] Loads<T>(System.Action<T> callBack) where T : Object
            {
                if (!IsValid())
                {
                    assets = DoLoads<T>(callBack);
                }
                if (assets != null)
                {
                    assetRefCount++;
                }
                return assets as T[];
            }

            public async Task<T[]> LoadsAsync<T>(System.Action<T> callBack) where T : Object
            {
                if (!IsValid())
                {
                    assets = await DoLoadsAsync<T>(callBack);
                }
                if (assets != null)
                {
                    assetRefCount++;
                }
                return assets as T[];
            }

            

            public void ReleaseAsset()
            {
                assetRefCount--;
                assetRefCount = Mathf.Max(0, assetRefCount);
                if (assetRefCount == 0)
                {
                    OnReleaseAsset();
                }
            }

            protected abstract T DoLoad<T>() where T : Object;
            protected abstract Task<T> DoLoadAsync<T>() where T : Object;
            protected abstract T[] DoLoads<T>(System.Action<T> callBack) where T : Object;
            protected abstract Task<T[]> DoLoadsAsync<T>(System.Action<T> callBack) where T : Object;
            protected abstract void OnReleaseAsset();


            public abstract GameObject Instantiate(InstantiationParameters param);
            public abstract Task<GameObject> InstantiateAsync(InstantiationParameters param);
            public abstract bool ReleaseInstance(GameObject obj);

        }
    }
}