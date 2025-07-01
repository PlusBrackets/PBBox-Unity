using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PBBox.UI
{
    [AddComponentMenu("PBBox/UI/Framework/UI Root")]
    [RequireComponent(typeof(Canvas)), DefaultExecutionOrder(-101), DisallowMultipleComponent]
    public class UIRoot : MonoBehaviour
    {
        private static readonly Lazy<Dictionary<string, UIRoot>> s_Roots = new Lazy<Dictionary<string, UIRoot>>(System.Threading.LazyThreadSafetyMode.None);

        public static readonly string DefaultRootId = "_";

        /// <summary>
        /// 获取指定ID的UIRoot实例。
        /// 如果ID为空，则返回默认的UIRoot实例。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static UIRoot Get(string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = DefaultRootId;
            }
            if (s_Roots.IsValueCreated && s_Roots.Value.TryGetValue(id, out var root))
            {
                return root;
            }
            return null;
        }

        private static bool RegisterRoot(UIRoot root)
        {
            if (!s_Roots.Value.TryAdd(root.Id, root))
            {
                Log.Warning($"UIRoot with ID '{root.Id}' is already registered. Please ensure unique IDs for each UIRoot.", root, "UI", Log.PBBoxLoggerName);
                return false;
            }
            return true;
        }

        private static void UnregisterRoot(UIRoot root)
        {
            if (!s_Roots.IsValueCreated)
            {
                return;
            }
            s_Roots.Value.Remove(root.Id);
        }


        [SerializeField]
        private bool m_DontDestroyOnLoad = false;
        [SerializeField, Tooltip("若为空，则取默认的UIRootID作为该Root的ID")]
        private string m_RootId = null;
        [SerializeField, Tooltip("自定义默认的ui容器，若为空则默认取自身")]
        private GameObject m_CustomDefaultContainer;
#if USE_ODIN
        [ChildGameObjectsOnly]
#endif
        [SerializeField, Tooltip("容器列表，GameObject的名称作为容器ID")]
        private GameObject[] m_ContainerList;

        private Canvas m_Canvas;
        private Dictionary<string, GameObject> m_ContainersLookup;

        public string Id => m_RootId;
        public Canvas Canvas
        {
            get
            {
                if (m_Canvas == null)
                {
                    m_Canvas = GetComponent<Canvas>();
                }
                return m_Canvas;
            }
        }

        private void Awake()
        {
            m_RootId = string.IsNullOrEmpty(m_RootId) ? DefaultRootId : m_RootId;
            if (!RegisterRoot(this))
            {
                gameObject.SetActive(false);
                return;
            }
            m_ContainersLookup = new Dictionary<string, GameObject>();
            foreach (var container in m_ContainerList)
            {
                m_ContainersLookup.TryAdd(container.name, container);
            }
            if (m_CustomDefaultContainer == null)
            {
                m_CustomDefaultContainer = gameObject;
            }
            if (m_DontDestroyOnLoad)
            {
                GameObject.DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            UnregisterRoot(this);
        }

        /// <summary>
        /// 获取指定名称的UI容器。
        /// 如果容器名称为空或未找到，则返回自定义默认容器。
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public GameObject GetContainer(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                return m_CustomDefaultContainer;
            }

            if (m_ContainersLookup.TryGetValue(containerName, out var container))
            {
                return container;
            }

            Log.Warning($"Container '{containerName}' not found in UIRoot '{Id}'. Using default container.", this, "UI", Log.PBBoxLoggerName);
            return m_CustomDefaultContainer;
        }

    }
}