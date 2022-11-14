/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.10.22
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace PBBox.Timeline
{
    [PBCommandClass]
    internal static class PBCmdEmitterCmds
    {
        
        [PBCommand(PBCmdEmitter.CMD_PREFIX + "SendMsg", false, true)]
        [PBCommand(PBCmdEmitter.CMD_PREFIX + "SendLocalMsg", false, true)]
        private static void CmdSendLockMsg(string msg,TimelineCmdExtraParams extraParams){
            if(extraParams == null)
                return;
            if(extraParams.receiver is PBSignalReceiver _receiver){
                _receiver.OnLocalMessage?.Invoke(msg, extraParams);
            }
        }

        /// <summary>
        /// 暂停该timeline
        /// </summary>
        /// <param name="extraParams"></param>
        [PBCommand(PBCmdEmitter.CMD_PREFIX + "Pause", false, true)]
        private static void CmdPause(TimelineCmdExtraParams extraParams)
        {
            if (extraParams == null)
                return;
            // extraParams.origin.GetGraph().GetRootPlayable(0).SetSpeed(0);
            extraParams.origin.GetGraph().Stop();
        }

        /// <summary>
        /// 暂停timeline，但当flag为cond时恢复该timeline的播放
        /// </summary>
        /// <param name="flagName"></param>
        /// <param name="checkValue"></param>
        /// <param name="extraParams"></param>
        [PBCommand(PBCmdEmitter.CMD_PREFIX + "PauseUtilFlag", false, true)]
        private static void CmdPauseUtilFlag(string flagName, bool cond = true, TimelineCmdExtraParams extraParams = null)
        {
            if(extraParams == null)
                return;
            var graph = extraParams.origin.GetGraph();
            
            // graph.Stop();
            var speed = graph.GetRootPlayable(0).GetSpeed();
            graph.GetRootPlayable(0).SetSpeed(0);
            Dictionary<string, object> attachDatas = (extraParams.receiver as IAttachDatas)?.AttachDatas;
            if (attachDatas == null)
                return;
            extraParams.receiver.StartCoroutine(WaitForFlag());

            IEnumerator WaitForFlag()
            {
                object flag;
                while (graph.IsValid() && !graph.IsDone() && graph.IsPlaying())
                {
                    if (attachDatas.TryGetValue(flagName, out flag))
                    {
                        if (flag != null && Convert.ToBoolean(flag) == cond)
                        {
                            break;
                        }
                    }
                    yield return null;
                }
                graph.GetRootPlayable(0).SetSpeed(speed);
            }
        }


        [PBCommand(PBCmdEmitter.CMD_PREFIX + "PauseUtilSec", false, true)]
        private static void CmdPauseUtilSec(float duration, TimelineCmdExtraParams extraParams = null)
        {
            if (extraParams == null)
                return;
            var graph = extraParams.origin.GetGraph();
            // graph.Stop();
            var speed = graph.GetRootPlayable(0).GetSpeed();
            graph.GetRootPlayable(0).SetSpeed(0);
            extraParams.receiver.StartCoroutine(WaitForSec());

            IEnumerator WaitForSec()
            {
                yield return new WaitForSeconds(duration);
                if (graph.IsValid())
                {
                    graph.GetRootPlayable(0).SetSpeed(speed);
                }
            }
        }

        /// <summary>
        /// 设置一个bool类型的flag
        /// </summary>
        /// <param name="flagName"></param>
        /// <param name="value"></param>
        /// <param name="extraParams"></param>
        [PBCommand(PBCmdEmitter.CMD_PREFIX + "SetFlag", false, true)]
        private static void CmdSetFlag(string flagName, bool value, TimelineCmdExtraParams extraParams)
        {
            if (extraParams == null)
                return;
            var attachDatas = extraParams.receiver as IAttachDatas;
            if (attachDatas == null)
            {
                DebugUtils.Log("[Timeline CMD] receiver 不存在IAttachDatas接口");
                return;
            }
            attachDatas.AttachDatas[flagName] = value;
            
        }

        /// <summary>
        /// 检测flag
        /// </summary>
        /// <param name="flagName"></param>
        /// <param name="cond"></param>
        /// <param name="extraParams"></param>
        /// <returns></returns>
        [PBCommand(PBCmdEmitter.CMD_PREFIX + "IsFlag", false, true)]
        [PBCommand(PBCmdEmitter.CMD_PREFIX + "CheckFlag", false, true)]
        private static bool CmdCheckFlag(string flagName, bool cond = true, TimelineCmdExtraParams extraParams = null)
        {
            if (extraParams == null)
                return false;
            var attachDatas = extraParams.receiver as IAttachDatas;
            if (attachDatas == null)
            {
                DebugUtils.Log("[Timeline CMD] receiver 不存在IAttachDatas接口");
                return false;
            }
            if (attachDatas.AttachDatas.TryGetValue(flagName, out object value))
            {
                bool _value = false;
                try
                {
                    _value = Convert.ToBoolean(value);
                    Debug.Log(flagName +":" + _value);
                    return _value == cond;
                }
                catch (Exception e)
                {
                    DebugUtils.Log($"[TImeline CMD] 值\"{flagName}\"无法被转换为Boolean类型\n\n{e.ToString()}");
                }
            }
            return false;
        }

        /// <summary>
        /// 跳转至特定markerer，需要在同一个track
        /// </summary>
        /// <param name="markerName"></param>
        /// <param name="extraParams"></param>
        [PBCommand(PBCmdEmitter.CMD_PREFIX + "JumpToMark", false, true)]
        [PBCommand(PBCmdEmitter.CMD_PREFIX + "JumpToMarker", false, true)]
        private static void CmdJumpToMarker(string markerName, TimelineCmdExtraParams extraParams)
        {
            if (extraParams == null)
                return;
            string _toMarkerName = markerName.ToLower();
            if (_toMarkerName.Equals(extraParams.emitter.name.ToLower()))
            {
                DebugUtils.Log("[Timeline CMD] JumpToMark 无法跳到自身");
                return;
            }
            int trackMarkCount = extraParams.emitter.parent.GetMarkerCount();
            for (int i = 0; i < trackMarkCount; i++)
            {
                var marker = extraParams.emitter.parent.GetMarker(i) as Marker;
                if (marker && _toMarkerName.Equals(marker.name.ToLower()))
                {
                    for (int playableIdx = 0; playableIdx < extraParams.origin.GetGraph().GetRootPlayableCount(); playableIdx++)
                    {
                        var playable = extraParams.origin.GetGraph().GetRootPlayable(playableIdx);
                        if (playable.IsNull())
                            continue;
                        playable.SetTime(marker.time);
                        return;
                    }
                }
            }
            DebugUtils.Log($"[Timeline CMD] JumpToMark失败，无法找到名称为{markerName}的Makerer");
        }
        
        [PBCommand(PBCmdEmitter.CMD_PREFIX + "FadeAnimState", false, true)]
        private static void CmdFadeAnimState(string stateName, float fadeDur, bool inFixedTime = true, TimelineCmdExtraParams extraParams = null)
        {
            if (extraParams == null)
                return;
            var keys = extraParams.referencesMap.Keys;
            foreach (string key in keys)
            {
                var obj = extraParams.GetReference<GameObject>(key);
                if (obj == null || obj.GetInstanceID() < 0)
                    continue;
                Animator animator = obj.GetComponent<Animator>();
                if (!animator)
                    continue;
                if (inFixedTime)
                {
                    animator.CrossFadeInFixedTime(stateName, fadeDur);
                }
                else
                {
                    animator.CrossFade(stateName, fadeDur);
                }
            }
        }

    }
}