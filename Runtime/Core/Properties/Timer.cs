/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.25
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Runtime.CompilerServices;

namespace PBBox.Properties
{
    /// <summary>
    /// 计时器类，提供计时器的基本功能，如开始、暂停、停止、倒计时等。
    /// </summary>
    public sealed partial class Timer
    {
        private TimerLite _timer;

        public IClock Clock { get; private set; }
        /// <summary>
        /// 获取计时器的开始时间，未开始则返回null
        /// </summary>
        public double? StartTime { get; private set; } = null;
        /// <summary>
        /// 获取计时器的上一次暂停时间，未暂停则返回null
        /// </summary>
        public double? LastPausedTime { get; private set; } = null;
        /// <summary>
        /// 计时器上一次暂停的总暂停时间
        /// </summary>
        public double LastPausedTotalTime { get; private set; } = 0;
        /// <summary>
        /// 获取计时器的倒计时时间，未设置倒计时则返回null
        /// </summary>
        public double Countdown => _timer.Countdown;
        public double Elapsed => _timer.Elapsed;
        /// <summary>
        /// 获取计时器从开始到当前的总经过时间，包含暂停的时间
        /// </summary>
        public double TotalElapsed => StartTime.HasValue ? Clock.Now - StartTime.Value : 0;
        /// <summary>
        /// 获取计时器的剩余时间，未设置倒计时则返回-1
        /// </summary>
        public double Remaining => _timer.Remaining;
        public bool HasCountdown => _timer.HasCountdown;

        /// <summary>
        /// 判断倒计时是否已经完成，若未设置倒计时则返回false
        /// </summary>
        public bool IsCompleted => _timer.IsCompleted;
        public bool IsStarted => StartTime.HasValue;
        public bool IsPaused => LastPausedTime.HasValue;
        /// <summary>
        /// 判断计时器是否正在运行，即已经启动且未暂停
        /// </summary>
        public bool IsRunning => IsStarted && !IsPaused;
        public TimerSnapshot Snapshot => _timer.Snapshot;


        public Timer(IClock clock)
        {
#if UNITY_EDITOR || PB_TEST_LOG
            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }
#endif
            Clock = clock;
            Stop();
        }

        /// <summary>
        /// 启动计时器，若提供倒计时则从倒计时开始计时
        /// </summary>
        /// <param name="countdown"></param>
        public void Start(double? countdown = null)
        {
            Stop();
            _timer = TimerLite.StartNow(Clock, countdown);
            StartTime = _timer.StartTime;
        }

        /// <summary>
        /// 根据指定的起始时间启动计时器
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="countdown"></param>
        public void StartFrom(double startTime, double? countdown = null)
        {
            Stop();
            _timer = new TimerLite(Clock, startTime, countdown);
            StartTime = _timer.StartTime;
        }

        /// <summary>
        /// 根据计时器的状态信息启动计时器，注重状态信息
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="lastPausedTime"></param>
        /// <param name="lastPausedTotalTime"></param>
        /// <param name="countdown"></param>
        public void StartByStatus(double startTime, double? lastPausedTime, double lastPausedTotalTime, double? countdown = null)
        {
            Stop();
            StartTime = startTime;
            LastPausedTime = lastPausedTime;
            LastPausedTotalTime = lastPausedTotalTime;
            if (LastPausedTime.HasValue)
            {
                var elapsed = LastPausedTime.Value - StartTime.Value - LastPausedTotalTime;
                _timer = new TimerLite(Clocks.Zero, -elapsed, countdown);
            }
            else
            {
                _timer = new TimerLite(Clock, StartTime.Value + LastPausedTotalTime, countdown);
            }
        }

        /// <summary>
        /// 根据计时器的状态信息启动计时器，注重经过时间
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="totalElapsed"></param>
        /// <param name="isPaused"></param>
        /// <param name="countdown"></param>
        public void StartByStatus(double elapsed, double totalElapsed, bool isPaused, double? countdown = null)
        {
            Stop();
            var now = Clock.Now;
            StartTime = now - totalElapsed;
            LastPausedTotalTime = totalElapsed - elapsed;
            if (isPaused)
            {
                LastPausedTime = now;
                _timer = new TimerLite(Clocks.Zero, -elapsed, countdown);
            }
            else
            {
                _timer = new TimerLite(Clock, StartTime.Value + LastPausedTotalTime, countdown);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Stop()
        {
            _timer = TimerLite.Empty;
            StartTime = null;
            LastPausedTime = null;
            LastPausedTotalTime = 0;
        }

        public void Pause()
        {
            if (StartTime.HasValue && !LastPausedTime.HasValue)
            {
                LastPausedTime = Clock.Now;
                var elapsed = LastPausedTime.Value - _timer.StartTime;
                _timer = new TimerLite(Clocks.Zero, -elapsed, _timer.Countdown);
            }
        }

        public void Resume()
        {
            if (StartTime.HasValue && LastPausedTime.HasValue)
            {
                LastPausedTotalTime += Clock.Now - LastPausedTime.Value;
                var newStartTime = StartTime.Value + LastPausedTotalTime;
                _timer = new TimerLite(Clock, newStartTime, _timer.Countdown);
                LastPausedTime = null;
            }
        }

        /// <summary>
        /// 更换计时器使用的时钟，保持计时器的经过时间不变
        /// </summary>
        /// <param name="newClock"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void ChangeClock(IClock newClock)
        {
#if UNITY_EDITOR || PB_TEST_LOG
            if (newClock == null)
            {
                throw new ArgumentNullException(nameof(newClock));
            }
#endif
            if (Clock == newClock)
            {
                return;
            }
            var oldClock = Clock;
            Clock = newClock;
            if (!StartTime.HasValue)
            {
                // 没有开始时间，说明计时器未启动，无需调整
                return;
            }
            var offset = Clock.Now - oldClock.Now;
            // 调整开始时间以保证经过时间不受时钟变化影响
            StartTime += offset;
            if (LastPausedTime.HasValue)
            {
                // 如果计时器处于暂停状态，需要调整暂停时间以匹配新的时钟
                LastPausedTime = LastPausedTime.Value + offset;
            }
            else
            {
                // 如果计时器处于运行状态，重新创建计时器快照以匹配新的时钟
                var newStartTime = StartTime.Value + LastPausedTotalTime;
                _timer = new TimerLite(Clock, newStartTime, _timer.Countdown);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ElapsedAt(double now)
        {
            return _timer.ElapsedAt(now);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCompletedAt(double now)
        {
            return _timer.IsCompletedAt(now);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double RemainingAt(double now)
        {
            return _timer.RemainingAt(now);
        }
    }

}