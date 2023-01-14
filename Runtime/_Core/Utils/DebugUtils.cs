/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
// #define LOG_DEFAULT
// #define LOG_INTERNAL

using System;
using UnityEngine;

namespace PBBox
{
    /// <summary>
    /// log工具，可控制开关
    /// </summary>
    [System.Obsolete]
    public static class DebugUtils
    {

        public static Action<object> Log = Default.Log;
        public static Action<object> LogWarning = Default.LogWarning;
        public static Action<object> LogError = Default.LogError;

        public static string AddClassInfo<T>(this string str)
        {
#if LOG_DEFAULT || LOG_INTERNAL || UNITY_EDITOR || GAME_TEST
            str = "[" + typeof(T).FullName + "]" + str;
#endif
            return str;
        }

        public static void Info<T>(object message) => Default.Info<T>(message);
        public static void InfoWarning<T>(object message) => Default.InfoWarning<T>(message);
        public static void InfoError<T>(object message) => Default.InfoError<T>(message);

        public static class Default
        {
#if LOG_DEFAULT || UNITY_EDITOR || GAME_TEST
            public static Action<object> Log = Debug.Log;
            public static Action<object> LogWarning = Debug.LogWarning;
            public static Action<object> LogError = Debug.LogError;
            public static void Info<T>(object message) => DebugUtils.Log($"[{typeof(T).FullName}] {message}");
            public static void InfoWarning<T>(object message) => DebugUtils.LogWarning($"[{typeof(T).FullName}] {message}");
            public static void InfoError<T>(object message) => DebugUtils.LogError($"[{typeof(T).FullName}] {message}");

#else
            public static void Log(object message) { }
            public static void LogWarning(object message) { }
            public static void LogError(object message) { }
            public static void Info<T>(object message) { }
            public static void InfoWarning<T>(object message) { }
            public static void InfoError<T>(object message) { } 
#endif
        }

        public static class Internal
        {
#if LOG_INTERNAL || UNITY_EDITOR
            public static Action<object> Log = Debug.Log;
            public static Action<object> LogWarning = Debug.LogWarning;
            public static Action<object> LogError = Debug.LogError;
            public static void Info<T>(object message) => DebugUtils.Log($"[{typeof(T).FullName}] {message}");
            public static void InfoWarning<T>(object message) => DebugUtils.LogWarning($"[{typeof(T).FullName}] {message}");
            public static void InfoError<T>(object message) => DebugUtils.LogError($"[{typeof(T).FullName}] {message}");
#else
            public static void Log(object message) { }
            public static void LogWarning(object message) { }
            public static void LogError(object message) { }
            public static void Info<T>(object message) { }
            public static void InfoWarning<T>(object message) { }
            public static void InfoError<T>(object message) { } 
#endif
        }

        public static class Test
        {
#if GAME_TEST
            public static Action<object> Log = Debug.Log;
            public static Action<object> LogWarning = Debug.LogWarning;
            public static Action<object> LogError = Debug.LogError;
            public static void Info<T>(object message) => DebugUtils.Log($"[{typeof(T).FullName}] {message}");
            public static void InfoWarning<T>(object message) => DebugUtils.LogWarning($"[{typeof(T).FullName}] {message}");
            public static void InfoError<T>(object message) => DebugUtils.LogError($"[{typeof(T).FullName}] {message}");
#else
            public static void Log(object message) { }
            public static void LogWarning(object message) { }
            public static void LogError(object message) { }
            public static void Info<T>(object message) { }
            public static void InfoWarning<T>(object message) { }
            public static void InfoError<T>(object message) { }
#endif
        }
    }

//     public static partial class PBExtensions
//     {
// #if LOG_DEFAULT || UNITY_EDITOR || GAME_TEST
//         public static void LogInfo(this object target, object message)
//         {
//             DebugUtils.Log($"[{target.GetType().FullName}] {message}");
//         }

//         public static void LogInfoWarning(this object target, object message)
//         {
//             DebugUtils.LogWarning($"[{target.GetType().FullName}] {message}");
//         }

//         public static void LogInfoError(this object target, object message)
//         {
//             DebugUtils.LogError($"[{target.GetType().FullName}] {message}");
//         }
// #else
//         public static void LogInfo(this object target, object message){}
//         public static void LogInfoWarning(this object target, object message){}
//         public static void LogInfoError(this object target, object message){}
// #endif

    // }
}
