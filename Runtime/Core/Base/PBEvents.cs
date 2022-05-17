/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PBBox
{
    /// <summary>
    /// 全局事件
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
    public class PBEvents : SingleClass<PBEvents>
    {
        readonly Dictionary<Enum, Delegate> _delegates = new Dictionary<Enum, Delegate>();
        readonly Dictionary<object, Delegate> _delegates2 = new Dictionary<object, Delegate>();

        PBEvents() { }

        protected override void OnDispose()
        {
            base.OnDispose();
            _delegates.Clear();
            _delegates2.Clear();
        }
        
        #region On, Add Listener
        public static void On(Enum key, Action listener)
        {
            Instance._On(key, listener);
        }
        public static void On<T>(Enum key, Action<T> listener)
        {
            Instance._On(key, listener);
        }
        public static void On<T1, T2>(Enum key, Action<T1, T2> listener)
        {
            Instance._On(key, listener);
        }
        public static void On<T1, T2, T3>(Enum key, Action<T1, T2, T3> listener)
        {
            Instance._On(key, listener);
        }
        public static void On<T1, T2, T3, T4>(Enum key, Action<T1, T2, T3, T4> listener)
        {
            Instance._On(key, listener);
        }
        void _On(Enum key, Delegate listener)
        {
            if (_delegates.TryGetValue(key, out var l))
            {
                _delegates[key] = Delegate.Combine(l, listener);
            }
            else
            {
                _delegates[key] = listener;
            }
        }

        public static void On(object key, Action listener)
        {
            Instance._On(key, listener);
        }
        public static void On<T>(object key, Action<T> listener)
        {
            Instance._On(key, listener);
        }
        public static void On<T1, T2>(object key, Action<T1, T2> listener)
        {
            Instance._On(key, listener);
        }
        public static void On<T1, T2, T3>(object key, Action<T1, T2, T3> listener)
        {
            Instance._On(key, listener);
        }
        public static void On<T1, T2, T3, T4>(object key, Action<T1, T2, T3, T4> listener)
        {
            Instance._On(key, listener);
        }
        void _On(object key, Delegate listener)
        {
            if (_delegates2.TryGetValue(key, out var l))
            {
                _delegates2[key] = Delegate.Combine(l, listener);
            }
            else
            {
                _delegates2[key] = listener;
            }
        }

        #endregion

        #region Off, Remove Listener
        public static void Off(Enum key)
        {
            Instance._delegates.Remove(key);
        }
        public static void Off(Enum key, Action listener)
        {
            Instance._Off(key, listener);
        }
        public static void Off<T>(Enum key, Action<T> listener)
        {
            Instance._Off(key, listener);
        }
        public static void Off<T1, T2>(Enum key, Action<T1, T2> listener)
        {
            Instance._Off(key, listener);
        }
        public static void Off<T1, T2, T3>(Enum key, Action<T1, T2, T3> listener)
        {
            Instance._Off(key, listener);
        }
        public static void Off<T1, T2, T3, T4>(Enum key, Action<T1, T2, T3, T4> listener)
        {
            Instance._Off(key, listener);
        }
        void _Off(Enum key, Delegate listener)
        {
            if (_delegates.TryGetValue(key, out var l))
            {
                var curDel = Delegate.Remove(l, listener);
                if (curDel != null)
                {
                    _delegates[key] = curDel;
                }
                else
                {
                    _delegates.Remove(key);
                }
            }
        }

        public static void Off(object key)
        {
            Instance._delegates2.Remove(key);
        }
        public static void Off(object key, Action listener)
        {
            Instance._Off(key, listener);
        }
        public static void Off<T>(object key, Action<T> listener)
        {
            Instance._Off(key, listener);
        }
        public static void Off<T1, T2>(object key, Action<T1, T2> listener)
        {
            Instance._Off(key, listener);
        }
        public static void Off<T1, T2, T3>(object key, Action<T1, T2, T3> listener)
        {
            Instance._Off(key, listener);
        }
        public static void Off<T1, T2, T3, T4>(object key, Action<T1, T2, T3, T4> listener)
        {
            Instance._Off(key, listener);
        }
        void _Off(object key, Delegate listener)
        {
            if (_delegates2.TryGetValue(key, out var l))
            {
                var curDel = Delegate.Remove(l, listener);
                if (curDel != null)
                {
                    _delegates2[key] = curDel;
                }
                else
                {
                    _delegates2.Remove(key);
                }
            }
        }
        #endregion

        #region Emit, Dispatch Event
        public static void Emit(Enum key)
        {
            var delegates = Instance._delegates;
            if (delegates.TryGetValue(key, out var l))
            {
                (l as Action)?.Invoke();
            }
        }
        public static void Emit<T>(Enum key, T arg1)
        {
            var delegates = Instance._delegates;
            if (delegates.TryGetValue(key, out var l))
            {
                (l as Action<T>)?.Invoke(arg1);
            }
        }
        public static void Emit<T1, T2>(Enum key, T1 arg1, T2 arg2)
        {
            var delegates = Instance._delegates;
            if (delegates.TryGetValue(key, out var l))
            {
                (l as Action<T1, T2>)?.Invoke(arg1, arg2);
            }
        }
        public static void Emit<T1, T2, T3>(Enum key, T1 arg1, T2 arg2, T3 arg3)
        {
            var delegates = Instance._delegates;
            if (delegates.TryGetValue(key, out var l))
            {
                (l as Action<T1, T2, T3>)?.Invoke(arg1, arg2, arg3);
            }
        }
        public static void Emit<T1, T2, T3, T4>(Enum key, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var delegates = Instance._delegates;
            if (delegates.TryGetValue(key, out var l))
            {
                (l as Action<T1, T2, T3, T4>)?.Invoke(arg1, arg2, arg3, arg4);
            }
        }

        public static void Emit(object key)
        {
            var delegates = Instance._delegates2;
            if (delegates.TryGetValue(key, out var l))
            {
                (l as Action)?.Invoke();
            }
        }
        public static void Emit<T>(object key, T arg1)
        {
            var delegates = Instance._delegates2;
            if (delegates.TryGetValue(key, out var l))
            {
                (l as Action<T>)?.Invoke(arg1);
            }
        }
        public static void Emit<T1, T2>(object key, T1 arg1, T2 arg2)
        {
            var delegates = Instance._delegates2;
            if (delegates.TryGetValue(key, out var l))
            {
                (l as Action<T1, T2>)?.Invoke(arg1, arg2);
            }
        }
        public static void Emit<T1, T2, T3>(object key, T1 arg1, T2 arg2, T3 arg3)
        {
            var delegates = Instance._delegates2;
            if (delegates.TryGetValue(key, out var l))
            {
                (l as Action<T1, T2, T3>)?.Invoke(arg1, arg2, arg3);
            }
        }
        public static void Emit<T1, T2, T3, T4>(object key, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var delegates = Instance._delegates2;
            if (delegates.TryGetValue(key, out var l))
            {
                (l as Action<T1, T2, T3, T4>)?.Invoke(arg1, arg2, arg3, arg4);
            }
        }
        #endregion
    }
}