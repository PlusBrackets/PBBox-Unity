/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.03.23
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
using UnityEngine.UI;

namespace PBBox.Unity.UI
{
    /// <summary>
    /// 带有尺寸范围的ContentSizeFitter
    /// </summary>
    [AddComponentMenu("PBBox/UI/Components/Content Size Fitter With Range")]
    [ExecuteAlways]
    public partial class ContentSizeFitterWithRange : ContentSizeFitter
    {
        [SerializeField, Tooltip("当对应方向的FitMode为Min或者Preferred时Size可变化到的最大值")]
        private float m_MaxWidth = -1;
        [SerializeField, Tooltip("当对应方向的FitMode为Min或者Preferred时Size可变化到的最小值")]
        private float m_MinWidth = -1;
        [SerializeField, Tooltip("当对应方向的FitMode为Min或者Preferred时Size可变化到的最大值")]
        private float m_MaxHeight = -1;
        [SerializeField, Tooltip("当对应方向的FitMode为Min或者Preferred时Size可变化到的最小值")]
        private float m_MinHeight = -1;

        public float MaxWidth
        {
            get { return m_MaxWidth; }
            set { SetMinMaxValue(ref m_MaxWidth, value); }
        }

        public float MinWidth
        {
            get { return m_MinWidth; }
            set { SetMinMaxValue(ref m_MinWidth, value); }
        }

        public float MaxHeight
        {
            get { return m_MaxHeight; }
            set { SetMinMaxValue(ref m_MaxHeight, value); }
        }

        public float MinHeight
        {
            get { return m_MinHeight; }
            set { SetMinMaxValue(ref m_MinHeight, value); }
        }

        [System.NonSerialized] private RectTransform m_Rect = null;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                {
                    m_Rect = transform as RectTransform;
                }
                return m_Rect;
            }
        }

        #pragma warning disable 649
        private DrivenRectTransformTracker m_Tracker;
        #pragma warning restore 649

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        public override void SetLayoutHorizontal()
        {
            // base.SetLayoutHorizontal();
            m_Tracker.Clear();
            HandleSelfFittingAlongAxisWithRange(0);
        }

        public override void SetLayoutVertical()
        {
            // base.SetLayoutVertical();
            HandleSelfFittingAlongAxisWithRange(1);
        }

        private void SetMinMaxValue(ref float value, float newValue)
        {
            var _value = newValue < 0 ? -1 : newValue;
            if (_value != value)
            {
                value = _value;
                SetDirty();
            }
        }

        //ContentSizeFitter中的HandleSelfFittingAlongAxis无法重写，m_Tracker无法访问，故直接使用新的m_Tracker以及设置方法进行更改RectTranform的size
        private void HandleSelfFittingAlongAxisWithRange(int axis)
        {
            var _rectTransform = rectTransform;
            FitMode _fitting = (axis == 0 ? horizontalFit : verticalFit);
            if (_fitting == FitMode.Unconstrained)
            {
                // Keep a reference to the tracked transform, but don't control its properties:
                m_Tracker.Add(this, _rectTransform, DrivenTransformProperties.None);
                return;
            }

            m_Tracker.Add(this, _rectTransform, (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

            // Set size to min or preferred size
            float _size = 0;
            if (_fitting == FitMode.MinSize)
            {
                _size = LayoutUtility.GetMinSize(_rectTransform, axis);
            }
            else
            {
                _size = LayoutUtility.GetPreferredSize(_rectTransform, axis);
            }
            // Limit to size
            float _max = axis == 0 ? m_MaxWidth : m_MaxHeight;
            float _min = axis == 0 ? m_MinWidth : m_MinHeight;
            if (_max >= 0)
            {
                _size = Mathf.Min(_size, _max);
            }
            else if (_min >= 0)
            {
                _size = Mathf.Max(_size, _min);
            }
            _rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, _size);
        }

    }
}