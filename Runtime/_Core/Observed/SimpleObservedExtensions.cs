/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using UnityEngine;

namespace PBBox
{
    public static partial class SimpleObservedExtensions
    {
        /// <summary>
        /// 订阅观察
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static SimpleObservable<T>.Subscription Subscribe<T>(this SimpleObservable<T> target, Action<T> onNext, Action onComplete = null, Action<Exception> onError = null)
        {
            return target.Subscribe(new SimpleObserver<T>(onNext, onComplete, onError));
        }

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
        /// 观察Enable
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static SimpleObservable<GameObject>.Subscription ObservedEnable(this GameObject target, Action<GameObject> onNext, Action onComplete = null)
        {
            return target.GetOrAddComponent<ObjectEnableObserved>().GetEnableObserved().Subscribe(onNext, onComplete);
        }

        /// <summary>
        /// 观察Disable
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static SimpleObservable<GameObject>.Subscription ObservedDisable(this GameObject target, Action<GameObject> onNext, Action onComplete = null)
        {
            return target.GetOrAddComponent<ObjectEnableObserved>().GetDisableObserved().Subscribe(onNext, onComplete);
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