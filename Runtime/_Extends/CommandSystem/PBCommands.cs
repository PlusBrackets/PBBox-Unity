/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.10.14
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PBBox
{
    public class PBCommands : SingleClass<PBCommands>
    {
        private struct CmdMethodInfo
        {
            public MethodInfo methodInfo { get; set; }
            public PBCommandAttribute cmdAttribute { get; set; }
        }

        private readonly Dictionary<string, CmdMethodInfo> _methodMap = new Dictionary<string, CmdMethodInfo>();
        private readonly BindingFlags _methodBindingFlag = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        protected override void Init()
        {
            base.Init();
            LoadAllCmd();
        }

        public void LoadAllCmd()
        {
#if UNITY_EDITOR || GAME_TEST
            System.Text.StringBuilder logs = new System.Text.StringBuilder();
            logs.AppendLine("");
            var testCost = new System.Diagnostics.Stopwatch();
            testCost.Start();
#endif
            int methodCount = 0;
            _methodMap.Clear();
            var methodInfos = ReflectionUtils.GetAllMethodWithAtturbute<PBCommandClassAttribute, PBCommandAttribute>(_methodBindingFlag, assemblyNames: PBBoxConfigs.CMD_SYS_REFLECT_ASSEMBLIES);

            foreach (var m in methodInfos)
            {
                var attrs = m.GetCustomAttributes<PBCommandAttribute>();
                foreach (var a in attrs)
                {
#if UNITY_EDITOR || GAME_TEST
                    logs.AppendLine($"载入cmd: {a.cmdName} <=> method: {m.IsSpecialName}");
#endif
                    methodCount++;
                    _methodMap[a.cmdName.Trim().ToLower()] = new CmdMethodInfo() { methodInfo = m, cmdAttribute = a };
                }
            }
#if UNITY_EDITOR || GAME_TEST
            testCost.Stop();
            DebugUtils.Internal.Info<PBCommands>($"PBCommand完成加载,已加载:{methodCount},耗时{testCost.Elapsed.TotalMilliseconds}ms"
                + logs.ToString());
#else
                            
             DebugUtils.Internal.Info<PBCommands>($"PBCommand完成加载,已加载:{methodCount}");
#endif
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="cmdStr">格式为 cmdName: param1, param2,.... 例如:echo:ABC,2</param>
        /// <param name="extraParameter"></param>
        public static void Excute<T>(string cmdStr, T extraParameter = null) where T : class
        {
            int index = cmdStr.IndexOf(':');
            if (index > 0)
            {
                string cmdKey = cmdStr.Substring(0, index);
                string paramStr = cmdStr.Remove(0, index + 1);
                Instance.Excute(cmdKey, paramStr, extraParameter);
            }
            else
            {
                Instance.Excute(cmdStr, null, extraParameter);
            }
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="cmdStr"></param>
        /// <param name="extraParameter"></param>
        /// <typeparam name="ResultT">返回类型</typeparam>
        /// <returns></returns>
        public static ResultT Excute<ResultT, T>(string cmdStr, T extraParameter = null) where T : class
        {
            int index = cmdStr.IndexOf(':');
            if (index > 0)
            {
                string cmdKey = cmdStr.Substring(0, index);
                string paramStr = cmdStr.Remove(0, index + 1);
                if (Instance.Excute(cmdKey, paramStr, extraParameter) is ResultT _result)
                {
                    return _result;
                }
            }
            else
            {
                if (Instance.Excute(cmdStr, null, extraParameter) is ResultT _result)
                {
                    return _result;
                }
            }
            DebugUtils.InfoError<PBCommands>($"Cmd cannot return the correct type: [{typeof(ResultT)}]. cmdStr:{cmdStr}");
            return default(ResultT);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="cmdKey">指令名</param>
        /// <param name="paramStr">参数字符串，使用英文逗号分隔</param>
        /// <param name="extraParameter"></param>
        private object Excute<T>(string cmdKey, string paramStr, T extraParameter = null) where T : class
        {
            if (_methodMap.TryGetValue(cmdKey.Trim().ToLower(), out var value))
            {
                List<object> parameters = new List<object>();
                if (value.cmdAttribute.keepFullParamStr)
                {
                    parameters.Add(paramStr);
                }
                else if (!string.IsNullOrEmpty(paramStr))
                {
                    string[] pStrs = ParseParamStr(paramStr);

                    var pInfos = value.methodInfo.GetParameters();
                    int pLength = value.cmdAttribute.passExtraParam ? pInfos.Length - 1 : pInfos.Length;
                    for (int i = 0; i < pLength; i++)
                    {
                        object parameter = null;
                        var pInfo = pInfos[i];
                        pStrs.TryGet(i, out var pStr);
                        if (pInfo.ParameterType == typeof(int))
                        {
                            parameter = SetParameter(int.TryParse(pStr?.Trim(), out int result), result, pInfo);
                        }
                        else if (pInfo.ParameterType == typeof(float))
                        {
                            parameter = SetParameter(float.TryParse(pStr?.Trim(), out float result), result, pInfo);
                        }
                        else if (pInfo.ParameterType == typeof(bool))
                        {
                            parameter = SetParameter(bool.TryParse(pStr?.Trim(), out bool result), result, pInfo);
                        }
                        else
                        {
                            parameter = pStr?.Trim();
                        }
                        parameters.Add(parameter);
                    }
                }
                if (value.cmdAttribute.passExtraParam)
                {
                    parameters.Add(extraParameter);
                }

#if GAME_TEST
                object methodResult = value.methodInfo.Invoke(null, parameters.ToArray());
                string paramStr2 = null;
                foreach (var p in parameters)
                {
                    paramStr2 += p.ToString() + ",";
                }
                paramStr2 = paramStr2?.TrimEnd(',');
                DebugUtils.Test.Info<PBCommands>(
                    $"Cmd [ {cmdKey}{(string.IsNullOrEmpty(paramStr) ? "" : $":{paramStr}")} ]. "
                    + $"Excuted function \"{value.methodInfo.Name}\" in \"{value.methodInfo.DeclaringType}\""
                    + $"{(string.IsNullOrEmpty(paramStr2) ? "" : $", with args: {paramStr2}")}"
                    + $"{(methodResult != null ? ", return: " + methodResult.ToString() : "")}"
                    + ".");
                return methodResult;
            }
            DebugUtils.Test.Info<PBCommands>($"Can not found cmd: \"{cmdKey}\"");
#else
                return value.methodInfo.Invoke(null, parameters.ToArray());
            }
#endif
            return null;

            static TParam SetParameter<TParam>(bool flag, TParam result, ParameterInfo pInfo)
            {
                if (flag)
                {
                    return result;
                }
                else
                {
                    return pInfo.HasDefaultValue ? (TParam)pInfo.DefaultValue : default(TParam);
                }
            }

            static string[] ParseParamStr(string paramStr)
            {
                //若包含数组
                if (paramStr.Contains('['))
                {
                    //TODO 后续拓展支持列表
                }
                //若包含特殊字符串
                if (paramStr.Contains('\"'))
                {
                    //TODO 等待后续拓展
                }
                //TODO 先忽略字符串中的','号，直接简单分割所有，后续添加额外的字符串处理，例如若有英文逗号或者前后需要有空格时，前后添加英文分号来标识字符串：'str,str '
                return paramStr.Split(',');
            }
        }
    }
}