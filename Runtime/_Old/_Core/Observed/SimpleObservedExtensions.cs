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
    }
}