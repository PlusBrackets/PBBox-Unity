/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.25
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox.Properties
{

    internal sealed class ZeroClock : IClock
    {
        public double Now => 0;

        internal ZeroClock() { }
    }

    internal sealed class StopwatchClock : IClock
    {
        private static System.Diagnostics.Stopwatch _stopwatch = System.Diagnostics.Stopwatch.StartNew();

        public double Now => _stopwatch.Elapsed.TotalSeconds;

        internal StopwatchClock() { }
    }

    internal sealed class SystemClock : IClock
    {
        public double Now => DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;

        internal SystemClock() { }
    }

    public class ManualClock : IClock
    {
        private double _now;

        public double Now => _now;

        public ManualClock(double initialTime = 0)
        {
            _now = initialTime;
        }

        public void Advance(double seconds)
        {
            _now += seconds;
        }
    }

    public partial interface IClock
    {
        /// <summary>
        /// 一个Now始终为0的时钟实例
        /// </summary>
        public static IClock Zero { get; private set; } = new ZeroClock();
        /// <summary>
        /// 一个基于计时器的时钟实例
        /// </summary>
        public static IClock Stopwatch { get; private set; } = new StopwatchClock();
        /// <summary>
        /// 一个基于系统时间的时钟实例
        /// </summary>
        public static IClock System { get; private set; } = new SystemClock();

        double Now { get; }
    }

}