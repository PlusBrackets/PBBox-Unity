/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.12.15
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Diagnostics;
using UnityEngine;

namespace PBBox
{
    public static partial class Log
    {
        /// <summary>
        /// 静态初始化，调用Log时初始化默认logger。
        /// </summary>
        static Log()
        {
            InitLogger(new UnityLogger(null), CreateLogger);
        }

        private static ILogger CreateLogger(string name)
        {
            return new UnityLogger(name);
        }

        [Conditional("UNITY_EDITOR")]
        public static void SetNameColor(string loggerName, Color color)
        {
#if UNITY_EDITOR
            if (GetLogger(loggerName) is UnityLogger _logger)
            {
                _logger.NameColor = color;
            }
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void SetTagColor(string loggerName, Color color)
        {
#if UNITY_EDITOR
            if (GetLogger(loggerName) is UnityLogger _logger)
            {
                _logger.TagColor = color;
            }
#endif
        }

        #region Log Functions    
#if PB_LOG_LEVEL_EDITOR_USE || !UNITY_EDITOR
        [Conditional("PB_LOG_0")]
#else
        [Conditional("UNITY_EDITOR")]
#endif
#if UNITY_2022_2_OR_NEWER 
        [HideInCallstack] //Unity 2022中加入了HideInCallStack的特性，可以从trace中隐藏
#endif
        public static void Debug(object message, UnityEngine.Object context, string tag = null, string loggerName = null)
        {
            ILogger _logger = GetLogger(loggerName);
            if (!CheckCanLog(_logger, 0)) return;
            object deco_msg = _logger.DecoMessage(0, tag, message);
            if (_logger is UnityLogger __logger)
            {
                __logger.LogDebugContext(deco_msg, context);
            }
            else
            {
                _logger.LogDebug(deco_msg);
            }
        }

#if PB_LOG_LEVEL_EDITOR_USE || !UNITY_EDITOR
        [Conditional("PB_LOG_0"), Conditional("PB_LOG_1")]
#else
        [Conditional("UNITY_EDITOR")]
#endif
#if UNITY_2022_2_OR_NEWER 
        [HideInCallstack]
#endif
        public static void Info(object message, UnityEngine.Object context, string tag = null, string loggerName = null)
        {
            ILogger _logger = GetLogger(loggerName);
            if (!CheckCanLog(_logger, 1)) return;
            object deco_msg = _logger.DecoMessage(0, tag, message);
            if (_logger is UnityLogger __logger)
            {
                __logger.LogInfoContext(deco_msg, context);
            }
            else
            {
                _logger.LogInfo(deco_msg);
            }
        }

#if PB_LOG_LEVEL_EDITOR_USE || !UNITY_EDITOR
        [Conditional("PB_LOG_0"), Conditional("PB_LOG_1"), Conditional("PB_LOG_2")]
#else
        [Conditional("UNITY_EDITOR")]
#endif
#if UNITY_2022_2_OR_NEWER 
        [HideInCallstack]
#endif
        public static void Warning(object message, UnityEngine.Object context, string tag = null, string loggerName = null)
        {
            ILogger _logger = GetLogger(loggerName);
            if (!CheckCanLog(_logger, 2)) return;
            object deco_msg = _logger.DecoMessage(0, tag, message);
            if (_logger is UnityLogger __logger)
            {
                __logger.LogWarningContext(deco_msg, context);
            }
            else
            {
                _logger.LogWarning(deco_msg);
            }
        }

#if PB_LOG_LEVEL_EDITOR_USE || !UNITY_EDITOR
        [Conditional("PB_LOG_0"), Conditional("PB_LOG_1"), Conditional("PB_LOG_2"), Conditional("PB_LOG_3")]
#else
        [Conditional("UNITY_EDITOR")]
#endif
#if UNITY_2022_2_OR_NEWER 
        [HideInCallstack]
#endif
        public static void Error(object message, UnityEngine.Object context, string tag = null, string loggerName = null)
        {
            ILogger _logger = GetLogger(loggerName);
            if (!CheckCanLog(_logger, 3)) return;
            object deco_msg = _logger.DecoMessage(0, tag, message);
            if (_logger is UnityLogger __logger)
            {
                __logger.LogErrorContext(deco_msg, context);
            }
            else
            {
                _logger.LogError(deco_msg);
            }
        }
        #endregion

    }
}