#if UNITY_5_3_OR_NEWER
/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using PBBox.Event;
using Unity.Scripting.LifecycleManagement;

namespace PBBox
{
    /// <summary>
    /// 帧更新事件
    /// </summary>
    public static partial class PBEventBus
    {
        [AutoStaticsCleanup]
        internal static EventSubject<UpdateFrameEvent> LateUpdateSubject { get; private set; } = new EventSubject<UpdateFrameEvent>();

        [AutoStaticsCleanup]
        internal static EventSubject<UpdateFrameEvent> FixedUpdateSubject { get; private set; } = new EventSubject<UpdateFrameEvent>();
        
        public static IEventSubjectIn<UpdateFrameEvent> OnLateUpdate => LateUpdateSubject;
        public static IEventSubjectIn<UpdateFrameEvent> OnFixedUpdate => FixedUpdateSubject;
    }
}
#endif