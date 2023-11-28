using System;
/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.21
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PBBox.UI
{
    /// <summary>
    /// 根据容器的变化调整RectTransfrom
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("PBBox/UI/Components/Rect Fit Container")]
    public class RectFitContainer : UIBehaviour
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

        [SerializeField]
        protected FitContainer m_FitContainer = FitContainer.Parent;

        [SerializeField, Tooltip("是否适应宽度")]
        protected bool m_FitWidth = true;
        [SerializeField, Tooltip("是否适应高度")]
        protected bool m_FitHeight = false;
        [SerializeField, Tooltip("若全适应则选取更大的scale，否则选取更小的scale")]
        protected bool m_CompareLarge = false;
        [SerializeField, Tooltip("改变自身Transform的Size还是Scale")]
        protected FitType m_FitType = FitType.Size;
        //[Tooltip("缩放指数")]
        //public float scalePow = 1f;
        [SerializeField, Tooltip("是否保持在容器中的大小比例。false则填充容器")]
        protected bool m_KeepOriginDesignSize = true;
        //public bool resizeEverEnable = true;
        [SerializeField, Tooltip("是否使用ScaleRange来限制改变程度")]
        protected bool m_ClampScale = true;
        [SerializeField, Attributes.MinMaxRangeAttribute(0f, 50f), Tooltip("缩放范围")]
        protected Vector2 m_ScaleRange = new Vector2(0, 3);
        [SerializeField]
        protected bool m_IsObservedParent = false;
        [SerializeField]
        protected bool m_ExecultInEditor = false;

        private RectTransform m_RectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (m_RectTransform == null)
                {
                    m_RectTransform = transform as RectTransform;
                }
                return m_RectTransform;
            }
        }

        public Vector2? DefaultSize { get; protected set; } = null;
        public Vector3? DefaultScale { get; protected set; } = null;

        private Canvas m_Canvas;
        private CanvasScaler m_CanvasScaler;
        private Vector2? m_ParentOriginSize;
        private IDisposable m_ParentSizeChangeSubscription;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            //Log.Debug($"OnValidate {name}");
            if (!CheckCanExecuteInEditor())
            {
                DefaultScale = null;
                DefaultSize = null;
                m_Canvas = null;
                m_CanvasScaler = null;
                m_ParentOriginSize = null;
                m_ParentSizeChangeSubscription?.Dispose();
                m_ParentSizeChangeSubscription = null;
            }
            else
            {
                TryInit();
            }
        }

        private bool CheckCanExecuteInEditor()
        {
            return m_ExecultInEditor || UnityEditor.EditorApplication.isPlaying;
        }
#endif

        protected override void Awake()
        {
            base.Awake();
#if UNITY_EDITOR
            if (!CheckCanExecuteInEditor())
            {
                return;
            }
#endif
            //Log.Debug($"Awake {name}");
            TryInit();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
#if UNITY_EDITOR
            if (!CheckCanExecuteInEditor())
            {
                return;
            }
#endif
            //Log.Debug($"OnEnable {name}");
            TryResize();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (m_ParentSizeChangeSubscription != null)
            {
                m_ParentSizeChangeSubscription?.Dispose();
                m_ParentSizeChangeSubscription = null;
            }
        }

        private bool TryInit()
        {
            //Log.Debug($"InitParam {name}");
            bool _result = true;
            TryInitDefaultSize();
            _result = TryInitCanvasData();
            _result = TryInitParentOriginSize() && _result;

            if (m_IsObservedParent && m_ParentSizeChangeSubscription == null)
            {
                var _parentRect = RectTransform.parent as RectTransform;
                m_ParentSizeChangeSubscription = _parentRect.ObservedRectDemensionsChange(OnParentSizeChanged);
            }
            return _result;
        }

        private void TryInitDefaultSize()
        {
            if (DefaultSize == null)
            {
                DefaultSize = RectTransform.rect.size;
            }
            if (DefaultScale == null)
            {
                DefaultScale = RectTransform.localScale;
            }

        }

        private bool TryInitCanvasData()
        {
            if (m_FitContainer != FitContainer.Canvas)
            {
                return true;
            }
            if (m_Canvas == null)
            {
                m_Canvas = GetComponentInParent<Canvas>();
            }
            if (m_CanvasScaler == null && m_Canvas != null)
            {
                m_CanvasScaler = m_Canvas.GetComponent<CanvasScaler>();
            }
            return m_CanvasScaler && m_Canvas;
        }

        private bool TryInitParentOriginSize()
        {
            if (m_ParentOriginSize == null)
            {
                m_ParentOriginSize = (RectTransform.parent as RectTransform).rect.size;
            }
            if (m_ParentOriginSize.Value.IsAlmostEqual(Vector2.zero))
            {
                m_ParentOriginSize = null;
            }
            return m_ParentOriginSize != null;
        }

        //#if UNITY_EDITOR
        //        protected override void OnRectTransformDimensionsChange()
        //        {
        //            base.OnRectTransformDimensionsChange();
        //            if (!CheckCanExecuteInEditor())
        //            {
        //                return;
        //            }
        //            if (UnityEditor.Selection.Contains(gameObject))
        //            {
        //                Log.Debug("Reset Default Size:" + Time.frameCount);
        //                DefaultSize = null;
        //                DefaultScale = null;
        //                TryInitDefaultSize();
        //            }
        //            //Log.Debug($"OnRectTransformDimensionsChange {name}");
        //        }
        //#endif

        protected override void OnBeforeTransformParentChanged()
        {
            base.OnBeforeTransformParentChanged();
            //Log.Debug($"OnBeforeTransformParentChanged {name}");
#if UNITY_EDITOR
            if (!CheckCanExecuteInEditor())
            {
                return;
            }
#endif
            m_ParentOriginSize = null;
            if (m_ParentSizeChangeSubscription != null)
            {
                m_ParentSizeChangeSubscription?.Dispose();
                m_ParentSizeChangeSubscription = null;
            }
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            //Log.Debug($"OnTransformParentChanged {name}");
#if UNITY_EDITOR
            if (!CheckCanExecuteInEditor())
            {
                return;
            }
#endif
            TryInitParentOriginSize();
            if (m_IsObservedParent && m_ParentSizeChangeSubscription == null)
            {
                var _parentRect = RectTransform.parent as RectTransform;
                m_ParentSizeChangeSubscription = _parentRect.ObservedRectDemensionsChange(OnParentSizeChanged);
            }
        }

        protected override void OnCanvasGroupChanged()
        {
            base.OnCanvasGroupChanged();
#if UNITY_EDITOR
            if (!CheckCanExecuteInEditor())
            {
                return;
            }
#endif
            m_Canvas = null;
            m_CanvasScaler = null;
            TryInitCanvasData();
            //m_Canvas = GetComponentInParent<Canvas>();
            //if (m_Canvas != null)
            //{
            //    m_CanvasScaler = m_Canvas.GetComponent<CanvasScaler>();
            //}
            //Log.Debug($"OnCanvasGroupChanged {name}");
        }

        protected void OnParentSizeChanged(RectTransform parent)
        {
            if (!m_IsObservedParent)
            {
                return;
            }
            if (m_FitContainer == FitContainer.Parent)
            {
                TryResize();
            }
        }

        public void UpdateFitContainer(Vector2? fitTargetOriginSize = null)
        {
            if (m_FitContainer == FitContainer.Canvas)
            {
                m_Canvas = GetComponentInParent<Canvas>();
                m_CanvasScaler = m_Canvas.GetComponent<CanvasScaler>();
            }
            if (fitTargetOriginSize != null)
            {
                m_ParentOriginSize = fitTargetOriginSize.Value;
            }
            else
            {
                m_ParentOriginSize = (RectTransform.parent as RectTransform).rect.size;
            }
        }

        public void TryResize()
        {
#if UNITY_EDITOR
            if (!CheckCanExecuteInEditor())
            {
                return;
            }
#endif
            if (!TryInit())
            {
                return;
            }
            // var scale = compareLarge ? 0 : float.MaxValue;
            //if (m_RectTransform == null)
            //{
            //    m_RectTransform = transform as RectTransform;
            //}

            RectTransform _fitTarget = null;
            Vector2? _targetOriginSize = Vector2.one;
            if (m_FitContainer == FitContainer.Canvas)
            {
                _targetOriginSize = m_CanvasScaler.referenceResolution;
                _fitTarget = m_Canvas.transform as RectTransform;
            }
            else if (m_FitContainer == FitContainer.Parent)
            {
                _targetOriginSize = m_ParentOriginSize;
                _fitTarget = RectTransform.parent as RectTransform;
            }
            if (!m_KeepOriginDesignSize)
            {
                _targetOriginSize = DefaultSize;
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
            if (m_ClampScale)
                _scaleRange = m_ScaleRange;
            var scale = GetFitScale(RectTransform, _fitTarget, m_FitWidth, m_FitHeight, m_CompareLarge, _targetOriginSize, 1, _scaleRange);
            switch (m_FitType)
            {
                case FitType.Size:
                    var toSize = DefaultSize.Value * scale;
                    RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, toSize.x);
                    RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, toSize.y);
                    break;
                case FitType.Scale:
                    RectTransform.localScale = DefaultScale.Value * scale;
                    break;
            }
            Log.Debug($"Resize {name} {scale} , {Time.frameCount}", this, "RectFitContainer", Log.PBBoxLoggerName);
        }

        /// <summary>
        /// 获取合适的scale
        /// </summary>
        /// <param name="rect">需要适应的rect</param>
        /// <param name="fitTarget">适应的目标</param>
        /// <param name="fitWidth">是否适应宽度</param>
        /// <param name="fitHeight">是否适应高度</param>
        /// <param name="compareLarge">若全适应择是否取较大的scale，否择取较小</param>
        /// <param name="targetOriginSize">设计大小，使用fitTarget.size/designSize取scale大小，为空则取rect的size作为designSize</param>
        /// <param name="scalePow">缩放指数</param>
        /// <param name="scaleRange">缩放范围，为空则不使用</param>
        /// <returns></returns>
        protected static float GetFitScale(RectTransform rect, RectTransform fitTarget, bool fitWidth = true, bool fitHeight = true, bool compareLarge = true, Vector2? targetOriginSize = null, float scalePow = 1f, Vector2? scaleRange = null)
        {
            var _scale = compareLarge ? 0 : float.MaxValue;
            if (targetOriginSize == null || targetOriginSize.Value.IsAlmostEqual(Vector2.zero))
            {
                targetOriginSize = rect.rect.size;
            }
            if (fitWidth)
            {
                float _scaleW = targetOriginSize.Value.x.IsAlmostEqual(0) ? 1 : (fitTarget.rect.width / targetOriginSize.Value.x);
                _scale = _scaleW;
            }
            if (fitHeight)
            {
                float _scaleH = targetOriginSize.Value.y.IsAlmostEqual(0) ? 1 : (fitTarget.rect.height / targetOriginSize.Value.y);
                if (compareLarge && _scale < _scaleH || !compareLarge && _scale > _scaleH)
                    _scale = _scaleH;
            }
            if (scaleRange != null)
            {
                _scale = Mathf.Clamp(_scale, scaleRange.Value.x, scaleRange.Value.y);
            }
            return _scale;
        }
    }
}