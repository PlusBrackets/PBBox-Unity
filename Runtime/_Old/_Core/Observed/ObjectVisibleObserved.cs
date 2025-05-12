/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox
{
    [DisallowMultipleComponent]
    [AddComponentMenu("PBBox/Observed/Renderer Visible Observerd")]
    public class ObjectVisibleObserved : MonoBehaviour
    {
        SimpleObservable<GameObject> m_SubjectVisible;
        SimpleObservable<GameObject> m_SubjectInvisible;

        public SimpleObservable<GameObject> GetVisibleObserved()
        {
            return m_SubjectVisible ?? (m_SubjectVisible = new SimpleObservable<GameObject>());
        }

        public SimpleObservable<GameObject> GetInvisibleObserved()
        {
            return m_SubjectInvisible ?? (m_SubjectInvisible = new SimpleObservable<GameObject>());
        }

        private void OnBecameVisible()
        {
            m_SubjectVisible?.OnNext(gameObject);
        }

        private void OnBecameInvisible()
        {
            m_SubjectInvisible?.OnNext(gameObject);
        }

        void OnDestroy()
        {
            m_SubjectVisible?.OnCompleted();
            m_SubjectInvisible?.OnCompleted();
        }

    }

    public static partial class SimpleObservedExtensions
    {
        /// <summary>
        /// 观察Enable
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static SimpleObservable<GameObject>.Subscription ObservedVisible(this GameObject target, Action<GameObject> onNext, Action onComplete = null)
        {
            return target.GetOrAddComponent<ObjectVisibleObserved>().GetVisibleObserved().Subscribe(onNext, onComplete);
        }

        /// <summary>
        /// 观察Disable
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static SimpleObservable<GameObject>.Subscription ObservedInvisible(this GameObject target, Action<GameObject> onNext, Action onComplete = null)
        {
            return target.GetOrAddComponent<ObjectVisibleObserved>().GetInvisibleObserved().Subscribe(onNext, onComplete);
        }
    }
}