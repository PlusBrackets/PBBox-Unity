/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using PBBox.Event;
#if UNITY_5_3_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace PBBox
{
    /// <summary>
    /// 帧更新事件数据
    /// </summary>
    public readonly struct FrameUpdateEvent
    {
        public readonly float DeltaTime;
        public readonly float UnscaledDeltaTime;

        public FrameUpdateEvent(float deltaTime, float unscaledDeltaTime)
        {
            DeltaTime = deltaTime;
            UnscaledDeltaTime = unscaledDeltaTime;
        }
    }
}