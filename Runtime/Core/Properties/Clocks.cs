/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.26
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox.Properties
{
    public static partial class Clocks
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

        private static Clock _gameTime = new Clock();
        private static Clock _stopwatchCached = new Clock();
        private static Clock _systemCached = new Clock();

        /// <summary>
        /// 一个基于游戏缩放时间的时钟实例
        /// </summary>
        public static IClock GameTime => _gameTime;
        /// <summary>
        /// 一个基于计时器的缓存时钟实例，每帧自动更新一次缓存
        /// </summary>
        public static IClock StopwatchCached => _stopwatchCached;
        /// <summary>
        /// 一个基于系统时间的缓存时钟实例，每帧自动更新一次缓存
        /// </summary>
        public static IClock SystemCached => _systemCached;

        /// <summary>
        /// 主动更新缓存时钟的时间
        /// </summary>
        public static void UpdateCachedClocks()
        {
            _stopwatchCached.SetTime(Stopwatch.Now);
            _systemCached.SetTime(System.Now);
        }

    }
}