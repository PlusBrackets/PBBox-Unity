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
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("PBBox/Observed/UIBehaviour Observed")]
    public class UIBehaviourObserved : UnityEngine.EventSystems.UIBehaviour
    {
        SimpleObservable<RectTransform> m_SubjectRectDimensions;

        public SimpleObservable<RectTransform> GetEnableObserved()
        {
            return m_SubjectRectDimensions ?? (m_SubjectRectDimensions = new SimpleObservable<RectTransform>());
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            m_SubjectRectDimensions?.OnNext(transform as RectTransform);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_SubjectRectDimensions?.OnCompleted();
        }

    }

    public static partial class SimpleObservedExtensions
    {
        /// <summary>
        /// 观察RectTransform的size变动
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static SimpleObservable<RectTransform>.Subscription ObservedRectDemensionsChange(this RectTransform target, Action<RectTransform> onNext, Action onComplete = null)
        {
            return target.GetOrAddComponent<UIBehaviourObserved>().GetEnableObserved().Subscribe(onNext, onComplete);
        }

        /// <summary>
        /// 观察RectTransform的size变动
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static SimpleObservable<RectTransform>.Subscription ObservedRectDemensionsChange(this GameObject target, Action<RectTransform> onNext, Action onComplete = null)
        {
            if (target.transform is RectTransform _rect)
            {
                return _rect.ObservedRectDemensionsChange(onNext, onComplete);
            }
            else
            {
                Log.Error("Can not found RectTransform on target", target, "Observed");
                return null;
            }
        }
    }
}