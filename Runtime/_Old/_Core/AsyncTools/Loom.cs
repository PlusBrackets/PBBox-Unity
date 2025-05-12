using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox
{
    /// <summary>
    /// 异步线程与Unity主线程交互的工具，利用Update触发Action调用UnityEngine里原本无法在其他线程中访问的组件
    /// </summary>
    [AddComponentMenu("")]
    public class Loom : SingleBehaviour<Loom>
    {
        /// <summary>
        /// 预先创建，防止在异步线程中创建
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void _CreateSelf()
        {
            if (!HasInstance)
            {
                Create();
            }
        }

        public enum TriggerTiming
        {
            OnUpdate,
            OnLateUpdate,
            OnFixedUpdate
        }

        protected override bool hideInHierarchy => true;

        internal interface IActionHandler
        {
            void InvokeAction();
        }

        public class ActionHandler<TResult> : IActionHandler
        {
            internal Func<TResult> Action { get; private set; }
            public bool IsDone { get; private set; }
            public TResult Result { get; private set; }
            private Task<TResult> _task;
            public Task<TResult> ActionTask
            {
                get
                {
                    if (_task == null)
                    {
                        lock (Action)
                        {
                            if (_task == null)
                            {
                                _task = Task<TResult>.Run(WaitForResult);
                            }
                        }
                    }
                    return _task;
                }
            }

            internal ActionHandler(Func<TResult> action)
            {
                this.Action = action;
                this.IsDone = false;
                this._task = null;
                this.Result = default(TResult);
            }

            private Task<TResult> WaitForResult()
            {
                while (!IsDone && Action != null)
                {
                    Thread.Sleep(1);
                }
                return Task.FromResult(Result);
            }

            void IActionHandler.InvokeAction()
            {
                if (IsDone) return;
                Result = Action.Invoke();
                Action = null;
                IsDone = true;
            }
        }

        private Lazy<List<IActionHandler>> _actionQueue = new Lazy<List<IActionHandler>>();
        private Lazy<List<IActionHandler>> _invokingQueue = new Lazy<List<IActionHandler>>();

        private Lazy<List<IActionHandler>> _actionQueue_LateUpdate = new Lazy<List<IActionHandler>>();
        private Lazy<List<IActionHandler>> _invokingQueue_LateUpdate = new Lazy<List<IActionHandler>>();

        private Lazy<List<IActionHandler>> _actionQueue_FixedUpdate = new Lazy<List<IActionHandler>>();
        private Lazy<List<IActionHandler>> _invokingQueue_FixedUdpate = new Lazy<List<IActionHandler>>();

        protected override void InitAsInstance()
        {
            base.InitAsInstance();
        }

        public static ActionHandler<bool> QueueOnMainThread(Action action, TriggerTiming timing = TriggerTiming.OnUpdate)
        {
            return QueueOnMainThread(() => { action.Invoke(); return true; }, timing);
        }

        public static ActionHandler<TResult> QueueOnMainThread<TResult>(Func<TResult> func, TriggerTiming timing = TriggerTiming.OnUpdate)
        {
            return Instance._QueueOnMainThread(func, timing);
        }

        private ActionHandler<TResult> _QueueOnMainThread<TResult>(Func<TResult> func, TriggerTiming timing = TriggerTiming.OnUpdate)
        {
            lock (this)
            {
                Lazy<List<IActionHandler>> actions = null;
                switch (timing)
                {
                    case TriggerTiming.OnLateUpdate:
                        actions = _actionQueue_LateUpdate;
                        break;
                    case TriggerTiming.OnFixedUpdate:
                        actions = _actionQueue_FixedUpdate;
                        break;
                    default:
                        actions = _actionQueue;
                        break;
                }
                ActionHandler<TResult> actionHandler = new ActionHandler<TResult>(func);

                actions.Value.Add(actionHandler);
                return actionHandler;
            }
        }

        private void DoAction(Lazy<List<IActionHandler>> actions, Lazy<List<IActionHandler>> invokings)
        {
            lock (this)
            {
                if (actions.IsValueCreated)
                {
                    invokings.Value.AddRange(actions.Value);
                    actions.Value.Clear();
                }
            }
            if (invokings.IsValueCreated)
            {
                foreach (var a in invokings.Value)
                {
                    a.InvokeAction();
                }
                invokings.Value.Clear();
            }
        }

        private void Update()
        {
            DoAction(_actionQueue, _invokingQueue);
        }

        private void LateUpdate()
        {
            DoAction(_actionQueue_LateUpdate, _invokingQueue_LateUpdate);
        }

        private void FixedUpdate()
        {
            DoAction(_actionQueue_FixedUpdate, _invokingQueue_FixedUdpate);
        }

    }
}