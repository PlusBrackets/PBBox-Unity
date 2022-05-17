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

        internal abstract class ObjectLoader : System.IDisposable
        {
            public string key { get; protected set; } = null;
            public Object asset { get; protected set; } = null;
            public int assetRefCount { get; protected set; } = 0;
            public bool isDisposed { get; private set; } = false;

            public ObjectLoader(string key)
            {
                this.key = key;
            }

            public T Load<T>() where T : Object
            {
                if (asset == null)
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
                if (asset == null)
                {
                    asset = await DoLoadAsync<T>();
                }
                if (asset != null)
                {
                    assetRefCount++;
                }
                return asset as T;
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
            protected abstract void OnReleaseAsset();


            public abstract GameObject Instantiate(InstantiationParameters param);
            public abstract Task<GameObject> InstantiateAsync(InstantiationParameters param);
            public abstract bool ReleaseInstance(GameObject obj);


            public void Dispose()
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    OnDispose();
                }
            }

            protected abstract void OnDispose();
        }
    }
}