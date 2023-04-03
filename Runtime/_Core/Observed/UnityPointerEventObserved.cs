/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.03.31
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PBBox
{
    [DisallowMultipleComponent]
    [AddComponentMenu("PBBox/Observed/Object Pointer Click Observerd")]
    public class UnityPointerEventObserved : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        SimpleObservable<GameObject> m_SubjectPointerUp;
        SimpleObservable<GameObject> m_SubjectPointerDown;
        SimpleObservable<GameObject> m_SubjectPointerClick;

        public SimpleObservable<GameObject> GetPointerUpObserved()
        {
            return m_SubjectPointerUp ?? (m_SubjectPointerUp = new SimpleObservable<GameObject>());
        }

        public SimpleObservable<GameObject> GetPointerDownObserved()
        {
            return m_SubjectPointerDown ?? (m_SubjectPointerDown = new SimpleObservable<GameObject>());
        }

        public SimpleObservable<GameObject> GetPointerClickObserved()
        {
            return m_SubjectPointerClick ?? (m_SubjectPointerClick = new SimpleObservable<GameObject>());
        }

        void OnDestroy()
        {
            m_SubjectPointerUp?.OnCompleted();
            m_SubjectPointerDown?.OnCompleted();
            m_SubjectPointerClick?.OnCompleted();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_SubjectPointerUp?.OnNext(gameObject);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_SubjectPointerDown?.OnNext(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            m_SubjectPointerClick?.OnNext(gameObject);
        }
    }
    
    public static partial class SimpleObservedExtensions
    {
        /// <summary>
        /// Pointer Up
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static SimpleObservable<GameObject>.Subscription ObservedPointerUp(this GameObject target, Action<GameObject> onNext, Action onComplete = null)
        {
            return target.GetOrAddComponent<UnityPointerEventObserved>().GetPointerUpObserved().Subscribe(onNext, onComplete);
        }

        /// <summary>
        /// Pointer Down
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static SimpleObservable<GameObject>.Subscription ObservedPointerDown(this GameObject target, Action<GameObject> onNext, Action onComplete = null)
        {
            return target.GetOrAddComponent<UnityPointerEventObserved>().GetPointerDownObserved().Subscribe(onNext, onComplete);
        }

        /// <summary>
        /// Pointer Click
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static SimpleObservable<GameObject>.Subscription ObservedPointerClick(this GameObject target, Action<GameObject> onNext, Action onComplete = null)
        {
            return target.GetOrAddComponent<UnityPointerEventObserved>().GetPointerClickObserved().Subscribe(onNext, onComplete);
        }
    }
}