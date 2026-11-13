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
    public partial struct UpdateFrameEvent : IEvent
    {
#if UNITY_5_3_OR_NEWER
        [AutoStaticsCleanup]
#endif
        public static EventSubject<UpdateFrameEvent> Subject { get; private set; } = new EventSubject<UpdateFrameEvent>();

        public readonly float DeltaTime;
        public readonly int FrameCount;

        public UpdateFrameEvent(float deltaTime, int frameCount)
        {
            DeltaTime = deltaTime;
            FrameCount = frameCount;
        }
    }
}