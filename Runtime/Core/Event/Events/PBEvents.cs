/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
using PBBox.Event;
#if UNITY_5_3_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace PBBox
{
    /// <summary>
    /// PBBox内置的一些事件
    /// </summary>
    public static partial class PBEvents
    {
#if UNITY_5_3_OR_NEWER
        [AutoStaticsCleanup]
#endif
        internal static EventSubject<FrameUpdateEvent> FrameUpdateSubject { get; private set; } = null;
#if UNITY_5_3_OR_NEWER
        [AutoStaticsCleanup]
#endif
        internal static EventSubject ApplicationQuitSubject { get; private set; } = null;

        /// <summary>
        /// 帧更新事件
        /// </summary>
        public static IEventSubjectIn<FrameUpdateEvent> OnFrameUpdate
        {
            get
            {
                if (FrameUpdateSubject == null)
                {
                    FrameUpdateSubject = new EventSubject<FrameUpdateEvent>();
                }
                return FrameUpdateSubject;
            }
        }

        /// <summary>
        /// 应用程序退出事件
        /// </summary>
        public static IEventSubjectNoParam OnApplicationQuit
        {
            get
            {
                if (ApplicationQuitSubject == null)
                {
                    ApplicationQuitSubject = new EventSubject();
                }
                return ApplicationQuitSubject;
            }
        }
    }
}