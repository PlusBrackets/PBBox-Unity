/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.12.15
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR && !UNITY_2022_1_OR_NEWER
using UnityEditor;
using System.Reflection;
using System.Text.RegularExpressions;
#endif

namespace PBBox
{
    public static partial class Log
    {
#if UNITY_EDITOR
        private static string[] s_LogLevelDefineSymbols = new string[]{
            "PB_LOG_0", "PB_LOG_1", "PB_LOG_2", "PB_LOG_3"
        };
        private static string s_LogInReleaseDefineSymbol = "PB_LOG_IN_RELEASE";

        #region Menu
        private const string STR_MENU_LOG_LEVEL_0 = "Tools/PBBox/Debug/Logging/Lv0 Log Any";
        private const string STR_MENU_LOG_LEVEL_1 = "Tools/PBBox/Debug/Logging/Lv1 No Debug";
        private const string STR_MENU_LOG_LEVEL_2 = "Tools/PBBox/Debug/Logging/Lv2 Warning And Error";
        private const string STR_MENU_LOG_LEVEL_3 = "Tools/PBBox/Debug/Logging/Lv3 Error Only";
        private const string STR_MENU_LOG_IN_RELEASE = "Tools/PBBox/Debug/Logging/Log In Release";

        [InitializeOnLoadMethod]
        private static void InitDefineSymbols()
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out var defines);
            bool _initedLogLevelDefine = false;
            foreach (var s in s_LogLevelDefineSymbols)
            {
                if (Array.IndexOf<string>(defines, s) >= 0)
                {
                    _initedLogLevelDefine = true;
                    break;
                }
            }
            if (!_initedLogLevelDefine)
            {
                Array.Resize(ref defines, defines.Length + 1);
                defines[defines.Length - 1] = s_LogLevelDefineSymbols[0];
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
            }
        }

        /// <summary>
        /// 切换log在发布后是否依旧有效
        /// </summary>
        private static void SwitchLogInReleaseDefine()
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out var defines);
            List<string> _temps = new List<string>(defines);
            int _idx = _temps.IndexOf(s_LogInReleaseDefineSymbol);
            if (_idx >= 0)
            {
                _temps.RemoveAt(_idx);
            }
            else
            {
                _temps.Add(s_LogInReleaseDefineSymbol);
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, _temps.ToArray());
        }

        /// <summary>
        /// 设置log等级的自定义宏变量
        /// </summary>
        /// <param name="level"></param>
        private static void SetLogLevelDefine(int level)
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out var defines);
            List<string> _temps = new List<string>(defines);
            bool _isSet = false;
            for (int i = _temps.Count - 1; i >= 0; i--)
            {
                if (Array.IndexOf<string>(s_LogLevelDefineSymbols, _temps[i]) >= 0)
                {
                    if (!_isSet)
                    {
                        _temps[i] = s_LogLevelDefineSymbols[level];
                        _isSet = true;
                    }
                    else
                    {
                        _temps.RemoveAt(i);
                    }
                }
            }
            if (!_isSet)
            {
                _temps.Add(s_LogLevelDefineSymbols[level]);
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, _temps.ToArray());
        }

        [MenuItem(STR_MENU_LOG_LEVEL_0, true)]
        private static bool InitLoggingMenuStates()
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out var symbols);
            Menu.SetChecked(STR_MENU_LOG_IN_RELEASE, Array.IndexOf<string>(symbols, s_LogInReleaseDefineSymbol) >= 0);
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

        [MenuItem(STR_MENU_LOG_IN_RELEASE, false, 10001)]
        private static void SwitchLogLevelDisable()
        {
            SwitchLogInReleaseDefine();
        }
        #endregion

        #region Console Window Click Trace Plugin
#if !UNITY_2022_2_OR_NEWER //2022版可以直接使用[HideInCallstack]特性隐藏调用方法

        /// <summary>
        /// 利用OnOpenAsset回调，忽略封装的回调路径，打开真实路径
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        [UnityEditor.Callbacks.OnOpenAsset(-1)]
        private static bool OnOpenAsset(int instance, int line)
        {
            var _target = EditorUtility.InstanceIDToObject(instance) as MonoScript;
            if (_target == null || !_target.name.StartsWith(typeof(Log).Name, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string _strStackTrace = GetStackTrace();
            if (string.IsNullOrEmpty(_strStackTrace) || !_strStackTrace.Contains(typeof(Log).FullName + ":"))
            {
                return false;
            }
            var _removeIndex = _strStackTrace.LastIndexOf(typeof(Log).FullName + ":");
            _strStackTrace = _strStackTrace.Remove(0, _removeIndex);
            _strStackTrace = _strStackTrace.Remove(0, _strStackTrace.IndexOf("\n") + 1);

            Match _matches = Regex.Match(_strStackTrace, @"\(at (.+):(\d+)\)", RegexOptions.IgnoreCase);
            while (_matches.Success)
            {
                var path = Path.Combine(Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets")), _matches.Groups[1].Value);
                if (!int.TryParse(_matches.Groups[2].Value, out int _tempLine))
                {
                    _tempLine = 0;
                }
                line = _tempLine;
                if (UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(Path.GetFullPath(path), line))
                {
                    break;
                }
                _matches = _matches.NextMatch();
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
            var _typeConsoleWindow = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            var _fieldInfo = _typeConsoleWindow.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            var _consoleWindowInstance = _fieldInfo.GetValue(null);
            if (_consoleWindowInstance != null)
            {
                if ((object)EditorWindow.focusedWindow == _consoleWindowInstance)
                {
                    _fieldInfo = _typeConsoleWindow.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                    return _fieldInfo.GetValue(_consoleWindowInstance).ToString();
                }
            }
            return null;
        }
#endif
        #endregion

#endif
    }
}