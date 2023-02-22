using System;
/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

namespace PBBox
{

    public interface IGameTimer
    {
        bool IsOver();
        void Start(float duration);
        void Stop();
        void Pause();
        void Resume();
        Task WaitForEnd();
    }

    /// <summary>
    /// 游戏计时器,使用Time.time计时
    /// </summary>
    public class GameTimer : IGameTimer
    {
        public delegate void TimerStateChange(GameTimer timer, State state);

        public event TimerStateChange onStateChanged;
        
        public enum State
        {
            Started,
            Stoped,
            Paused
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        protected float startTime = 0;
        /// <summary>
        /// 持续时长
        /// </summary>
        /// <value></value>
        public float duration { get; set; } = 0;
        /// <summary>
        /// 取得当前时间
        /// </summary>
        protected virtual float currentTime => Time.time;
        /// <summary>
        /// 经过时间
        /// </summary>
        public float passTime
        {
            get => (state == State.Paused ? lastPauseTime : currentTime) - startTime;
            set
            {
                if (!isStoped)
                {
                    float offset = passTime - value;
                    startTime += passTime - value;
                    duration += passTime - value;
                }
            }
        }
        /// <summary>
        /// 倒计时
        /// </summary>
        public float countDown => Mathf.Max(0, duration - passTime);
        /// <summary>
        /// 时间进度
        /// </summary>
        public float progress => duration > 0 ? Mathf.Clamp01(passTime / duration) : 1;
        /// <summary>
        /// 上一次暂停时间
        /// </summary>
        protected float lastPauseTime = -1;
        private State m_State = State.Stoped;
        /// <summary>
        /// 当前timer状态
        /// </summary>
        /// <value></value>
        public State state
        {
            get => m_State;
            private set
            {
                if (value != m_State)
                {
                    m_State = value;
                    onStateChanged?.Invoke(this, state);
                }
            }
        }
        public bool isStarted => state == State.Started;
        public bool isStoped => state == State.Stoped;
        public bool isPaused => state == State.Paused;

        /// <summary>
        /// 是否开始计时且计时结束
        /// </summary>
        public bool IsOver()
        {
            if (state == State.Stoped)
                return false;
            return this.countDown <= 0;
        }

        /// <summary>
        /// 开始计时
        /// </summary>
        /// <param name="duration"></param>
        public void Start(float duration)
        {
            startTime = currentTime;
            this.duration = duration;
            lastPauseTime = -1;
            state = State.Started;
        }

        /// <summary>
        /// 重新开始上一次计时
        /// </summary>
        public void Restart()
        {
            Start(duration);
        }

        /// <summary>
        /// 停止计时
        /// </summary>
        public void Stop()
        {
            startTime = 0;
            duration = 0;
            lastPauseTime = -1;
            state = State.Stoped;
        }

        /// <summary>
        /// 暂停计时
        /// </summary>
        public void Pause()
        {
            if (state == State.Started)
            {
                state = State.Paused;
                lastPauseTime = currentTime;
            }
        }

        /// <summary>
        /// 继续计时
        /// </summary>
        public void Resume()
        {
            if (state == State.Paused)
            {
                state = State.Started;
                startTime += currentTime - lastPauseTime;
            }
        }

        /// <summary>
        /// 维持进度不变，缩放duration
        /// </summary>
        /// <param name="scaler"></param>
        public void Scale(float scaler)
        {
            var pass = this.passTime;
            duration *= scaler;
            startTime += (1 - scaler) * pass;
        }

        /// <summary>
        /// 等待至Over或者结束
        /// </summary>
        /// <returns></returns>
        public async Task WaitForEnd()
        {
            while (!isStoped && !IsOver())
            {
                await Task.Delay(Mathf.Max(1, Mathf.RoundToInt(countDown) * 500));
            }
            return;
        }

        public IEnumerator WaitForEndEnumerable()
        {
            while (!isStoped && !IsOver())
            {
                yield return null;
            }
        }
    }
        
    /// <summary>
    /// 游戏计时器，使用Time.unscaleTime计时
    /// </summary>
    public class GameUnscaleTimer : GameTimer
    {
        protected override float currentTime => Time.unscaledTime;
    }

    [System.Serializable]
    /// <summary>
    /// 游戏计时器，可以切换Time.unscaleTime或Time.time计时
    /// </summary>
    public class GameDualityTimer : GameTimer
    {
        [SerializeField]
        private bool m_UseUnscaleTime = false;
        public bool useUnscaleTime
        {
            get => m_UseUnscaleTime;
            set
            {
                if (value != m_UseUnscaleTime)
                {
                    m_UseUnscaleTime = value;
                    OnCurrentTimeStateChanged();
                }
            }
        }

        protected override float currentTime => m_UseUnscaleTime ? Time.unscaledTime : Time.time;

        public GameDualityTimer(bool useUnscaleTime = false) : base()
        {
            this.useUnscaleTime = useUnscaleTime;
        }

        private void OnCurrentTimeStateChanged()
        {
            if (!isStoped)
            {
                float timeOffset = Time.unscaledTime - currentTime;
                if (useUnscaleTime)
                {
                    startTime += timeOffset;
                    if (lastPauseTime >= 0)
                        lastPauseTime += timeOffset;
                }
                else
                {
                    startTime -= timeOffset;
                    if (lastPauseTime >= 0)
                        lastPauseTime -= timeOffset;
                }
            }

        }

    }

    public class SystemTimer : GameTimer
    {
        private long _createdTicks = DateTime.UtcNow.Ticks;
        protected override float currentTime => (float)((DateTime.UtcNow.Ticks - _createdTicks) / 10000) / 1000f;
    }

    public class ManualTimer : GameTimer
    {
        private float m_AdvancedSec = 0f;
        protected override float currentTime => m_AdvancedSec;

        public void Advance(float sec){
            m_AdvancedSec += sec;
        }
    }

}
