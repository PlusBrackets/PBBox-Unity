/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.15
 *@author: PlusBrackets
 --------------------------------------------------------*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace PBBox.UI
{
    /// <summary>
    /// 根据父物体的大小来设置自身的大小
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public sealed class RectFitParentSize : UIBehaviour, ILayoutSelfController
    {

#pragma warning disable 649
        private DrivenRectTransformTracker m_Tracker;
#pragma warning restore 649

        [System.NonSerialized]
        private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        [Flags]
        public enum FitMode
        {
            Horizontal = 1,
            Vertical = 2,
        }

        [SerializeField]
        private FitMode m_FitMode = FitMode.Horizontal | FitMode.Vertical;
        [SerializeField, Tooltip("自身宽高比")]
        private float m_RectWHRatio = 1;
        [SerializeField, Tooltip("占父物体的比例"), Range(0, 1)]
        private float m_FitParentScale = 1;
        private bool m_IsSizeDirty = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        private void TryFitSize()
        {
            if (!m_IsSizeDirty)
            {
                return;
            }
            m_Tracker.Clear();
            var parent = rectTransform.parent as RectTransform;
            if (parent == null)
                return;
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.SizeDeltaY);
            Vector2 size = parent.rect.size;
            float ratio = m_RectWHRatio;
            if (ratio <= 0)
            {
                ratio = 1;
            }
            if (m_FitMode.HasFlag(FitMode.Horizontal))
            {
                size.x *= m_FitParentScale;
                size.y = size.x / ratio;
            }
            if (m_FitMode.HasFlag(FitMode.Vertical))
            {
                var size2 = parent.rect.size;
                size2.y *= m_FitParentScale;
                size2.x = size2.y * ratio;
                if (size.y > size2.y)
                {
                    size = size2;
                }
            }
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            m_IsSizeDirty = false;
        }

        public void SetLayoutHorizontal()
        {
            TryFitSize();
        }

        public void SetLayoutVertical()
        {
            TryFitSize();
        }

        private void SetDirty()
        {
            if (!IsActive())
                return;
            m_IsSizeDirty = true;
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }

#endif
    }
}