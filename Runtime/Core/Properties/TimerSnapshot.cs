/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.26
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Runtime.CompilerServices;

namespace PBBox.Properties
{
    
    /// <summary>
    /// 计时器的快照结构体，记录计时器的开始时间和倒计时信息，用于在特定时间点获取计时器状态。
    /// </summary>
    public readonly struct TimerSnapshot
    {
        private readonly double _startTime;
        private readonly double _countdown;

        
        public double StartTime => _startTime;
        public double Countdown => _countdown;

        public TimerSnapshot(double startTime, double countdown)
        {
            _startTime = startTime;
            _countdown = countdown;
        }

        public TimerSnapshot(double startTime)
        {
            _startTime = startTime;
            _countdown = double.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Elapsed(double now)
        {
            return now - _startTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCompleted(double now)
        {
            return now - _startTime >= _countdown;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Remaining(double now)
        {
            return Math.Max(0, _countdown - (now - _startTime));
        }
    }
}