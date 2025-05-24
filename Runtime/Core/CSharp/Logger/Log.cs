/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.12.15
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Diagnostics;
using System.Collections.Generic;
using System;
#if UNITY_2022_2_OR_NEWER
using UnityEngine;
#endif

namespace PBBox
{
    public static partial class Log
    {
        private interface ILogger
        {
            string Name { get; }
            bool Enable { get; set; }
            int LogLevel { get; set; }

            Action<object> LogDebug { get; }
            Action<object> LogInfo { get; }
            Action<object> LogWarning { get; }
            Action<object> LogError { get; }

            object DecoMessage(int level, string tag, object message);
        }

        private static Lazy<Dictionary<string, ILogger>> s_AdditionLoggers = new Lazy<Dictionary<string, ILogger>>(System.Threading.LazyThreadSafetyMode.None);
        private static ILogger s_DefaultLogger;
        private static Func<string, ILogger> s_LoggerCreator;

        internal static readonly string PBBoxLoggerName = "PBBox";

        private static void InitLogger(ILogger defaultLogger, Func<string, ILogger> loggerCreator)
        {
            s_DefaultLogger = defaultLogger;
            s_LoggerCreator = loggerCreator;
        }

        private static ILogger GetLogger(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return s_DefaultLogger;
            }
            ILogger logger;
            lock (s_AdditionLoggers)
            {
                if (!s_AdditionLoggers.Value.TryGetValue(name, out logger))
                {
                    logger = s_LoggerCreator(name);
                    s_AdditionLoggers.Value.Add(name, logger);
                }
            }
            return logger;
        }

        // [Conditional("PB_LOG_LEVEL_EDITOR_USE"), Conditional("UNITY_EDITOR")]
        public static void Enable(string loggerName = null)
        {
            GetLogger(loggerName).Enable = true;
        }

        // [Conditional("PB_LOG_LEVEL_EDITOR_USE"), Conditional("UNITY_EDITOR")]
        public static void Disable(string loggerName = null)
        {
            GetLogger(loggerName).Enable = false;
        }

        /// <summary>
        /// log的等级, 0:log any, 1:no debug, 2:warning and error, 3:error only
        /// </summary>
        /// <param name="loggerName"></param>
        /// <param name="logLevel">0:log any, 1:no debug, 2:warning and error, 3:error only</param>
        public static void SetLogLevel(string loggerName, int logLevel)
        {
            GetLogger(loggerName).LogLevel = logLevel;
        }

        private static bool CheckCanLog(ILogger logger, int logLevel)
        {
            return logger.Enable && logger.LogLevel <= logLevel;
        }

        #region Log Functions    
        /// <summary>
        /// Debug log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tag"></param>
        /// <param name="loggerName">Logger名称，如果不为空，则会取该名称的Logger进行输出</param>
#if PB_LOG_LEVEL_EDITOR_USE || !UNITY_EDITOR
        [Conditional("PB_LOG_0")]
#else
        [Conditional("UNITY_EDITOR")]
#endif
#if UNITY_2022_2_OR_NEWER
//Unity 2022中加入了HideInCallStack的特性，可以从trace中隐藏
        [HideInCallstack] 
#endif
        public static void Debug(object message, string tag = null, string loggerName = null)
        {
            ILogger logger = GetLogger(loggerName);
            if (CheckCanLog(logger, 0)) logger.LogDebug(logger.DecoMessage(0, tag, message));
        }

        /// <summary>
        /// Info log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tag"></param>
        /// <param name="loggerName">Logger名称，如果不为空，则会取该名称的Logger进行输出</param>
#if PB_LOG_LEVEL_EDITOR_USE || !UNITY_EDITOR
        [Conditional("PB_LOG_0"), Conditional("PB_LOG_1")]
#else
        [Conditional("UNITY_EDITOR")]
#endif
#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static void Info(object message, string tag = null, string loggerName = null)
        {
            ILogger logger = GetLogger(loggerName);
            if (CheckCanLog(logger, 1)) logger.LogInfo(logger.DecoMessage(1, tag, message));
        }

        /// <summary>
        /// Warning log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tag"></param>
        /// <param name="loggerName">Logger名称，如果不为空，则会取该名称的Logger进行输出</param>
#if PB_LOG_LEVEL_EDITOR_USE || !UNITY_EDITOR
        [Conditional("PB_LOG_0"), Conditional("PB_LOG_1"), Conditional("PB_LOG_2")]
#else
        [Conditional("UNITY_EDITOR")]
#endif
#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static void Warning(object message, string tag = null, string loggerName = null)
        {
            ILogger logger = GetLogger(loggerName);
            if (CheckCanLog(logger, 2)) logger.LogWarning(logger.DecoMessage(2, tag, message));
        }

        /// <summary>
        /// Error log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tag"></param>
        /// <param name="loggerName">Logger名称，如果不为空，则会取该名称的Logger进行输出</param>
#if PB_LOG_LEVEL_EDITOR_USE || !UNITY_EDITOR
        [Conditional("PB_LOG_0"), Conditional("PB_LOG_1"), Conditional("PB_LOG_2"), Conditional("PB_LOG_3")]
#else
        [Conditional("UNITY_EDITOR")]
#endif
#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static void Error(object message, string tag = null, string loggerName = null)
        {
            ILogger logger = GetLogger(loggerName);
            if (CheckCanLog(logger, 3)) logger.LogError(logger.DecoMessage(3, tag, message));
        }
        #endregion
    }
}