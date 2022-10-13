/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.31
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_ODIN
using Sirenix.OdinInspector;
#endif

namespace PBBox.UI
{
    [DefaultExecutionOrder(-50)]
    [AddComponentMenu("PBBox/UI/Framework/UI View Register")]
    public sealed class UIViewRegister : MonoBehaviour
    {
        [Header("Asset Reference")]
        [SerializeField]
        bool m_UnregisterOnDestroy = true;
#if USE_ODIN
        [AssetsOnly]
#endif 
        [SerializeField]
        GameObject[] m_UIPrefabs;
        [Header("AssetManager Preload")]
        [SerializeField]
        bool m_UnloadOnDestroy = true;
        [SerializeField]
        string[] m_PreloadUIIDs;
        [Header("Init Status")]
        [SerializeField]
        string[] m_DefaultShowUI;
        [SerializeField]
        string[] m_DestroyHideUI;
        [SerializeField]
        string[] m_DestoryHideTag;

        void Awake()
        {
            if (m_UIPrefabs != null)
            {
                foreach (var p in m_UIPrefabs)
                {
                    UIViews.RegisterUIPrefab(p);
                }
            }
            if (m_PreloadUIIDs != null)
            {
                foreach (var id in m_PreloadUIIDs)
                {
                    UIViews.Load(id);
                }
            }
        }

        void Start()
        {
            if (m_DefaultShowUI == null) return;
            foreach (var uiid in m_DefaultShowUI)
            {
                UIViews.Show(uiid);
            }
        }

        void OnDestroy()
        {
            if (!UIViews.HasInstance) return;
            if (m_DestroyHideUI != null)
            {
                foreach (var uiid in m_DestroyHideUI)
                {
                    UIViews.Hide(uiid);
                }
            }
            if (m_DestoryHideTag != null)
            {
                foreach (var tag in m_DestoryHideTag)
                {
                    UIViews.HideWithTag(tag, true);
                }
            }
            if (m_UnregisterOnDestroy)
            {
                foreach (var p in m_UIPrefabs)
                {
                    UIViews.UnregisterUIPrefab(p);
                }
            }
            if (m_UnloadOnDestroy)
            {
                foreach (var id in m_PreloadUIIDs)
                {
                    UIViews.Unload(id);
                }
            }
        }
    }

}
