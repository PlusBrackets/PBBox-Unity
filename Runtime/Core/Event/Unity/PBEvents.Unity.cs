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
        internal static EventSubject<UpdateFrameEvent> LateUpdateSubject { get; private set; } = null;

        [AutoStaticsCleanup]
        internal static EventSubject<UpdateFrameEvent> FixedUpdateSubject { get; private set; } = null;
        
        /// <summary>
        /// Unity的LateUpdate事件
        /// </summary>
        public static IEventSubjectIn<UpdateFrameEvent> OnLateUpdate
        {
            get
            {
                if (LateUpdateSubject == null)
                {
                    LateUpdateSubject = new EventSubject<UpdateFrameEvent>();
                }
                return LateUpdateSubject;
            }
        }

        /// <summary>
        /// Unity的FixedUpdate事件
        /// </summary>
        public static IEventSubjectIn<UpdateFrameEvent> OnFixedUpdate
        {
            get
            {
                if (FixedUpdateSubject == null)
                {
                    FixedUpdateSubject = new EventSubject<UpdateFrameEvent>();
                }
                return FixedUpdateSubject;
            }
        }
    }
}
#endif