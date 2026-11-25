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
    /// 计时器的轻量级结构体，提供计时器的基本功能，如开始时间、倒计时、经过时间等。
    /// </summary>
    public readonly struct TimerLite
    {
        public static readonly TimerLite Empty = new TimerLite(Clocks.Zero, 0, null);

        private readonly IClock _clock;
        private readonly double _startTime;
        private readonly double _countdown;

        public IClock Clock => _clock;
        public double StartTime => _startTime;
        /// <summary>
        /// 获取计时器的倒计时时间，未设置倒计时则返回正无穷大
        /// </summary>
        public double Countdown => _countdown;
        public double Elapsed => _clock.Now - _startTime;
        /// <summary>
        /// 获取计时器的剩余时间
        /// </summary>
        public double Remaining => Math.Max(0, _countdown - Elapsed);
        /// <summary>
        /// 判断倒计时是否已经完成
        /// </summary>
        public bool IsCompleted => Elapsed >= _countdown;
        public bool HasCountdown => !double.IsPositiveInfinity(_countdown);
        public bool IsValid => _clock != null;

        public TimerSnapshot Snapshot => new TimerSnapshot(_startTime, _countdown);

        public TimerLite(IClock clock, double startTime, double? countdown = null)
        {
#if UNITY_EDITOR || PB_TEST_LOG
            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }
#endif
            _clock = clock;
            _startTime = startTime;
            _countdown = countdown.HasValue ? countdown.Value : double.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ElapsedAt(double now)
        {
            return now - _startTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCompletedAt(double now)
        {
            return now - _startTime >= _countdown;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double RemainingAt(double now)
        {
            return Math.Max(0, _countdown - (now - _startTime));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimerLite StartNow(IClock clock)
        {
            return new TimerLite(clock, clock.Now, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimerLite StartNow(IClock clock, double? countdown)
        {
            return new TimerLite(clock, clock.Now, countdown);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimerLite StartFrom(IClock clock, double startTime)
        {
            return new TimerLite(clock, startTime, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimerLite StartFrom(IClock clock, double startTime, double? countdown)
        {
            return new TimerLite(clock, startTime, countdown);
        }
    }

}