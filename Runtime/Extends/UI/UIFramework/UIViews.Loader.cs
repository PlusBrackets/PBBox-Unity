/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.31
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace PBBox.UI
{
    /// <summary>
    /// UI 施工中
    /// </summary>
    public sealed partial class UIViews : SingleClass<UIViews>
    {
        // [System.Serializable]
        // public struct UIPath
        // {
        //     public enum PathType
        //     {
        //         PATH_ASSET = 0,
        //         PATH_RESOURCE,
        //         PATH_ADDRESSABLE
        //     }
        //     public string UIID;
        //     public PathType pathType;
        //     public string path;
        // }

        // Dictionary<string, UIPath> m_UIPaths;
        HashSet<string> m_LoadedUIList;

        void InitViewLoader()
        {
            m_LoadedUIList = new HashSet<string>();
            //load ui path assets
            // m_UIPaths = new Dictionary<string, UIPath>();
        }

        public static async Task<IUIView> LoadAsync(string uiid, System.Action<IUIView> callBack = null)
        {
            if (IsLoaded(uiid))
                return Instance.m_ViewPrefabs[uiid];
            var ins = Instance;
            ins.m_LoadedUIList.Add(uiid);
            GameObject obj = await AssetManager.LoadAssetAsync<GameObject>(uiid);
            IUIView view = null;
            if (obj != null)
            {
                view = obj.GetComponent<IUIView>();
                RegisterUIPrefab(view);
                DebugUtils.Info<UIViews>($"Loaded UI: [{uiid}]");
            }
            else
            {
                ins.m_LoadedUIList.Remove(uiid);
                DebugUtils.InfoError<UIViews>($"Can not load UI: [{uiid}], resouce name may be incorrect.");
            }
            callBack?.Invoke(view);
            return view;
        }

        public static IUIView Load(string uiid)
        {
            if (IsLoaded(uiid))
                return Instance.m_ViewPrefabs[uiid];
            GameObject obj = AssetManager.LoadAsset<GameObject>(uiid);
            IUIView view = null;
            if (obj != null)
            {
                view = obj.GetComponent<IUIView>();
                RegisterUIPrefab(view);
                if (!Instance.m_LoadedUIList.Add(uiid))
                {
                    AssetManager.ReleaseAsset(uiid);
                }
                DebugUtils.Info<UIViews>($"Loaded UI: [{uiid}]");
            }
            else
            {
                DebugUtils.InfoError<UIViews>($"Can not load UI: [{uiid}], resouce name may be incorrect.");
            }
            return view;
        }

        public static bool IsLoaded(string uiid)
        {
            return Instance.m_ViewPrefabs.ContainsKey(uiid);
        }

        public static void Unload(string uiid)
        {
            UnregisterUIPrefab(uiid);
            if (Instance.m_LoadedUIList.Contains(uiid))
            {
                AssetManager.ReleaseAsset(uiid);
                Instance.m_LoadedUIList.Remove(uiid);
            }
        }

        /// <summary>
        /// 注册一个uiPrefab,用于后续直接创建
        /// </summary>
        /// <param name="uiPrefab"></param>
        public static void RegisterUIPrefab(GameObject uiPrefab)
        {
            IUIView view = uiPrefab.GetComponent<IUIView>();
            if (view != null)
            {
                RegisterUIPrefab(view);
            }
        }

        /// <summary>
        /// 注册一个uiPrefab,用于后续直接创建
        /// </summary>
        /// <param name="uiPrefab"></param>
        public static void RegisterUIPrefab(IUIView uiPrefab)
        {
            if (!Instance.m_ViewPrefabs.TryAdd(uiPrefab.GetUIID(), uiPrefab))
            {
                DebugUtils.InfoWarning<UIViews>($"UIPrefabs字典中已存在ID为[{uiPrefab.GetUIID()}]的UI预制体");
            }
        }

        /// <summary>
        /// 注销一个uiPrefab
        /// </summary>
        /// <param name="uiid"></param>
        public static void UnregisterUIPrefab(string uiid)
        {
            Instance.m_ViewPrefabs.Remove(uiid);
        }

        /// <summary>
        /// 注销一个uiPrefab
        /// </summary>
        /// <param name="uiPrefab"></param>
        public static void UnregisterUIPrefab(GameObject uiPrefab)
        {
            IUIView view = uiPrefab.GetComponent<IUIView>();
            if (view != null)
            {
                UnregisterUIPrefab(view);
            }
        }

        /// <summary>
        /// 注销一个uiPrefab
        /// </summary>
        /// <param name="uiPrefab"></param>
        public static void UnregisterUIPrefab(IUIView uiPrefab)
        {
            Instance.m_ViewPrefabs.Remove(uiPrefab.GetUIID());
        }
    }

}