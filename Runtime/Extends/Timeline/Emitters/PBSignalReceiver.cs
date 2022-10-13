using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace PBBox.Timeline
{
    [AddComponentMenu("PBBox/Timeline/PB Signal Receiver")]
    public class PBSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is PBCmdEmitter cmdEmitter)
            {
                if (cmdEmitter.cmds != null)
                {
                    foreach (var cmd in cmdEmitter.cmds)
                    {
                        cmd.extraParams.SetPlayableData(origin, gameObject, cmdEmitter);
                        PBCommands.Excute(cmd.command, cmd.extraParams);
                    }
                }
                // Debug.Log(cmdEmitter.command);
            }
        }
    }
}