/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.31
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
#if USE_ODIN
using Sirenix.OdinInspector;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PBBox.UI
{

    [AddComponentMenu("PBBox/UI/Framework/UI View Canvas")]
    [RequireComponent(typeof(Canvas)), DefaultExecutionOrder(-101), DisallowMultipleComponent]
    public sealed class UIViewCanvas : MonoBehaviour
    {
        [SerializeField]
        bool m_DontDestroyOnLoad = false;
        [SerializeField, Tooltip("若为空，则取GameObject的名称作为该Canvas的ID")]
        string m_CanvasID = null;
        public string canvasID { get; private set; }
        Canvas m_Canvas;
        public Canvas canvas { get; }
#if USE_ODIN
        [ChildGameObjectsOnly]
#endif
        [SerializeField,Tooltip("自定义默认的ui容器，若为空则默认取自身")]
        GameObject m_CustomDefaultContainer;
#if USE_ODIN
        [ChildGameObjectsOnly]
#endif
        [SerializeField]
        GameObject[] m_ContainerList;
        Dictionary<string, GameObject> m_Containers;
        public Dictionary<string, GameObject> containers => m_Containers;

        void Awake()
        {
            m_Canvas = GetComponent<Canvas>();
            canvasID = string.IsNullOrEmpty(m_CanvasID) ? gameObject.name : m_CanvasID;
            m_Containers = new Dictionary<string, GameObject>();
            foreach (var g in m_ContainerList)
            {
                m_Containers.TryAdd(g.name, g);
            }
            if (m_CustomDefaultContainer == null)
            {
                m_CustomDefaultContainer = gameObject;
            }
            if (UIViews.RegisterCnavas(this) && m_DontDestroyOnLoad)
            {
                GameObject.DontDestroyOnLoad(gameObject);
            }
        }

        void OnDestroy()
        {
            UIViews.UnregisterCanvas(this);
        }

        public GameObject GetContainer(string containerName)
        {
            GameObject g = null;
            if (!string.IsNullOrEmpty(containerName))
            {
                m_Containers.TryGetValue(containerName, out g);
            }
            if (g == null) g = m_CustomDefaultContainer;
            return g;
        }
    }
}