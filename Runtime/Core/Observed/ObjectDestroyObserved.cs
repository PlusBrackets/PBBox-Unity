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
    [AddComponentMenu("")]
    public class ObjectDestroyObserved : MonoBehaviour
    {
        SimpleObservable<GameObject> m_Subject;
        HashSet<IDisposable> m_Disposables;
        bool isDestroyed = false;

        public SimpleObservable<GameObject> GetObserved()
        {
            return m_Subject ?? (m_Subject = new SimpleObservable<GameObject>());
        }

        public void AddDisposableOnDestroy(IDisposable disposable)
        {
            if (isDestroyed)
            {
                disposable.Dispose();
            }
            else
            {
                if (m_Disposables == null) m_Disposables = new HashSet<IDisposable>();
                m_Disposables.Add(disposable);
            }
        }

        void OnDestroy()
        {
            isDestroyed = true;
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
}