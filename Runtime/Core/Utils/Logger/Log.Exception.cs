/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.12.15
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    public static partial class Log
    {
        /// <summary>
        /// PBBox格式的Exception
        /// </summary>
        public class FetalErrorException : Exception
        {
            public string Tag { get; private set; }
            public string Logger { get; private set; }

            public FetalErrorException(string message, string tag = null, string loggerName = null)
            : base(DecoMessage(message, tag, loggerName))
            {
                Tag = tag;
                Logger = loggerName;
            }

            public FetalErrorException(string message, Exception innerException, string tag = null, string loggerName = null)
            : base(DecoMessage(message, tag, loggerName), innerException)
            {
                Tag = tag;
                Logger = loggerName;
            }

            private static string DecoMessage(string msg, string tag, string logger)
            {
                string temp = msg;
#if UNITY_EDITOR
                temp = "<color=#FF00EA>[FetalError]</color> " + temp;
#else
                    temp = "[FetalError] " + temp;
#endif
                if (!string.IsNullOrEmpty(tag))
                {
#if UNITY_EDITOR
                    temp = "<color=#008080>[" + tag + "]</color> " + temp;
#else
                    temp = "[" + tag + "] " + temp;
#endif
                }
                if (!string.IsNullOrEmpty(logger))
                {
#if UNITY_EDITOR
                    temp = "<color=#808080>[" + logger + "]</color> " + temp;
#else
                    temp = "[" + logger + "] " + temp;
#endif
                }
                return temp;
            }

        }
    }
}