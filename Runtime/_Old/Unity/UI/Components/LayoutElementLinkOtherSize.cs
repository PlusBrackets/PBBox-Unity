/*--------------------------------------------------------
 *Copyright (c) 2016-2024 PlusBrackets
 *@update: 2024.07.24
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PBBox;
using System;

namespace PBBox.Unity.UI
{
    /// <summary>
    /// 用于将LayoutElement的尺寸与其他RectTransform的尺寸关联起来
    /// </summary>
    [AddComponentMenu("PBBox/UI/Components/LayoutElementLinkOtherSize")]
    [RequireComponent(typeof(LayoutElement))]
    [ExecuteAlways]
    public class LayoutElementLinkOtherSize : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_LinkTarget;
        private LayoutElement m_LayoutElement;
        private IDisposable m_TargetSizeChangeSubscription = null;
        [SerializeField, Tooltip("是否控制宽度")]
        private bool m_ControlWidth = false;
        [SerializeField, Tooltip("是否控制高度")]
        private bool m_ControlHeight = false;
        [SerializeField, Tooltip("本身尺寸可设置到的最小值，如果为-1则表示不限制")]
        private Vector2 m_MinSize = new Vector2(-1, -1);
        [SerializeField, Tooltip("本身尺寸可设置到的最大值，如果为-1则表示不限制")]
        private Vector2 m_MaxSize = new Vector2(-1, -1);

#if UNITY_EDITOR
        private bool m_IsEditorRunning = false;

        private void OnValidate()
        {
            if (m_LinkTarget == null && m_TargetSizeChangeSubscription != null)
            {
                m_TargetSizeChangeSubscription.Dispose();
                m_TargetSizeChangeSubscription = null;
            }
            if (m_IsEditorRunning)
            {
                if (m_LinkTarget != null)
                {
                    if (m_TargetSizeChangeSubscription == null)
                    {
                        SubscribeTargetSizeChange();
                    }
                }
                if (m_LayoutElement != null && m_LayoutElement.IsActive())
                {
                    LayoutRebuilder.MarkLayoutForRebuild(m_LayoutElement.transform as RectTransform);
                }
            }
        }
#endif

        private void OnEnable()
        {
#if UNITY_EDITOR
            m_IsEditorRunning = true;
#endif
            if (m_LayoutElement == null)
            {
                m_LayoutElement = GetComponent<LayoutElement>();
            }
            if (m_LinkTarget != null)
            {
                SubscribeTargetSizeChange();
            }
        }

        private void SubscribeTargetSizeChange()
        {
            if (m_LinkTarget != null)
            {
                if (m_TargetSizeChangeSubscription != null)
                {
                    m_TargetSizeChangeSubscription.Dispose();
                }
                m_TargetSizeChangeSubscription = m_LinkTarget.ObservedRectDemensionsChange(OnTargetSizeChange);
            }
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            m_IsEditorRunning = false;
#endif
            if (m_TargetSizeChangeSubscription != null)
            {
                m_TargetSizeChangeSubscription.Dispose();
                m_TargetSizeChangeSubscription = null;
            }
        }

        private void OnTargetSizeChange(RectTransform target)
        {
            if (m_LayoutElement == null)
            {
                m_LayoutElement = GetComponent<LayoutElement>();
            }
            if (m_LayoutElement == null)
            {
                return;
            }
            if (m_ControlWidth)
            {
                float _width = ClampSize(target.rect.width, m_MinSize.x, m_MaxSize.x);
                m_LayoutElement.minWidth = _width;
                m_LayoutElement.preferredWidth = _width;
            }
            if (m_ControlHeight)
            {
                float _height = ClampSize(target.rect.height, m_MinSize.y, m_MaxSize.y);
                m_LayoutElement.minHeight = _height;
                m_LayoutElement.preferredHeight = _height;
            }
        }

        private float ClampSize(float value, float min, float max)
        {
            if (min >= 0)
            {
                value = Mathf.Max(value, min);
            }
            if (max >= 0)
            {
                value = Mathf.Min(value, max);
            }
            return value;
        }
    }
}