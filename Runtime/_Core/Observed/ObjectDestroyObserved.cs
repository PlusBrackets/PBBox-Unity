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
    [AddComponentMenu("PBBox/Observed/Object Destroy Observerd")]
    public class ObjectDestroyObserved : MonoBehaviour
    {
        private SimpleObservable<GameObject> m_Subject;
        private HashSet<IDisposable> m_Disposables;
        private bool m_IsDestroyed = false;

        public SimpleObservable<GameObject> GetObserved()
        {
            return m_Subject ?? (m_Subject = new SimpleObservable<GameObject>());
        }

        public void AddDisposableOnDestroy(IDisposable disposable)
        {
            if (m_IsDestroyed)
            {
                disposable.Dispose();
            }
            else
            {
                if (m_Disposables == null)
                {
                    m_Disposables = new HashSet<IDisposable>();
                }
                m_Disposables.Add(disposable);
            }
        }

        void OnDestroy()
        {
            m_IsDestroyed = true;
            if (m_Disposables != null)
            {
                foreach (var d in m_Disposables)
                {
                    d.Dispose();
                }
            }
            m_Disposables = null;
            m_Subject?.OnNext(gameObject);
            m_Subject?.OnCompleted();
        }

    }

    public static partial class SimpleObservedExtensions
    {
        /// <summary>
        /// 观察Destroy,当GameObejct Destroy时触发回调
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <returns></returns>
        public static SimpleObservable<GameObject>.Subscription ObservedDestroy(this GameObject target, Action<GameObject> onNext)
        {
            return target.GetOrAddComponent<ObjectDestroyObserved>().GetObserved().Subscribe(onNext);
        }

        /// <summary>
        /// 当对应的Object Destroy时销毁此观察订阅
        /// </summary>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static SimpleObservable<T>.Subscription DisposeWhenObjectDestroy<T>(this SimpleObservable<T>.Subscription target, GameObject obj)
        {
            obj.GetOrAddComponent<ObjectDestroyObserved>().AddDisposableOnDestroy(target);
            return target;
        }
    }
}