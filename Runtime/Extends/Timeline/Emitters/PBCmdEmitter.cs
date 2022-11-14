/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.10.22
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace PBBox.Timeline
{
    [System.Serializable]
    public class TimelineCmdExtraParams
    {
        /// <summary>
        /// 播放的playable
        /// </summary>
        public Playable origin;
        /// <summary>
        /// 接收信号的object
        /// </summary>
        [System.NonSerialized]
        public MonoBehaviour receiver;
        /// <summary>
        /// 发送信号的marker
        /// </summary>
        [System.NonSerialized]
        public Marker emitter;
        public SDictionary<string, ExposedReference<Object>> referencesMap;
        public SDictionary<string, string> valuesMap;

        internal void SetPlayableData(Playable origin, MonoBehaviour receiver, Marker emitter)
        {
            this.origin = origin;
            this.receiver = receiver;
            this.emitter = emitter;
        }

        public Object GetReference(string key)
        {
            if (referencesMap == null) return null;
            if (referencesMap.TryGetValue(key, out var expRef))
            {
                return expRef.Resolve(origin.GetGraph().GetResolver());
            }
            return null;
        }

        public string GetString(string key)
        {
            if (valuesMap == null) return null;
            if (valuesMap.TryGetValue(key, out string value))
            {
                return value;
            }
            return null;
        }

        public bool GetInt(string key, ref int value)
        {
            if (valuesMap == null) return false;
            if (valuesMap.TryGetValue(key, out string str))
            {
                return int.TryParse(str, out value);
            }
            return false;
        }

        public bool GetFloat(string key, ref float value)
        {
            if (valuesMap == null) return false;
            if (valuesMap.TryGetValue(key, out string str))
            {
                return float.TryParse(str, out value);
            }
            return false;
        }

        public bool GetBool(string key, ref bool value)
        {
            if (valuesMap == null) return false;
            if (valuesMap.TryGetValue(key, out string str))
            {
                return bool.TryParse(str, out value);
            }
            return false;
        }

        public T GetReference<T>(string key) where T : Object => GetReference(key) as T;
    }

    [SerializeField]
    public class PBCmdEmitter : SignalEmitter
    {
        public const string CMD_PREFIX = "CmdTimeline_";

        public enum CmdType
        {
            Normal = 0,
            [Tooltip("需要cmd返回bool")]
            Condition,
            // [Tooltip("需要cmd是协程形式")]
            // WaitForEnd
        }

        [System.Serializable]
        public class CmdData
        {
            [Tooltip("输入的指令，需要在代码中使用PBCommandSystem定义好对应的静态方法。" +
            "\n指令格式为：\n\ncmd:param1,param2,param3" +
            "\n\nparam支持 int, float, string, bool 类型")]
            public string command;
            public CmdType cmdType;

            [Tooltip("作为指令的额外参数传入静态方法")]
            public TimelineCmdExtraParams extraParams = new TimelineCmdExtraParams();
        }

        public List<CmdData> cmds = new List<CmdData>();

    }
}