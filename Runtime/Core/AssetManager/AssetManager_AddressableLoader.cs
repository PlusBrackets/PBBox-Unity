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

        internal sealed class ObjectLoader_Addressable : ObjectLoader
        {
            AssetReference a;
            AsyncOperationHandle handler;
            System.Lazy<Dictionary<int, AsyncOperationHandle<GameObject>>> m_References = new System.Lazy<Dictionary<int, AsyncOperationHandle<GameObject>>>();
            Dictionary<int, AsyncOperationHandle<GameObject>> references => m_References.Value;

            public ObjectLoader_Addressable(string key) : base(key)
            {
            }

            protected override T DoLoad<T>()
            {
                T _asset = null;
                if (!handler.IsValid())
                {
                    handler = Addressables.LoadAssetAsync<T>(key);
                }
                _asset = handler.WaitForCompletion() as T;
                return _asset;
            }

            protected async override Task<T> DoLoadAsync<T>()
            {
                if (!handler.IsValid())
                {
                    handler = Addressables.LoadAssetAsync<T>(key);
                }
                return await handler.Task as T;
            }

            protected override void OnReleaseAsset()
            {
                if (handler.IsValid())
                {
                    asset = null;
                    Addressables.Release(handler);
                    handler = default;
                }
            }

            public override GameObject Instantiate(InstantiationParameters param)
            {
                var op = Addressables.InstantiateAsync(key, param);
                op.WaitForCompletion();
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    references.Add(op.Result.GetInstanceID(), op);
                    return op.Result;
                }
                return null;
            }

            public override async Task<GameObject> InstantiateAsync(InstantiationParameters param)
            {
                var op = Addressables.InstantiateAsync(key, param);
                await op.Task;
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    references.Add(op.Result.GetInstanceID(), op);
                    return op.Result;
                }
                return null;
            }

            public override bool ReleaseInstance(GameObject obj)
            {
                int id = obj.GetInstanceID();
                if (references.TryGetValue(id, out var handler))
                {
                    if (Addressables.ReleaseInstance(handler))
                    {
                        references.Remove(id);
                        return true;
                    }
                    return false;
                }
                else
                {
                    GameObject.Destroy(obj);
                    return false;
                }
            }

            protected override void OnDispose()
            {
            }
        }
    }
}