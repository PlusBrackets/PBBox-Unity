using System.Threading.Tasks;
using UnityEngine;

namespace PBBox.View
{
    /// <summary>
    /// Prefab视图工厂类，用于创建和管理Prefab视图。
    /// </summary>

    public class PrefabViewFactory : IViewFactory
    {
        
        //public string AssetPath { get; }
        //public AssetLoaderTypes LoaderTypes { get; } = AssetLoaderTypes.Default;
        private AssetLoadHandle m_AssetHandle;

        public PrefabViewFactory(string assetPath, AssetLoaderTypes loaderTypes = AssetLoaderTypes.Default)
        {
            //AssetPath = assetPath;
            //LoaderTypes = loaderTypes;
            m_AssetHandle = new AssetLoadHandle(assetPath, loaderTypes);
        }

        IView IViewFactory.CreateView(IController controller)
        {
            //var assetPath = string.IsNullOrEmpty(AssetPath) ? Id : AssetPath;
            //m_AssetHandle = new AssetLoadHandle(assetPath, LoaderTypes);
            //GameObject prefab = null;
            //if (m_AssetHandle.IsVaild())
            //{
            //    prefab = m_AssetHandle.GetAsset<GameObject>();
            //}
            //else
            //{
            var prefab = m_AssetHandle.Load<GameObject>();
            //}
            if (prefab == null)
            {
                Log.Error($"Failed to load prefab from path: {m_AssetHandle.Key}", nameof(PrefabViewFactory), Log.PBBoxLoggerName);
                return null;
            }
            var obj = Object.Instantiate(prefab);
            var view = obj.GetComponent<IView>();
            if (view == null)
            {
                view = obj.AddComponent<MonoViewBase>();
            }
            view.Controller = controller;
            return view;
        }

        async Task<bool> IViewFactory.PreloadView(IController controller)
        {
            //var assetPath = string.IsNullOrEmpty(AssetPath) ? Id : AssetPath;
            //m_AssetHandle = new AssetLoadHandle(assetPath, LoaderTypes);
            if(m_AssetHandle.IsVaild())
            {
                // 如果已经加载过了，就不需要再次加载
                return true;
            }
            var obj = await m_AssetHandle.LoadAsync<GameObject>();
            m_AssetHandle.Release();
            if (obj == null)
            {
                Log.Error($"Failed to preload prefab from path: {m_AssetHandle.Key}", nameof(PrefabViewFactory), Log.PBBoxLoggerName);
                return false;
            }
            return true;
        }

        void IViewFactory.ReleaseView(IController controller)
        {
            if (controller.HasView())
            {
                var view = controller.GetView() as Component;
                if (view != null)
                {
                    Object.Destroy(view.gameObject);
                }
            }
            m_AssetHandle.Release();
        }
    }
}
