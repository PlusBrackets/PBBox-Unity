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
    /// 帧更新事件
    /// </summary>
    public readonly partial struct UpdateFrameEvent
    {
        public readonly float DeltaTime;

        public UpdateFrameEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}