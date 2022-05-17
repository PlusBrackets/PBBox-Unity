using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace PBBox
{
    public sealed partial class AssetManager : SingleClass<AssetManager>
    {
        System.Lazy<Dictionary<string, ObjectLoader>> m_Loaders = new System.Lazy<Dictionary<string, ObjectLoader>>();
        Dictionary<string, ObjectLoader> loaders => m_Loaders.Value;
        System.Lazy<Dictionary<GameObject, ObjectLoader>> m_LookUp = new System.Lazy<Dictionary<GameObject, ObjectLoader>>();
        Dictionary<GameObject, ObjectLoader> lookUp => m_LookUp.Value;

        AssetManager() { }

        ObjectLoader GetLoader(string key)
        {
            if (!loaders.TryGetValue(key, out ObjectLoader loader))
            {
                loader = new ObjectLoader_Addressable(key);
                loaders.Add(key, loader);
            }
            return loader;
        }

        public static T LoadAsset<T>(string key) where T : Object
        {
            return Instance.GetLoader(key).Load<T>();
        }

        public static async Task<T> LoadAssetAsync<T>(string key, System.Action<T> callBack = null) where T : Object
        {
            var asset = await Instance.GetLoader(key).LoadAsync<T>();
            callBack?.Invoke(asset);
            return asset;
        }

        public static void ReleaseAsset(string key)
        {
            Instance.GetLoader(key).ReleaseAsset();
        }

        static InstantiationParameters GetInstantiationParam(Transform parent = null, Vector3? pos = null, Quaternion? rot = null, bool inWorldSpace = false)
        {
            InstantiationParameters param;
            if (pos.HasValue || rot.HasValue)
            {
                param = new InstantiationParameters(pos.HasValue ? pos.Value : Vector3.zero, rot.HasValue ? rot.Value : Quaternion.identity, parent);
            }
            else
            {
                param = new InstantiationParameters(parent, inWorldSpace);
            }
            return param;
        }

        public static GameObject Instantiate(string key, Transform parent = null, Vector3? pos = null, Quaternion? rot = null, bool inWorldSpace = false)
        {
            InstantiationParameters param = GetInstantiationParam(parent, pos, rot, inWorldSpace);
            var loader = Instance.GetLoader(key);
            GameObject obj = Instance.GetLoader(key).Instantiate(param);
            if (obj != null)
            {
                Instance.lookUp.Add(obj, loader);
            }
            return obj;
        }

        public static async Task<GameObject> InstantiateAsync(string key, Transform parent = null, Vector3? pos = null, Quaternion? rot = null, bool inWorldSpace = false,
        System.Action<GameObject> callBack = null)
        {
            InstantiationParameters param = GetInstantiationParam(parent, pos, rot, inWorldSpace);
            var loader = Instance.GetLoader(key);
            GameObject obj = await Instance.GetLoader(key).InstantiateAsync(param);
            if (obj != null)
            {
                Instance.lookUp.Add(obj, loader);
            }
            callBack?.Invoke(obj);
            return obj;
        }

        public static bool ReleaseInstance(GameObject obj)
        {
            if (Instance.lookUp.ContainsKey(obj))
            {
                var temp = Instance.lookUp[obj];
                Instance.lookUp.Remove(obj);
                return temp.ReleaseInstance(obj);
            }
            else
            {
                GameObject.Destroy(obj);
                return false;
            }
        }

        public static void ReleaseInvaildInstance(string key = null)
        {
            GameObject[] keys = Instance.lookUp.Keys.ToArray();
            Debug.Log(keys.Length);
            foreach (var obj in keys)
            {
                if (obj == null)
                {
                    var temp = Instance.lookUp[obj];
                    Instance.lookUp.Remove(obj);
                    temp.ReleaseInstance(obj);
                }
            }
        }
    }
}