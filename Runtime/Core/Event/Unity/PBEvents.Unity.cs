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
    public static partial class PBEvents
    {
        [AutoStaticsCleanup]
        internal static EventSubject<FrameUpdateEvent> LateUpdateSubject { get; private set; } = null;

        [AutoStaticsCleanup]
        internal static EventSubject<FrameUpdateEvent> FixedUpdateSubject { get; private set; } = null;
        
        /// <summary>
        /// Unity的LateUpdate事件
        /// </summary>
        public static IEventSubjectIn<FrameUpdateEvent> OnLateUpdate
        {
            get
            {
                if (LateUpdateSubject == null)
                {
                    LateUpdateSubject = new EventSubject<FrameUpdateEvent>();
                }
                return LateUpdateSubject;
            }
        }

        /// <summary>
        /// Unity的FixedUpdate事件
        /// </summary>
        public static IEventSubjectIn<FrameUpdateEvent> OnFixedUpdate
        {
            get
            {
                if (FixedUpdateSubject == null)
                {
                    FixedUpdateSubject = new EventSubject<FrameUpdateEvent>();
                }
                return FixedUpdateSubject;
            }
        }
    }
}
#endif