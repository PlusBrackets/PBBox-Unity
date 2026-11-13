/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.13
 *@author: PlusBrackets
 --------------------------------------------------------*/

namespace PBBox.Event
{
    /// <summary>
    /// 事件主题管理器接口，提供获取或创建事件主题的方法
    /// </summary>
    public interface IEventSubjectManager
    {
        /// <summary>
        /// 获取或创建特定事件类型的事件主题
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="isLater">是否为延迟事件</param>
        /// <returns></returns>
        IEventSubject GetOrCreateSubject<TEvent>(bool isLater = false) where TEvent : IEvent;
        /// <summary>
        /// 获取或创建指定事件ID的事件主题
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="isLater">是否为延迟事件</param>
        /// <returns></returns>
        IEventSubject GetOrCreateSubject(int eventId, bool isLater = false);
        /// <summary>
        /// 获取特定事件类型的事件主题
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="isLater">是否为延迟事件</param>
        /// <returns></returns>
        IEventSubject GetSubject<TEvent>(bool isLater = false) where TEvent : IEvent;
        /// <summary>
        /// 获取指定事件ID的事件主题
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="isLater">是否为延迟事件</param>
        /// <returns></returns>
        IEventSubject GetSubject(int eventId, bool isLater = false);
    }
}