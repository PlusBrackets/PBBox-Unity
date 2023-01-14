/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.12.15
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using UnityEngine;
using System.Text;

namespace PBBox
{
    public static partial class Log
    {
        private class UnityLogger : ILogger
        {
            private static Lazy<StringBuilder> s_MessageBuilder = new Lazy<StringBuilder>(System.Threading.LazyThreadSafetyMode.None);
            
            public string Name { get; private set; }
            public bool Enable { get; set; } = true;
            public int LogLevel { get; set; } = 0;

#if UNITY_EDITOR
            public Color TagColor
            {
                get => m_TagColor;
                set
                {
                    m_TagColor = value;
                    m_TagColorString = ColorUtility.ToHtmlStringRGB(value);
                }
            }
            public Color NameColor
            {
                get => m_NameColor;
                set
                {
                    m_NameColor = value;
                    m_NameColorString = ColorUtility.ToHtmlStringRGB(value);
                }
            }
            private Color m_TagColor = new Color(0, 0.5f, 0.5f, 1);
            private Color m_NameColor = Color.gray;
            private string m_TagColorString;
            private string m_NameColorString;
#endif
            public Action<object> LogDebug => UnityEngine.Debug.Log;
            public Action<object> LogInfo => UnityEngine.Debug.Log;
            public Action<object> LogWarning => UnityEngine.Debug.LogWarning;
            public Action<object> LogError => UnityEngine.Debug.LogError;

            public Action<object, UnityEngine.Object> LogDebugContext => UnityEngine.Debug.Log;
            public Action<object, UnityEngine.Object> LogInfoContext => UnityEngine.Debug.Log;
            public Action<object, UnityEngine.Object> LogWarningContext => UnityEngine.Debug.LogWarning;
            public Action<object, UnityEngine.Object> LogErrorContext => UnityEngine.Debug.LogError;

            protected internal UnityLogger(string name)
            {
                Name = name;
#if UNITY_EDITOR
                TagColor = m_TagColor;
                NameColor = m_NameColor;
#endif
            }

            public object DecoMessage(int level, string tag, object message)
            {
                lock(s_MessageBuilder)
                {
                    var _msgBuilder = s_MessageBuilder.Value;
                    if (!string.IsNullOrEmpty(Name))
                    {
                        _msgBuilder
#if UNITY_EDITOR
                            .Append("<color=#").Append(m_NameColorString).Append(">")
#endif
                            .Append('[').Append(Name).Append(']')
#if UNITY_EDITOR
                            .Append("</color>");
#endif
                    }
                    if (!string.IsNullOrEmpty(tag))
                    {
                        _msgBuilder
#if UNITY_EDITOR
                            .Append("<color=#").Append(m_TagColorString).Append(">")
#endif
                            .Append('[').Append(tag).Append(']')
#if UNITY_EDITOR
                            .Append("</color>");
#endif
                    }
                    switch (level)
                    {
                        case 0:
                            _msgBuilder
#if UNITY_EDITOR
                                .Append("<color=#808080>[DEBUG] </color>")
#else
                                .Append("[DEBUG] ")
#endif
                                .Append(message);
// #if UNITY_EDITOR
//                                 .Append("<color=#808080>")
//                                 .Append(message)
//                                 .Append("</color>");
// #else
//                                 .Append(message);
// #endif
                            break;
                        case 1:
                            _msgBuilder
                                .Append("[INFO] ")
                                .Append(message);
                            // if (_msgBuilder.Length != 0)
                            // {
                            //     _msgBuilder.Append(message);
                            // }
                            break;
                        case 2:
                            _msgBuilder
#if UNITY_EDITOR
                                .Append("<color=#ffaa00>[WARNING] </color>")
#else
                                .Append("[WARNING] ")
#endif
                                .Append(message);
                            break;
                        case 3:
                            _msgBuilder
#if UNITY_EDITOR
                                .Append("<color=#ff0000>[ERROR] </color>")
#else
                                .Append("[ERROR] ")
#endif
                                .Append(message);
                            break;
                    }
                    
                    if (s_MessageBuilder.Value.Length >= 0)
                    {
                        message = s_MessageBuilder.Value.ToString();
                    }
                    s_MessageBuilder.Value.Clear();
                }
                return message;
            }
        }
        
    }
}