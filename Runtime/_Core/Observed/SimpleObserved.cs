/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace PBBox
{
    /// <summary>
    /// 简单可观察类,同时也为观察者,相当于事件触发器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SimpleObservable<T> : IObservable<T>, IObserver<T>//, IDisposable
    {
        bool isDisposed = false;
        bool isStoped = false;
        bool isObservarsOccupy => observars == nextingObservars;
        HashSet<IObserver<T>> observars;
        HashSet<IObserver<T>> nextingObservars;

        IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
        {
            if (isDisposed) throw new ObjectDisposedException(this.GetType().Name);
            if (isStoped) return Subscription.Empty;
            if (observars == null) observars = new HashSet<IObserver<T>>();
            else if (isObservarsOccupy) observars = new HashSet<IObserver<T>>(observars);
            if (observars.Add(observer))
            {
                return new Subscription(this, observer);
            }
            else
            {
                return Subscription.Empty;
            }
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public Subscription Subscribe(SimpleObserver<T> observer)
        {
            return (this as IObservable<T>).Subscribe(observer) as Subscription;
        }

        public void OnCompleted()
        {
            if (isDisposed) throw new ObjectDisposedException(this.GetType().Name);
            var _observers = observars;
            observars = null;
            isStoped = true;
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
        }

        public void OnNext(T value)
        {
            nextingObservars = observars;
            foreach (var o in nextingObservars)
            {
                o.OnNext(value);
            }
            nextingObservars = null;
        }

        public void OnError(Exception e)
        {
            if (isDisposed) throw new ObjectDisposedException(this.GetType().Name);
            var _observers = observars;
            observars = null;
            isStoped = true;
            foreach (var observer in _observers)
            {
                observer.OnError(e);
            }
        }

        // private void OnDispose()
        // {
        //     isDisposed = true;
        //     observars = null;
        // }

        // public void Dispose()
        // {
        //     OnDispose();
        //     GC.SuppressFinalize(this);
        // }

        public void DisposeObservar(IObserver<T> target)
        {
            if(isDisposed)
                return;
            if (isObservarsOccupy)
            {
                observars = new HashSet<IObserver<T>>(observars);
            }
            else
            {
                observars.Remove(target);
            }
        }

        public class Subscription : IDisposable
        {
            public readonly static Subscription Empty = new Subscription(null, null);

            SimpleObservable<T> parent;
            IObserver<T> target;

            public Subscription(SimpleObservable<T> parent, IObserver<T> target)
            {
                this.parent = parent;
                this.target = target;
            }

            public void Dispose()
            {
                if (parent != null && target != null && parent.observars != null)
                {
                    parent.DisposeObservar(target);
                }
                parent = null;
                target = null;
            }
        }
    }

    /// <summary>
    /// 简单观察者，用于订阅SimpleObservable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SimpleObserver<T> : IObserver<T>
    {
        readonly Action<T> onNext;
        readonly Action onCompleted;
        readonly Action<Exception> onError;

        public SimpleObserver(Action<T> onNext, Action onCompleted, Action<Exception> onError)
        {
            this.onNext = onNext;
            this.onCompleted = onCompleted;
            this.onError = onError;
        }

        public void OnCompleted()
        {
            onCompleted?.Invoke();
        }

        public void OnNext(T value)
        {
            onNext?.Invoke(value);
        }

        public void OnError(Exception e)
        {
            onError?.Invoke(e);
        }
    }
}