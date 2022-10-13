/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.06.21
 *@author: PlusBrackets
 --------------------------------------------------------*/
//Define USE Odin
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
#if USE_ODIN
using Sirenix.OdinInspector;
#endif
//Define End
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace PBBox.UI
{
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("PBBox/UI/Components/Scroll View Drag Trigger")]
    public class ScrollViewDragTrigger : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [System.Serializable]
        public class DragEvent : UnityEvent<Direction4> { }
        [System.Serializable]
        public class DragToggleEvent : UnityEvent<Direction4, bool> { }
        public class ElasticDirectionState
        {
            public bool isReady { get; internal set; } = false;
            public float outDistance { get; internal set; } = 0f;
            internal GameUnscaleTimer holdTimer = new GameUnscaleTimer();
            public float passedTime => holdTimer.passTime;
            public bool isDragging{get; internal set;} = false;
        }

        private ScrollRect m_ScrollRect;
        public bool IsDragging { get; private set; }
        [Header("Elastic Events")]
        [Tooltip("检测弹性事件的方向")]
        public Direction4 checkElasticEventDirection = Direction4.Up | Direction4.Left | Direction4.Down | Direction4.Right;
        [Tooltip("触发弹性事件时需要拖动至超出的距离，ScrollView的movementType需要为Elastic")]
        public float triggerElasticEventDistance = 100f;
        [Tooltip("开始弹性事件时需要拖动至超出的距离，ScrollView的movementType需要为Elastic")]
        public float startElasticEventDistance = 10f;
        [Tooltip("触发ready后要持续多长时间触发，小于0时禁用")]
        public float elasticTriggerHoldDuration = -1f;
#if USE_ODIN
        [FoldoutGroup("Events")]
        [Tooltip("当拖动至可触发的距离是传入true，从可触发距离拖回时传入false")]
        public DragToggleEvent onElasticReadyStateChanged = new DragToggleEvent();
        [FoldoutGroup("Events")]
        [Tooltip("触发ready后不拖动（触发EndDrag）时触发")]
        public DragEvent onElasticTrigger = new DragEvent();
        [FoldoutGroup("Events")]
        [Tooltip("开始拖动时传入true，取消拖动时传入false")]
        public DragToggleEvent onElasticStateChanged = new DragToggleEvent();
#else
        [Tooltip("当拖动至可触发的距离是传入true，从可触发距离拖回时传入false")]
        public DragToggleEvent onElasticReadyStateChanged = new DragToggleEvent();
        [Tooltip("触发ready后不拖动（触发EndDrag）时触发")]
        public DragEvent onElasticTrigger = new DragEvent();
        [Tooltip("开始拖动时传入true，取消拖动时传入false")]
        public DragToggleEvent onElasticStateChanged = new DragToggleEvent();
#endif
        public bool CanTriggerElasticEvent
        {
            get
            {
                if (m_ScrollRect == null)
                    return false;
                return m_ScrollRect.movementType == ScrollRect.MovementType.Elastic;
            }
        }
        private Dictionary<Direction4, ElasticDirectionState> _elasticDictionStateMap = new Dictionary<Direction4, ElasticDirectionState>(){
            {Direction4.Up,new ElasticDirectionState()},
            {Direction4.Left,new ElasticDirectionState()},
            {Direction4.Down,new ElasticDirectionState()},
            {Direction4.Right,new ElasticDirectionState()},
        };

        private void Awake()
        {
            m_ScrollRect = GetComponent<ScrollRect>();
#if UNITY_EDITOR && GAME_TEST
            onElasticReadyStateChanged.AddListener((d, f) =>
            {
                DebugUtils.Test.Log("[ScrollViewDragTrigger] " + gameObject.name + " " + d.ToString() + (f ? " ready" : " cancel"));
            });
            onElasticTrigger.AddListener((d) =>
            {
                DebugUtils.Test.Log("[ScrollViewDragTrigger] " + gameObject.name + " " + d.ToString() + " trigger");
            });
#endif
        }

        private void OnEnable()
        {
            m_ScrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }

        private void OnDisable()
        {
            if (m_ScrollRect)
                m_ScrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
        }

        private void OnScrollValueChanged(Vector2 vector)
        {
            if (!IsDragging || !CanTriggerElasticEvent)
                return;
            var content = m_ScrollRect.content;
            var viewPortRect = m_ScrollRect.viewport.rect;
            _elasticDictionStateMap[Direction4.Up].outDistance = checkElasticEventDirection.HasFlag(Direction4.Up) ? -content.anchoredPosition.y : 0f;
            _elasticDictionStateMap[Direction4.Down].outDistance = checkElasticEventDirection.HasFlag(Direction4.Down) ? content.anchoredPosition.y - Mathf.Max(0, (content.rect.height - viewPortRect.height)) : 0f;
            _elasticDictionStateMap[Direction4.Left].outDistance = checkElasticEventDirection.HasFlag(Direction4.Left) ? content.anchoredPosition.x : 0f;
            _elasticDictionStateMap[Direction4.Right].outDistance = checkElasticEventDirection.HasFlag(Direction4.Right) ? -content.anchoredPosition.x - Mathf.Max(0, (content.rect.width - viewPortRect.width)) : 0f;
            foreach (var kp in _elasticDictionStateMap)
            {
                if(!kp.Value.isDragging && kp.Value.outDistance > startElasticEventDistance){
                    kp.Value.isDragging = true;
                    onElasticStateChanged.Invoke(kp.Key, true);
                }
                if (!kp.Value.isDragging)
                    continue;
                if (!kp.Value.isReady && kp.Value.outDistance > triggerElasticEventDistance)
                {
                    kp.Value.isReady = true;
                    kp.Value.holdTimer.Start(elasticTriggerHoldDuration);
                    onElasticReadyStateChanged.Invoke(kp.Key, true);
                }
                else if (kp.Value.isReady && kp.Value.outDistance <= triggerElasticEventDistance)
                {
                    kp.Value.isReady = false;
                    onElasticReadyStateChanged.Invoke(kp.Key, false);
                    kp.Value.holdTimer.Stop();
                }
            }
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            IsDragging = true;
            ResetElasticState();
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            IsDragging = false;
            TryTriggerElasticEvent();
        }

        //添加保险，避免没有正确EngDrag
        private void ResetElasticState()
        {
            foreach (var kp in _elasticDictionStateMap)
            {
                kp.Value.outDistance = 0f;
                if (kp.Value.isReady)
                {
                    kp.Value.isReady = false;
                    onElasticReadyStateChanged.Invoke(kp.Key, false);
                }
                kp.Value.holdTimer.Stop();
                if (kp.Value.isDragging)
                {
                    kp.Value.isDragging = false;
                    onElasticStateChanged.Invoke(kp.Key, false);
                }
            }
        }

        private void TryTriggerElasticEvent()
        {
            bool canTrigger = CanTriggerElasticEvent;
            foreach (var kp in _elasticDictionStateMap)
            {
                kp.Value.outDistance = 0f;
                if (kp.Value.isReady)
                {
                    kp.Value.isReady = false;
                    if (canTrigger && checkElasticEventDirection.HasFlag(kp.Key) && kp.Value.holdTimer.IsOver())
                    {
                        onElasticTrigger.Invoke(kp.Key);
                    }
                    else
                    {
                        onElasticReadyStateChanged.Invoke(kp.Key, false);
                    }
                }
                if (kp.Value.isDragging)
                {
                    kp.Value.isDragging = false;
                    onElasticStateChanged.Invoke(kp.Key, false);
                }
                kp.Value.holdTimer.Stop();
            }
        }

        public ElasticDirectionState GetElasticDirectionState(Direction4 direct)
        {
            _elasticDictionStateMap.TryGetValue(direct, out var value);
            return value;
        }
    }
}