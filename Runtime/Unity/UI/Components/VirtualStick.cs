/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.03.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PBBox.Unity.Input;

namespace PBBox.Unity.UI
{
    /// <summary>
    /// 虚拟摇杆UI
    /// </summary>
    [AddComponentMenu("PBBox/UI/Components/Virtual Stick")]
    public class VirtualStick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IStickInput
    {
        [Tooltip("虚拟摇杆背景组件")]
        [SerializeField]
        private RectTransform m_StickBackground;
        [Tooltip("虚拟摇杆本身，需要为StickBackground的子物体")]
        [SerializeField]
        private RectTransform m_StickHandle;
        [Tooltip("StickHandle可活动的自定义半径，若大于0，则取该半径设置而不是取StickBackground的尺寸")]
        [SerializeField, Min(0)]
        private float m_CustomRadius = 0;
        [Tooltip("是否开启动态原点模式，该模式在首次点击时会自动将摇杆移动至该位置")]
        [SerializeField]
        private bool m_DynamicOriginMode = false;
        [Tooltip("动态原点模式中是否忽略StickBackground自身的点击")]
        [SerializeField]
        private bool m_DynamicOriginModeIgnoreSelfTouch = false;
        [Tooltip("如果设置，当Drag时如果PointerEventData中hover不包括此GameObject，则中断输入")]
        [SerializeField]
        private RectTransform m_DragInterruptChecker;
        [SerializeField, Space]
        private UnityEngine.Events.UnityEvent m_OnInputBegin;
        [SerializeField]
        private UnityEngine.Events.UnityEvent m_OnInputEnd;

        private Vector3 m_BackgroundDefaultPos;
        private int? m_CurPointerId;

        public Vector2 InputVector { get; protected set; }
        public bool IsInputting { get; private set; }

        public bool DynamicOriginMode
        {
            get => m_DynamicOriginMode;
            set
            {
                m_DynamicOriginMode = value;
            }
        }

        public bool DynamicOriginModeIgnoreSelfTouch
        {
            get => m_DynamicOriginModeIgnoreSelfTouch;
            set
            {
                m_DynamicOriginModeIgnoreSelfTouch = value;
            }
        }

        public float CustomRadius
        {
            get => m_CustomRadius;
            set
            {
                m_CustomRadius = value;
            }
        }

        private void Start()
        {
            m_StickHandle.SetLocalPositionAndRotation(Vector2.zero, m_StickHandle.localRotation);
        }

        private void OnDisable()
        {
            Interrupt();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!m_CurPointerId.HasValue || m_CurPointerId.Value != eventData.pointerId)
            {
                return;
            }
            if (eventData.hovered.Count > 0 && m_DragInterruptChecker != null && !eventData.hovered.Contains(m_DragInterruptChecker.gameObject))
            {
                Interrupt();
                return;
            }
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_StickBackground, eventData.position, eventData.pressEventCamera, out var _localPoint))
            {
                Vector2 _range = m_CustomRadius > 0 ? new Vector2(m_CustomRadius, m_CustomRadius) : m_StickBackground.sizeDelta / 2f;
                _localPoint /= _range;
                InputVector = _localPoint.sqrMagnitude > 1f ? _localPoint.normalized : _localPoint;
                m_StickHandle.SetLocalPositionAndRotation(new Vector2(InputVector.x * _range.x, InputVector.y * _range.y), m_StickHandle.localRotation);
                // m_StickHandle.anchoredPosition = new Vector2(InputVector.x * _range.x, InputVector.y * _range.y);
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (m_CurPointerId.HasValue)
            {
                return;
            }
            m_CurPointerId = eventData.pointerId;
            m_BackgroundDefaultPos = m_StickBackground.anchoredPosition3D;
            if (m_DynamicOriginMode)
            {
                if (!(m_DynamicOriginModeIgnoreSelfTouch
                    && RectTransformUtility.RectangleContainsScreenPoint(m_StickBackground, eventData.position, eventData.pressEventCamera))
                    && RectTransformUtility.ScreenPointToLocalPointInRectangle(m_StickBackground.parent as RectTransform, eventData.position, eventData.pressEventCamera, out var _localPoint))
                {
                    m_StickBackground.SetLocalPositionAndRotation(_localPoint, m_StickBackground.localRotation);
                }
            }
            if (!IsInputting)
            {
                m_OnInputBegin?.Invoke();
            }
            IsInputting = true;
            ((IDragHandler)this).OnDrag(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (!m_CurPointerId.HasValue || m_CurPointerId.Value != eventData.pointerId)
            {
                return;
            }
            EndInput();
        }

        private void EndInput()
        {
            m_CurPointerId = null;
            m_StickHandle.SetLocalPositionAndRotation(Vector2.zero, m_StickHandle.localRotation);
            m_StickBackground.anchoredPosition3D = m_BackgroundDefaultPos;
            InputVector = Vector2.zero;
            if (IsInputting)
            {
                m_OnInputEnd?.Invoke();
            }
            IsInputting = false;
        }

        public void Interrupt()
        {
            if (!m_CurPointerId.HasValue)
            {
                return;
            }
            EndInput();
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (m_CustomRadius > 0 && m_StickBackground != null)
            {
                var _tempColor = Gizmos.color;
                var _tempMatrix = Gizmos.matrix;
                Gizmos.color = Color.green;
                Gizmos.matrix = m_StickBackground.localToWorldMatrix;
                GizmosUtils.DrawCircle(Vector3.zero, Vector3.forward, m_CustomRadius);
                // Gizmos.DrawWireSphere(Vector3.zero, m_CustomRadius);
                Gizmos.color = _tempColor;
                Gizmos.matrix = _tempMatrix;
            }
        }
#endif
    }
}