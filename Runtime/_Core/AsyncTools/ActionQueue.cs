/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox
{
    /// <summary>
    /// action队列
    /// </summary>
    public class ActionQueue
    {   
        public const string NAME_Default = "__Default";
        public const string NAME_ResLoad = "__ResLoad";

        #region define
        public delegate bool SafetyChecker(object safety);

        public enum ExecuteMode
        {
            /// <summary>
            /// 自动执行，不保留action队列。
            /// </summary>
            Auto,
            /// <summary>
            /// 手动执行，不保留action队列。
            /// </summary>
            Manual,
            /// <summary>
            /// 手动执行，保留action队列。
            /// </summary>
            Manual_KeepQueue,
        }

        private struct _Action
        {
            public Action action;
            public Func<Task> actionAsync;
            public Func<bool> canAction;
            public bool multithreading;
            public int cost;

            private _Action(Func<bool> canAction, bool multithreading, int cost)
            {
                this.action = null;
                actionAsync = null;
                this.canAction = canAction;
                this.multithreading = multithreading;
                this.cost = cost;
            }

            public _Action(Action action, Func<bool> canAction, bool multithreading, int cost) : this(canAction, multithreading, cost)
            {
                this.action = action;
            }

            public _Action(Func<Task> action, Func<bool> canAction, int cost) : this(canAction, false, cost)
            {
                actionAsync = action;
            }
        }

        /// <summary>
        /// ActionQueue的标识名
        /// </summary>
        /// <value></value>
        public string name { get; private set; }
        List<_Action> m_ActionList;
        /// <summary>
        /// 是否正在执行中
        /// </summary>
        /// <value></value>
        public bool isActing { get; private set; } = false;
        // private bool m_Canceled = false;
        bool m_IsActionLooping = false;
        public ExecuteMode mode { get; private set; }
        public object safety { get; set; }
        /// <summary>
        /// safety检查，若返回false，则不能执行action
        /// </summary>
        public SafetyChecker safetyChecker;
        /// <summary>
        /// 执行的action有cost限制时，cost用完则插入一帧的等待，只对主线程action有效
        /// </summary>
        public int maxCost = 100;
        int currentCost;
        public bool isDestroyed{get;private set;} = false;

        GameDualityTimer delayTimer = new GameDualityTimer();

        #endregion
        #region static

        static Dictionary<string, ActionQueue> queues;

        /// <summary>
        /// 取得一个actionQueue
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static ActionQueue Get(string name, ExecuteMode mode = ExecuteMode.Auto)
        {
            if (queues == null)
            {
                queues = new Dictionary<string, ActionQueue>();
            }
            ActionQueue queue;
            if (!queues.TryGetValue(name, out queue))
            {
                queue = new ActionQueue(name, null, mode);
                queues.Add(name, queue);
            }
            return queue;
        }

        public static bool Has(string name)
        {
            if (queues != null)
            {
                return queues.ContainsKey(name);
            }
            return false;
        }

        public static void Remove(string name)
        {
            if (Has(name))
            {
                ActionQueue queue = queues[name];
                queues.Remove(name);
                queue.Destroy();
            }
        }

        #endregion

        /// <summary>
        /// Action队列
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        /// <param name="sourceQueue"></param>
        public ActionQueue(string name = null, object safety = null, ExecuteMode mode = ExecuteMode.Manual, ActionQueue sourceQueue = null)
        {
            this.name = name;
            m_ActionList = sourceQueue == null ? new List<_Action>() : new List<_Action>(sourceQueue.m_ActionList);
            isActing = false;
            // m_Canceled = false;
            this.safety = safety == null ? this : safety;
            this.mode = mode;
        }

        /// <summary>
        /// Action队列
        /// </summary>
        /// <param name="sourceQueue"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public ActionQueue(ActionQueue sourceQueue, ExecuteMode mode = ExecuteMode.Manual) : this(null, null, mode, sourceQueue) { }

        /// <summary>
        /// Action队列
        /// </summary>
        /// <param name="safety"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public ActionQueue(object safety, ExecuteMode mode = ExecuteMode.Manual) : this(null, safety, mode, null) { }

        /// <summary>
        /// Do
        /// </summary>
        /// <param name="action">执行异步Action</param>
        /// <param name="canAction">当开始执行Action前会判断canAction，若为false，则取消执行，方便异步时判断</param>
        /// <param name="cost">消耗的cost，默认最大cost为100,消耗完cost后会自动延后一帧</param>
        /// <returns></returns>
        public ActionQueue Do(Func<Task> action, Func<bool> canAction = null, int cost = 0)
        {
            m_ActionList.Add(new _Action(action, canAction, cost));
            if (mode == ExecuteMode.Auto)
            {
                Execute();
            }
            return this;
        }

        /// <summary>
        /// Do
        /// </summary>
        /// <param name="action">执行Action</param>
        /// <param name="canAction">当开始执行Action前会判断canAction，若为false，则取消执行，方便异步时判断</param>
        /// <param name="multithreading">是否使用多线程执行该Action，常用于消耗cpu时间的计算</param>
        /// <param name="cost">消耗的cost，默认最大cost为100,消耗完cost后会自动延后一帧</param>
        /// <returns></returns>
        public ActionQueue Do(Action action, Func<bool> canAction = null, bool multithreading = false, int cost = 0)
        {
            m_ActionList.Add(new _Action(action, canAction, multithreading, cost));
            if (mode == ExecuteMode.Auto)
            {
                Execute();
            }
            return this;
        }

        /// <summary>
        /// 等待（秒）
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public ActionQueue Delay(float second)
        {
            return Delay(Mathf.RoundToInt(second * 1000f));
        }

        /// <summary>
        /// 等待(毫秒)
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public ActionQueue Delay(int milliseconds)
        {
            return Do(async () =>
            {
                await Task.Delay(milliseconds);
                return;
            });
        }

        /// <summary>
        /// 使用Time.time或者Time.unscaleTime等待
        /// </summary>
        /// <param name="second"></param>
        /// <param name="useUnscaleTime"></param>
        /// <returns></returns>
        public ActionQueue DelayGameTime(float second, bool useUnscaleTime = false)
        {
            return Do(async () =>
            {
                delayTimer.useUnscaleTime = useUnscaleTime;
                delayTimer.Start(second);
                await delayTimer.WaitForEnd();
                // while (!delayTimer.IsOver())
                // {
                    // await Task.Delay(Mathf.Max(1, Mathf.RoundToInt(delayTimer.countDown) * 500));
                // }
                return;
            });
        }

        /// <summary>
        /// 直到返回true
        /// </summary>
        /// <param name="until"></param>
        /// <param name="checkInterval">调用频率(ms),受帧数限制</param>
        /// <returns></returns>
        public ActionQueue Until(Func<bool> until, int checkInterval = 1)
        {
            return Do(async () =>
            {
                checkInterval = Mathf.Max(1,checkInterval);
                while (!until.Invoke() && isActing)
                {
                    await Task.Delay(checkInterval);
                }
                return;
            });
        }

        /// <summary>
        /// 循环操作
        /// </summary>
        /// <param name="action"></param>
        /// <param name="canLoop">是否可以循环，例如 t=>t<5，则循环5次,t为当前循环的次数</param>
        /// <param name="loopInterval">循环的间隔（s）</param>
        /// <param name="useUnscaleTime"></param>
        /// <returns></returns>
        public ActionQueue Loop(Action action,Func<int,bool> canLoop,float loopInterval,bool useUnscaleTime = false){
            return Do(async ()=>{
                int times =  0;
                delayTimer.useUnscaleTime = useUnscaleTime;
                while (canLoop(times) && isActing)
                {
                    action?.Invoke();
                    times++;
                    delayTimer.Start(loopInterval);
                    await delayTimer.WaitForEnd();
                }
            });
        }
        
        async void StartActions()
        {
            if (m_IsActionLooping)
                return;
            m_IsActionLooping = true;
            int index = 0;
            while (isActing && safety != null && m_ActionList != null && index < m_ActionList.Count && (safetyChecker == null || safetyChecker.Invoke(safety)))
            {
                if (currentCost >= maxCost)
                {
                    await Task.Delay(1);
                    currentCost = 0;
                }
                _Action ac = m_ActionList[index];
                if (mode == ExecuteMode.Manual_KeepQueue)
                {
                    index++;
                }
                else
                {
                    m_ActionList.RemoveAt(index);
                }
                if (ac.canAction != null && !ac.canAction.Invoke())
                {
                    continue;
                }
                if (ac.actionAsync != null)
                {
                    await ac.actionAsync.Invoke();
                }
                else if (ac.action != null)
                {
                    if (ac.multithreading)
                    {
                        await Task.Run(ac.action);
                    }
                    else
                    {
                        ac.action.Invoke();
                    }
                }
                currentCost += ac.cost;
            }
            isActing = false;
            m_IsActionLooping = false;
        }

        async void WaitToStartAction(){
            while(m_IsActionLooping){
                await Task.Delay(1);
            }
            if(isActing){
                StartActions();
            }
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        public void Execute()
        {
            if (isActing)
                return;
            isActing = true;
            WaitToStartAction();
        }

        /// <summary>
        /// 取消后续执行，会根据mode的设置清空action队列
        /// </summary>
        public void Cancel()
        {
            if (isActing){
                // m_Canceled = true;
                isActing = false;
                delayTimer.Stop();
            }
            if (mode != ExecuteMode.Manual_KeepQueue)
            {
                m_ActionList.Clear();
            }
        }

        public void Destroy()
        {
            if (isDestroyed)
                return;
            isDestroyed = true;
            isActing = false;
            delayTimer.Stop();
            m_ActionList = null;
            safety = null;
        }
    }
}