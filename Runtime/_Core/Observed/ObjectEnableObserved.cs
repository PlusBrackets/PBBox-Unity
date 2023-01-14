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
    [AddComponentMenu("PBBox/Observed/Object Enable Observerd")]
    public class ObjectEnableObserved : MonoBehaviour
    {
        SimpleObservable<GameObject> m_SubjectEnable;
        SimpleObservable<GameObject> m_SubjectDisable;

        public SimpleObservable<GameObject> GetEnableObserved()
        {
            return m_SubjectEnable ?? (m_SubjectEnable = new SimpleObservable<GameObject>());
        }

        public SimpleObservable<GameObject> GetDisableObserved()
        {
            return m_SubjectDisable ?? (m_SubjectDisable = new SimpleObservable<GameObject>());
        }

        void OnEnable()
        {
            m_SubjectEnable?.OnNext(gameObject);
        }

        void OnDisable()
        {
            m_SubjectDisable?.OnNext(gameObject);
        }

        void OnDestroy()
        {
            m_SubjectEnable?.OnCompleted();
            m_SubjectDisable?.OnCompleted();
        }

    }
}