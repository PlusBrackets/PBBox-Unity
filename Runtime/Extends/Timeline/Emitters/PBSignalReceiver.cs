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
using UnityEngine.Events;

namespace PBBox.Timeline
{
    [AddComponentMenu("PBBox/Timeline/PB Signal Receiver")]
    public class PBSignalReceiver : MonoBehaviour, INotificationReceiver, IAttachDatas
    {
        public UnityAction<string, TimelineCmdExtraParams> OnLocalMessage { get; private set; }

        public Dictionary<string, object> AttachDatas { get; } = new Dictionary<string, object>();

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            #if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying)
                return;
            #endif
            if (notification is PBCmdEmitter cmdEmitter)
            {
                if (cmdEmitter.cmds != null)
                {
                    foreach (var cmd in cmdEmitter.cmds)
                    {
                        cmd.extraParams.SetPlayableData(origin, this, cmdEmitter);
                        string cmdStr = PBCmdEmitter.CMD_PREFIX + cmd.command;
                        switch (cmd.cmdType)
                        {
                            case PBCmdEmitter.CmdType.Normal:
                                PBCommands.Excute(cmdStr, cmd.extraParams);
                                break;
                            case PBCmdEmitter.CmdType.Condition:
                                if (!PBCommands.Excute<bool, TimelineCmdExtraParams>(cmdStr, cmd.extraParams))
                                {
                                    return;
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}