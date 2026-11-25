/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.25
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
#if UNITY_5_3_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace PBBox.Properties
{

    public interface IClock
    {
        double Now { get; }
    }

    /// <summary>
    /// 基础的时钟，允许用户手动设置和推进时间。
    /// </summary>
    public class Clock : IClock
    {
        private double _now;
        public double Now => _now;

        public Clock(double initialTime = 0)
        {
            _now = initialTime;
        }

        public void SetTime(double time)
        {
            _now = time;
        }

        public void Advance(double delta)
        {
            _now += delta;
        }
    }
    
    /// <summary>
    /// 一个Now始终为0的时钟
    /// </summary>
    internal sealed class ZeroClock : IClock
    {
        public double Now => 0;

        internal ZeroClock() { }
    }

    internal sealed partial class StopwatchClock : IClock
    {
#if UNITY_5_3_OR_NEWER
        [AutoStaticsCleanup]
#endif
        private static System.Diagnostics.Stopwatch _stopwatch = System.Diagnostics.Stopwatch.StartNew();

        public double Now => _stopwatch.Elapsed.TotalSeconds;

        internal StopwatchClock() { }
    }

    /// <summary>
    /// 基于系统时间的时钟实例，返回当前的UTC时间。
    /// </summary>
    internal sealed class SystemClock : IClock
    {
        public double Now => DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;

        internal SystemClock() { }
    }
}