/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace PBBox
{
    #region some unityEvent
    [Serializable]
    public class UEvent_Object : UnityEvent<object> { }
    [Serializable]
    public class UEvent_String : UnityEvent<string> { }
    [Serializable]
    public class UEvent_Int : UnityEvent<int> { }
    [Serializable]
    public class UEvent_Float : UnityEvent<float> { }
    [Serializable]
    public class UEvent_Bool : UnityEvent<bool> { }
    #endregion

    /// <summary>
    /// 静态全局事件，比PBEvents性能好，但灵活性不足
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class PBEvent<T>
    {
        public delegate void Event(T ev);
        public static event PBEvent<T>.Event Listener;
        public static void Emit(T ev) { Listener.Invoke(ev); }
    }

    /// <summary>
    /// 事件分发器
    /// </summary>
    public partial class PBEvents : SingleClass<PBEvents>
    {
        readonly Dictionary<Enum, MyDelegateCollection> _delegates = new Dictionary<Enum, MyDelegateCollection>();
        readonly Dictionary<object, MyDelegateCollection> objKeyDelegates = new Dictionary<object, MyDelegateCollection>();

        #region functions
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _delegates.Clear();
            objKeyDelegates.Clear();
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">监听执行顺序,默认为0</param>
        /// <param name="isOnce">是否只执行一次</param>
        void _AddListener(Enum key, Delegate listener, int order, bool isOnce)
        {
            if (_delegates.TryGetValue(key, out var l))
            {
                l.AddDelegate(listener, order, isOnce);
            }
            else
            {
                _delegates[key] = new MyDelegateCollection();
                _delegates[key].AddDelegate(listener, order, isOnce);
            }
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">监听执行顺序,默认为0</param>
        /// <param name="isOnce">是否只执行一次</param>
        void _AddListener(object key, Delegate listener, int order, bool isOnce)
        {
            if (objKeyDelegates.TryGetValue(key, out var l))
            {
                l.AddDelegate(listener, order, isOnce);
            }
            else
            {
                objKeyDelegates[key] = new MyDelegateCollection();
                objKeyDelegates[key].AddDelegate(listener, order, isOnce);
            }
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
        void _RemoveListener(Enum key, Delegate listener, int? order)
        {
            if (_delegates.TryGetValue(key, out var l))
            {
                l.RemoveDelegate(listener, order);
            }
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
        void _RemoveListener(object key, Delegate listener, int? order)
        {
            if (objKeyDelegates.TryGetValue(key, out var l))
            {
                l.RemoveDelegate(listener, order);
            }
        }
        #endregion

        #region static On, Add Listener
        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">自定义执行顺序，默认为0</param>
        public static void On(Enum key, Delegate listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, false);
        }
        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">自定义执行顺序，默认为0</param>
        public static void On(Enum key, Action listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, false);
        }
        public static void On<T>(Enum key, Action<T> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, false);
        }
        public static void On<T1, T2>(Enum key, Action<T1, T2> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, false);
        }
        public static void On<T1, T2, T3>(Enum key, Action<T1, T2, T3> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, false);
        }
        public static void On<T1, T2, T3, T4>(Enum key, Action<T1, T2, T3, T4> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, false);
        }

        /// <summary>
        /// 添加监听,只执行一次
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">自定义执行顺序，默认为0</param>
        public static void Once(Enum key, Delegate listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, true);
        }
        /// <summary>
        /// 添加监听，只执行一次
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">自定义执行顺序，默认为0</param>
        public static void Once(Enum key, Action listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, true);
        }
        public static void Once<T>(Enum key, Action<T> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, true);
        }
        public static void Once<T1, T2>(Enum key, Action<T1, T2> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, true);
        }
        public static void Once<T1, T2, T3>(Enum key, Action<T1, T2, T3> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, true);
        }
        public static void Once<T1, T2, T3, T4>(Enum key, Action<T1, T2, T3, T4> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, true);
        }


        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">自定义执行顺序，默认为0</param>
        public static void On(object key, Delegate listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, false);
        }
        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">自定义执行顺序，默认为0</param>
        public static void On(object key, Action listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, false);
        }
        public static void On<T>(object key, Action<T> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, false);
        }
        public static void On<T1, T2>(object key, Action<T1, T2> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, false);
        }
        public static void On<T1, T2, T3>(object key, Action<T1, T2, T3> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, false);
        }
        public static void On<T1, T2, T3, T4>(object key, Action<T1, T2, T3, T4> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, false);
        }

        /// <summary>
        /// 添加监听,只执行一次
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">自定义执行顺序，默认为0</param>
        public static void Once(object key, Delegate listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, true);
        }
        /// <summary>
        /// 添加监听,只执行一次
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">自定义执行顺序，默认为0</param>
        public static void Once(object key, Action listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, true);
        }
        public static void Once<T>(object key, Action<T> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, true);
        }
        public static void Once<T1, T2>(object key, Action<T1, T2> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, true);
        }
        public static void Once<T1, T2, T3>(object key, Action<T1, T2, T3> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, true);
        }
        public static void Once<T1, T2, T3, T4>(object key, Action<T1, T2, T3, T4> listener, int order = 0)
        {
            Instance._AddListener(key, listener, order, true);
        }

        #endregion

        #region static Off, Remove Listener
        /// <summary>
        /// 移除所有监听
        /// </summary>
        /// <param name="key"></param>
        public static void Off(Enum key)
        {
            if (!HasInstance)
                return;
            Instance._delegates.Remove(key);
        }
        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
        public static void Off(Enum key, Delegate listener, int? order = null)
        {
            if (!HasInstance)
                return;
            Instance._RemoveListener(key, listener, order);
        }
        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
        public static void Off(Enum key, Action listener, int order = 0)
        {
            if (!HasInstance)
                return;
            Instance._RemoveListener(key, listener, order);
        }
        public static void Off<T>(Enum key, Action<T> listener, int? order = null)
        {
            if (!HasInstance)
                return;
            Instance._RemoveListener(key, listener, order);
        }
        public static void Off<T1, T2>(Enum key, Action<T1, T2> listener, int? order = null)
        {
            if (!HasInstance)
                return;
            Instance._RemoveListener(key, listener, order);
        }
        public static void Off<T1, T2, T3>(Enum key, Action<T1, T2, T3> listener, int? order = null)
        {
            if (!HasInstance)
                return;
            Instance._RemoveListener(key, listener, order);
        }
        public static void Off<T1, T2, T3, T4>(Enum key, Action<T1, T2, T3, T4> listener, int? order = null)
        {
            if (!HasInstance)
                return;
            Instance._RemoveListener(key, listener, order);
        }

        /// <summary>
        /// 移除所有监听
        /// </summary>
        /// <param name="key"></param>
        public static void Off(object key)
        {
            if (!HasInstance)
                return;
            Instance.objKeyDelegates.Remove(key);
        }
        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
        public static void Off(object key, Delegate listener, int? order = null)
        {
            if (!HasInstance)
                return;
            Instance._RemoveListener(key, listener, order);
        }
        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
        /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
        public static void Off(object key, Action listener, int order = 0)
        {
            if (!HasInstance)
                return;
            Instance._RemoveListener(key, listener, order);
        }
        public static void Off<T>(object key, Action<T> listener, int? order = null)
        {
            if (!HasInstance)
                return;
            Instance._RemoveListener(key, listener, order);
        }
        public static void Off<T1, T2>(object key, Action<T1, T2> listener, int? order = null)
        {
            if (!HasInstance)
                return;
            Instance._RemoveListener(key, listener, order);
        }
        public static void Off<T1, T2, T3>(object key, Action<T1, T2, T3> listener, int? order = null)
        {
            if (!HasInstance)
                return;
            Instance._RemoveListener(key, listener, order);
        }
        public static void Off<T1, T2, T3, T4>(object key, Action<T1, T2, T3, T4> listener, int? order = null)
        {
            if (!HasInstance)
                return;
            Instance._RemoveListener(key, listener, order);
        }

        #endregion

        #region static Emit, Dispatch Event

        public static void Emit(Enum key, params object[] args)
        {
            if (!HasInstance)
                return;
            var delegates = Instance._delegates;
            if (delegates.TryGetValue(key, out var l))
            {
                l.TravelForInvoke(d =>
                {
                    bool success = true;
                    try
                    {
                        d.DynamicInvoke(args);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    return success;
                });
            }
        }

        public static void Emit(Enum key)
        {
            if (!HasInstance)
                return;
            var delegates = Instance._delegates;
            if (delegates.TryGetValue(key, out var l))
            {
                l.TravelForInvoke(d =>
                {
                    if (d is Action _d)
                    {
                        _d.Invoke();
                        return true;
                    }
                    return false;
                });
            }
        }
        public static void Emit<T>(Enum key, T arg1)
        {
            if (!HasInstance)
                return;
            var delegates = Instance._delegates;
            if (delegates.TryGetValue(key, out var l))
            {
                l.TravelForInvoke(d =>
                {
                    if (d is Action<T> _d)
                    {
                        _d.Invoke(arg1);
                        return true;
                    }
                    return false;
                });
            }
        }
        public static void Emit<T1, T2>(Enum key, T1 arg1, T2 arg2)
        {
            if (!HasInstance)
                return;
            var delegates = Instance._delegates;
            if (delegates.TryGetValue(key, out var l))
            {
                l.TravelForInvoke(d =>
                {
                    if (d is Action<T1, T2> _d)
                    {
                        _d.Invoke(arg1, arg2);
                        return true;
                    }
                    return false;
                });
            }
        }
        public static void Emit<T1, T2, T3>(Enum key, T1 arg1, T2 arg2, T3 arg3)
        {
            if (!HasInstance)
                return;
            var delegates = Instance._delegates;
            if (delegates.TryGetValue(key, out var l))
            {
                l.TravelForInvoke(d =>
                {
                    if (d is Action<T1, T2, T3> _d)
                    {
                        _d.Invoke(arg1, arg2, arg3);
                        return true;
                    }
                    return false;
                });
            }
        }
        public static void Emit<T1, T2, T3, T4>(Enum key, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (!HasInstance)
                return;
            var delegates = Instance._delegates;
            if (delegates.TryGetValue(key, out var l))
            {
                l.TravelForInvoke(d =>
                {
                    if (d is Action<T1, T2, T3, T4> _d)
                    {
                        _d.Invoke(arg1, arg2, arg3, arg4);
                        return true;
                    }
                    return false;
                });
            }
        }

        public static void Emit(object key, params object[] args)
        {
            if (!HasInstance)
                return;
            var delegates = Instance.objKeyDelegates;
            if (delegates.TryGetValue(key, out var l))
            {
                l.TravelForInvoke(d =>
                {
                    bool success = true;
                    try
                    {
                        d.DynamicInvoke(args);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    return success;
                });
            }
        }

        public static void Emit(object key)
        {
            if (!HasInstance)
                return;
            var delegates = Instance.objKeyDelegates;
            if (delegates.TryGetValue(key, out var l))
            {
                l.TravelForInvoke(d =>
                {
                    if (d is Action _d)
                    {
                        _d.Invoke();
                        return true;
                    }
                    return false;
                });
            }
        }
        public static void Emit<T>(object key, T arg1)
        {
            if (!HasInstance)
                return;
            var delegates = Instance.objKeyDelegates;
            if (delegates.TryGetValue(key, out var l))
            {
                l.TravelForInvoke(d =>
                {
                    if (d is Action<T> _d)
                    {
                        _d.Invoke(arg1);
                        return true;
                    }
                    return false;
                });
            }
        }
        public static void Emit<T1, T2>(object key, T1 arg1, T2 arg2)
        {
            if (!HasInstance)
                return;
            var delegates = Instance.objKeyDelegates;
            if (delegates.TryGetValue(key, out var l))
            {
                l.TravelForInvoke(d =>
                {
                    if (d is Action<T1, T2> _d)
                    {
                        _d.Invoke(arg1, arg2);
                        return true;
                    }
                    return false;
                });
            }
        }
        public static void Emit<T1, T2, T3>(object key, T1 arg1, T2 arg2, T3 arg3)
        {
            if (!HasInstance)
                return;
            var delegates = Instance.objKeyDelegates;
            if (delegates.TryGetValue(key, out var l))
            {
                l.TravelForInvoke(d =>
                {
                    if (d is Action<T1, T2, T3> _d)
                    {
                        _d.Invoke(arg1, arg2, arg3);
                        return true;
                    }
                    return false;
                });
            }
        }
        public static void Emit<T1, T2, T3, T4>(object key, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (!HasInstance)
                return;
            var delegates = Instance.objKeyDelegates;
            if (delegates.TryGetValue(key, out var l))
            {
                l.TravelForInvoke(d =>
                {
                    if (d is Action<T1, T2, T3, T4> _d)
                    {
                        _d.Invoke(arg1, arg2, arg3, arg4);
                        return true;
                    }
                    return false;
                });
            }
        }
        #endregion

    }

}