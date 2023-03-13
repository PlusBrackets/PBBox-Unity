using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PBBox;
using PBBox.Properties;

namespace PBBox.UI
{
    [AddComponentMenu("PBBox/Serializable/Reference Store")]
    public class ReferenceStore : MonoBehaviour, IReferenceStore
    {

        [SerializeField, Tooltip("存放自身的组件以便获取")]
        SDictionary<string, Object> m_Reference;
        [SerializeField, Tooltip("以列表的形式，存放自身的组件以便获取")]
        SDictionary<string, Object[]> m_ReferenceArray;
        System.Lazy<Dictionary<string, Component>> m_StoredComponents = new System.Lazy<Dictionary<string, Component>>();


        SDictionary<string, Object> IReferenceStore.GetReferenceMap()
        {
            return m_Reference;
        }

        SDictionary<string, Object[]> IReferenceStore.GetReferenceArrayMap()
        {
            return m_ReferenceArray;
        }

        Dictionary<string, Component> IReferenceStore.GetComponentStorage()
        {
            return m_StoredComponents.Value;
        }
    }
}