/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.21
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PBBox.UI
{
    /// <summary>
    /// 根据容器的变化调整RectTransfrom
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("PBBox/UI/Components/Rect Fit Container")]
    public class RectFitContainer : MonoBehaviour
    {
        public enum FitType
        {
            Size,
            Scale
        }

        public enum FitContainer
        {
            Canvas = 0,
            Parent = 1
        }
        public FitContainer fitContainer = FitContainer.Canvas;

        public bool fitWidth = true;
        public bool fitHeight = false;
        public bool compareLarge = false;
        public FitType fitType = FitType.Size;
        public float scalePow = 1f;
        [Tooltip("保持设计大小，按容器的占比。false则填充容器")]
        public bool keepDesignSize = true;
        public bool resizeEverEnable = true;
        public bool clampScale = true;
        [Attributes.MinMaxRangeAttribute(0f, 10f)]
        public Vector2 scaleRange = new Vector2(0, 3);

        public Vector2 defaultSize{get;set;} = Vector2.zero;
        public Vector3 defalutScale{get;set;} = Vector3.one;
        private RectTransform m_RectTransform;
        private Canvas m_Canvas;
        private CanvasScaler m_CanvasScaler;
        private bool isStarted = false;

        private Vector2 m_ParentDesignSize;

        private void Awake()
        {
            m_Canvas = GetComponentInParent<Canvas>();
            m_CanvasScaler = m_Canvas.GetComponent<CanvasScaler>();
            m_RectTransform = transform as RectTransform;
            defalutScale = m_RectTransform.localScale;
            defaultSize = m_RectTransform.rect.size;
            m_ParentDesignSize = (m_RectTransform.parent as RectTransform).rect.size;
        }

        private void OnEnable()
        {
            if (isStarted && resizeEverEnable)
                Resize();
        }

        private void Start()
        {
            isStarted = true;
            Resize();
        }

        public void UpdateFitContainer(Vector2? fitTargetDesignSize = null)
        {
            if(fitContainer == FitContainer.Canvas)
            m_Canvas = GetComponentInParent<Canvas>();
            m_CanvasScaler = m_Canvas.GetComponent<CanvasScaler>();
            if (fitTargetDesignSize != null)
            {
                m_ParentDesignSize = fitTargetDesignSize.Value;
            }
            else
            {
                m_ParentDesignSize = (m_RectTransform.parent as RectTransform).rect.size;
            }
        }

        public void Resize()
        {
            // var scale = compareLarge ? 0 : float.MaxValue;
            if(m_RectTransform==null)
            {
                m_RectTransform = transform as RectTransform;
            }

            Vector2 designSize = Vector2.one;
            RectTransform fitTarget = null;
            if (fitContainer == FitContainer.Canvas)
            {
                designSize = m_CanvasScaler.referenceResolution;
                fitTarget = m_Canvas.transform as RectTransform;
            }
            else if (fitContainer == FitContainer.Parent)
            {
                designSize = m_ParentDesignSize;
                fitTarget = m_RectTransform.parent as RectTransform;
            }
            if (!keepDesignSize)
            {
                designSize = defaultSize;
            }
            // if (fitWidth)
            // {
            //     float scaleW = fitRect.rect.width / designSize.x;
            //     scale = scaleW;
            // }
            // if (fitHeight)
            // {
            //     float scaleH = fitRect.rect.height / designSize.y;
            //     if (compareLarge && scale < scaleH || !compareLarge && scale > scaleH)
            //         scale = scaleH;
            // }
            // scale = Mathf.Pow(scale, scalePow);
            // if (clampScale)
            //     scale = Mathf.Clamp(scale, scaleRange.x, scaleRange.y);
            // Debug.Log(designSize);
            Vector2? _scaleRange = null;
            if (clampScale)
                _scaleRange = scaleRange;
            var scale = GetFitScale(m_RectTransform, fitTarget, fitWidth, fitHeight, compareLarge, designSize, scalePow, _scaleRange);
            switch (fitType)
            {
                case FitType.Size:
                    var toSize = defaultSize * scale;
                    m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, toSize.x);
                    m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, toSize.y);
                    break;
                case FitType.Scale:
                    m_RectTransform.localScale = defalutScale * scale;
                    break;
            }
        }

        /// <summary>
        /// 获取合适的scale
        /// </summary>
        /// <param name="rect">需要适应的rect</param>
        /// <param name="fitTarget">适应的目标</param>
        /// <param name="fitWidth">是否适应宽度</param>
        /// <param name="fitHeight">是否适应高度</param>
        /// <param name="compareLarge">若全适应择是否取较大的scale，否择取较小</param>
        /// <param name="designSize">设计大小，使用fitTarget.size/designSize取scale大小，为空则取rect的size作为designSize</param>
        /// <param name="scalePow">缩放指数</param>
        /// <param name="scaleRange">缩放范围，为空则不使用</param>
        /// <returns></returns>
        public static float GetFitScale(RectTransform rect, RectTransform fitTarget, bool fitWidth = true, bool fitHeight = true, bool compareLarge = true, Vector2? designSize = null, float scalePow = 1f, Vector2? scaleRange = null)
        {
            var scale = compareLarge ? 0 : float.MaxValue;
            if (designSize == null)
            {
                designSize = rect.rect.size;
            }
            if (fitWidth)
            {
                float scaleW = fitTarget.rect.width / designSize.Value.x;
                scale = scaleW;
            }
            if (fitHeight)
            {
                float scaleH = fitTarget.rect.height / designSize.Value.y;
                if (compareLarge && scale < scaleH || !compareLarge && scale > scaleH)
                    scale = scaleH;
            }
            if (scaleRange != null)
                scale = Mathf.Clamp(scale, scaleRange.Value.x, scaleRange.Value.y);
            return scale;
        }
    }
}