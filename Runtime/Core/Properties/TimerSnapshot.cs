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
        public static readonly TimerLite Empty = new TimerLite(IClock.Zero, 0, null);

        private readonly IClock _clock;
        private readonly double _startTime;
        private readonly double? _countdown;

        public IClock Clock => _clock;
        public double Now => _clock.Now;
        public double StartTime => _startTime;
        public double? Countdown => _countdown;
        public double Elapsed => _clock.Now - _startTime;
        /// <summary>
        /// 获取计时器的剩余时间，未设置倒计时则返回-1
        /// </summary>
        public double Remaining => _countdown.HasValue ? Math.Max(0, _countdown.Value - Elapsed) : -1;
        /// <summary>
        /// 判断倒计时是否已经完成，若未设置倒计时则返回false
        /// </summary>
        public bool IsCompleted => _countdown.HasValue && Elapsed >= _countdown.Value;

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
            _countdown = countdown;
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

namespace PBBox
{
    using PBBox.Properties;

    public static partial class PBExtensions
    {
        public static TimerLite StartTimerNow(this IClock clock, double? countdown = null)
        {
            return TimerLite.StartNow(clock, countdown);
        }

        public static TimerLite StartTimerFrom(this IClock clock, double startTime, double? countdown = null)
        {
            return TimerLite.StartFrom(clock, startTime, countdown);
        }
    }
}