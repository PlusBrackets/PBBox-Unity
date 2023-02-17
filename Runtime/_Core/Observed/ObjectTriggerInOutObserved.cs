using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PBBox
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class ObjectTriggerInOutObserved : MonoBehaviour
    {
        SimpleObservable<Collider> m_SubjectEnter;
        SimpleObservable<Collider> m_SubjectExit;

        public SimpleObservable<Collider> GetEnterObserved()
        {
            return m_SubjectEnter ?? (m_SubjectEnter = new SimpleObservable<Collider>());
        }

        public SimpleObservable<Collider> GetExitObserved()
        {
            return m_SubjectExit ?? (m_SubjectExit = new SimpleObservable<Collider>());
        }

        private void OnTriggerEnter(Collider other)
        {
            m_SubjectEnter?.OnNext(other);
        }

        private void OnTriggerExit(Collider other)
        {
            m_SubjectExit?.OnNext(other);
        }
    }

    public static partial class SimpleObservedExtensions
    {
        /// <summary>
        /// 观察Trigger Enter
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static SimpleObservable<Collider>.Subscription ObservedTriggerEnter(this GameObject target, Action<Collider> onNext, Action onComplete = null)
        {
            return target.GetOrAddComponent<ObjectTriggerInOutObserved>().GetEnterObserved().Subscribe(onNext, onComplete);
        }

        /// <summary>
        /// 观察Trigger Exit
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static SimpleObservable<Collider>.Subscription ObservedTriggerExit(this GameObject target, Action<Collider> onNext, Action onComplete = null)
        {
            return target.GetOrAddComponent<ObjectTriggerInOutObserved>().GetExitObserved().Subscribe(onNext, onComplete);
        }
    }
}