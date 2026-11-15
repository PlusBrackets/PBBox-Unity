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
    public static partial class PBEventBus
    {
#if UNITY_5_3_OR_NEWER
        [AutoStaticsCleanup]
#endif
        internal static EventSubject<UpdateFrameEvent> FrameUpdateSubject { get; private set; } = new EventSubject<UpdateFrameEvent>();
#if UNITY_5_3_OR_NEWER
        [AutoStaticsCleanup]
#endif
        internal static EventSubject ApplicationQuitSubject { get; private set; } = new EventSubject();

        /// <summary>
        /// 帧更新事件
        /// </summary>
        public static IEventSubjectIn<UpdateFrameEvent> OnFrameUpdate => FrameUpdateSubject;
        /// <summary>
        /// 应用程序退出事件
        /// </summary>
        public static IEventSubjectNoParam OnApplicationQuit => ApplicationQuitSubject;
    }
}