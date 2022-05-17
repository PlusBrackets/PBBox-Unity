/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using UnityEngine;

namespace PBBox
{
    public static class SimpleObservedExtensions
    {
        /// <summary>
        /// 订阅观察
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static SimpleObservable<T>.Subscription Subscribe<T>(this SimpleObservable<T> target, Action<T> onNext)
        {
            return target.Subscribe(new SimpleObserver<T>(onNext, null, null));
        }

        /// <summary>
        /// 观察Destroy,当GameObejct Destroy时触发回调
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <returns></returns>
        public static SimpleObservable<object>.Subscription ObservedDestroy(this GameObject target, Action<object> onNext)
        {
            return GetOrAddComponent<ObjectDestroyObserved>(target).GetObserved().Subscribe(onNext);
        }

        /// <summary>
        /// 观察Destroy,当GameObejct Destroy时触发回调
        /// </summary>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <returns></returns>
        public static SimpleObservable<object>.Subscription ObservedDestroy(this Component target, Action<object> onNext)
        {
            return target.gameObject.ObservedDestroy(onNext);
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
            GetOrAddComponent<ObjectDestroyObserved>(obj).AddDisposableOnDestroy(target);
            return target;
        }

        static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }
    }
}