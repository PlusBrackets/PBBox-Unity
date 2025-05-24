/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.12.15
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;
using UnityEditor.Build;

#if !UNITY_2022_1_OR_NEWER || UNITY_6000_0_OR_NEWER
using System.Reflection;
using System.Text.RegularExpressions;
#endif
#endif

namespace PBBox
{
    public static partial class Log
    {
#if UNITY_EDITOR
        private static string[] s_LogLevelDefineSymbols = new string[]{
            "PB_LOG_0", "PB_LOG_1", "PB_LOG_2", "PB_LOG_3"
        };
        private static string s_LogLevelReleaseOnlyDefineSymbol = "PB_LOG_LEVEL_EDITOR_USE";

        #region Menu
        private const string STR_MENU_LOG_LEVEL_0 = "Tools/PBBox/Debug/Logging/Lv0 Log Any";
        private const string STR_MENU_LOG_LEVEL_1 = "Tools/PBBox/Debug/Logging/Lv1 No Debug";
        private const string STR_MENU_LOG_LEVEL_2 = "Tools/PBBox/Debug/Logging/Lv2 Warning And Error";
        private const string STR_MENU_LOG_LEVEL_3 = "Tools/PBBox/Debug/Logging/Lv3 Error Only";
        private const string STR_MENU_LOG_IN_EDITOR = "Tools/PBBox/Debug/Logging/Log Level In Editory";

        private static string[] GetDefineSymbols()
        {
#if UNITY_6000_0_OR_NEWER
            PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), out var defines);
#else
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out var defines);
#endif
            return defines;
        }

        private static void SetDefineSymbols(string[] defines)
        {
#if UNITY_6000_0_OR_NEWER
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), defines);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
#endif
        }

        [InitializeOnLoadMethod]
        private static void InitDefineSymbols()
        {
            var defines = GetDefineSymbols();
            bool initedLogLevelDefine = false;
            foreach (var s in s_LogLevelDefineSymbols)
            {
                if (Array.IndexOf<string>(defines, s) >= 0)
                {
                    initedLogLevelDefine = true;
                    break;
                }
            }
            if (!initedLogLevelDefine)
            {
                Array.Resize(ref defines, defines.Length + 1);
                defines[defines.Length - 1] = s_LogLevelDefineSymbols[0];
                SetDefineSymbols(defines);
            }
        }

        /// <summary>
        /// 切换log在发布后是否依旧有效
        /// </summary>
        private static void SwitchLogInReleaseDefine()
        {
            var defines = GetDefineSymbols();
            List<string> temps = new List<string>(defines);
            int idx = temps.IndexOf(s_LogLevelReleaseOnlyDefineSymbol);
            if (idx >= 0)
            {
                temps.RemoveAt(idx);
            }
            else
            {
                temps.Add(s_LogLevelReleaseOnlyDefineSymbol);
            }
            SetDefineSymbols(temps.ToArray());
        }

        /// <summary>
        /// 设置log等级的自定义宏变量
        /// </summary>
        /// <param name="level"></param>
        private static void SetLogLevelDefine(int level)
        {
            var defines = GetDefineSymbols();
            List<string> temps = new List<string>(defines);
            bool isSet = false;
            for (int i = temps.Count - 1; i >= 0; i--)
            {
                if (Array.IndexOf<string>(s_LogLevelDefineSymbols, temps[i]) >= 0)
                {
                    if (!isSet)
                    {
                        temps[i] = s_LogLevelDefineSymbols[level];
                        isSet = true;
                    }
                    else
                    {
                        temps.RemoveAt(i);
                    }
                }
            }
            if (!isSet)
            {
                temps.Add(s_LogLevelDefineSymbols[level]);
            }
            SetDefineSymbols(temps.ToArray());
        }

        [MenuItem(STR_MENU_LOG_LEVEL_0, true)]
        private static bool InitLoggingMenuStates()
        {
            var symbols = GetDefineSymbols();
            Menu.SetChecked(STR_MENU_LOG_IN_EDITOR, Array.IndexOf<string>(symbols, s_LogLevelReleaseOnlyDefineSymbol) >= 0);
            Menu.SetChecked(STR_MENU_LOG_LEVEL_0, Array.IndexOf<string>(symbols, s_LogLevelDefineSymbols[0]) >= 0);
            Menu.SetChecked(STR_MENU_LOG_LEVEL_1, Array.IndexOf<string>(symbols, s_LogLevelDefineSymbols[1]) >= 0);
            Menu.SetChecked(STR_MENU_LOG_LEVEL_2, Array.IndexOf<string>(symbols, s_LogLevelDefineSymbols[2]) >= 0);
            Menu.SetChecked(STR_MENU_LOG_LEVEL_3, Array.IndexOf<string>(symbols, s_LogLevelDefineSymbols[3]) >= 0);
            return true;
        }

        [MenuItem(STR_MENU_LOG_LEVEL_0)]
        private static void SwitchLogLevel_0()
        {
            SetLogLevelDefine(0);
        }

        [MenuItem(STR_MENU_LOG_LEVEL_1)]
        private static void SwitchLogLevel_1()
        {
            SetLogLevelDefine(1);
        }

        [MenuItem(STR_MENU_LOG_LEVEL_2)]
        private static void SwitchLogLevel_2()
        {
            SetLogLevelDefine(2);
        }

        [MenuItem(STR_MENU_LOG_LEVEL_3)]
        private static void SwitchLogLevel_3()
        {
            SetLogLevelDefine(3);
        }

        [MenuItem(STR_MENU_LOG_IN_EDITOR, false, 10001)]
        private static void SwitchLogLevelDisable()
        {
            SwitchLogInReleaseDefine();
        }
        #endregion

        #region Console Window Click Trace Plugin
#if !UNITY_2022_2_OR_NEWER || UNITY_6000_0_OR_NEWER//2022版可以直接使用[HideInCallstack]特性隐藏调用方法

        /// <summary>
        /// 利用OnOpenAsset回调，忽略封装的回调路径，打开真实路径
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        [UnityEditor.Callbacks.OnOpenAsset(-1)]
        private static bool OnOpenAsset(int instance, int line)
        {
            string checkPath = AssetDatabase.GetAssetPath(EditorUtility.InstanceIDToObject(instance));
            if (!checkPath.Contains("/Log."))
            {
                return false;
            }

            string strStackTrace = GetStackTrace();
            if (string.IsNullOrEmpty(strStackTrace) || !strStackTrace.Contains(typeof(Log).FullName + ":"))
            {
                return false;
            }
            var _removeIndex = strStackTrace.LastIndexOf(typeof(Log).FullName + ":");
            strStackTrace = strStackTrace.Remove(0, _removeIndex);
            strStackTrace = strStackTrace.Remove(0, strStackTrace.IndexOf("\n") + 1);

            Match matches = Regex.Match(strStackTrace, @"\(at (.+):(\d+)\)", RegexOptions.IgnoreCase);
            while (matches.Success)
            {
                var path = Path.Combine(Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets")), matches.Groups[1].Value);
                if (!int.TryParse(matches.Groups[2].Value, out int tempLine))
                {
                    tempLine = 0;
                }
                line = tempLine;
                if (UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(Path.GetFullPath(path), line))
                {
                    break;
                }
                matches = matches.NextMatch();
            }
            return true;
        }

        /// <summary>
        /// 使用反射获得正在双击的trace数据，
        /// 详情见源码 https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/ConsoleWindow.cs
        /// 参考 http://www.cppblog.com/heath/archive/2016/06/21/213777.html
        /// </summary>
        /// <returns></returns>
        private static string GetStackTrace()
        {
            var typeConsoleWindow = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            var fieldInfo = typeConsoleWindow.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            var consoleWindowInstance = fieldInfo.GetValue(null);
            if (consoleWindowInstance != null)
            {
                if ((object)EditorWindow.focusedWindow == consoleWindowInstance)
                {
                    fieldInfo = typeConsoleWindow.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                    return fieldInfo.GetValue(consoleWindowInstance).ToString();
                }
            }
            return null;
        }
#endif
        #endregion

#endif
    }
}