/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PBBox;
using PBBox.UI;
using PBBox.Variables;

namespace PBBox.UI
{
    [AddComponentMenu("PBBox/UI/Framework/UI View")]
    public class UIView : BaseUIView
    {
        [Space]
        [SerializeField, Tooltip("唯一UIID")]
        string m_UIID;
        [Tooltip("触发关闭UI后隐藏或销毁UI的延迟,用于适配关闭动画")]
        public float hideDelay = 0f;
        [SerializeField, Tooltip("存放自身的组件以便获取")]
        SDictionary<string, Object> m_Reference;
        System.Lazy<Dictionary<string, Component>> m_StoredComponents = new System.Lazy<Dictionary<string, Component>>();

        #if UNITY_EDITOR
        void OnValidate()
        {
            if(string.IsNullOrEmpty(m_UIID)){
                m_UIID = gameObject.name;
            }    
        }
        #endif

        public override string GetUIID()
        {
            return m_UIID;
        }

        /// <summary>
        /// 获取一个Reference
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetReference<T>(string key) where T : Object
        {
            if (m_Reference == null) return null;
            if (m_Reference.TryGetValue(key, out var r))
            {
                return r as T;
            }
            return null;
        }

        /// <summary>
        /// 获取一个Reference作为GameObject
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public GameObject GetReference(string key){
            return GetReference<GameObject>(key);
        }

        /// <summary>
        /// 从References中获取一个Component
        /// </summary>
        /// <param name="key"></param>
        /// <param name="store">是否存下来以备后续使用</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetRefComponent<T>(string key,bool store = false) where T : Component
        {
            string cKey = key + "_" + typeof(T).FullName;
            if (!m_StoredComponents.Value.TryGetValue(cKey, out var component))
            {
                var obj = GetReference<GameObject>(key);
                if (obj == null)
                {
                    return null;
                }
                component = obj.GetComponent<T>();
                if(store){
                    StoreRefComponent(key, component as T);
                }
            }
            return component as T;
        }

        /// <summary>
        /// 储存一个自身Component以备后用
        /// </summary>
        /// <param name="key"></param>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        public void StoreRefComponent<T>(string key, T component) where T : Component
        {
            string cKey = key + "_" + typeof(T).FullName;
            m_StoredComponents.Value[cKey] = component;
        }

        protected override float GetHideEndDelay()
        {
            return hideDelay;
        }

    }
}