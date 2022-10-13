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
        public GameObject receiver;
        /// <summary>
        /// 发送信号的marker
        /// </summary>
        [System.NonSerialized]
        public Marker emitter;
        public SDictionary<string, ExposedReference<Object>> referencesMap;

        internal void SetPlayableData(Playable origin, GameObject receiver, Marker emitter)
        {
            this.origin = origin;
            this.receiver = receiver;
            this.emitter = emitter;
        }

        public Object GetReference(string key)
        {
            if (referencesMap.TryGetValue(key, out var expRef))
            {
                return expRef.Resolve(origin.GetGraph().GetResolver());
            }
            return null;
        }

        public T GetReference<T>(string key) where T : Object => GetReference(key) as T;
    }

    [SerializeField]
    public class PBCmdEmitter : SignalEmitter
    {
        [System.Serializable]
        public class CmdData
        {
            [Tooltip("输入的指令，需要在代码中使用PBCommandSystem定义好对应的静态方法。" +
            "\n指令格式为：\n\ncmd:param1,param2,param3" +
            "\n\nparam支持 int, float, string, bool 类型")]
            public string command;

            [Tooltip("作为指令的额外参数传入静态方法")]
            public TimelineCmdExtraParams extraParams = new TimelineCmdExtraParams();
        }

        public List<CmdData> cmds = new List<CmdData>();

    }
}