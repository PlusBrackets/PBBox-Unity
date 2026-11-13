/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    /// <summary>
    /// 表示带有输入参数的事件处理委托，用于处理结构体类型的事件数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventData"></param>
    public delegate void ActionIn<T>(in T eventData);
    
    /// <summary>
    /// 事件管理器接口，提供订阅、取消订阅和发布事件的方法
    /// </summary>
    public interface IEventManager : ISingleton<IEventManager>
    {
        /// <summary>
        /// 订阅特定事件类型的事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <param name="isLater">是否为延迟事件，通常为下一帧或者该帧稍后执行</param>
        /// <returns>返回事件订阅对象，可用于管理订阅的生命周期</returns>
        EventSubscription Subscribe<TEvent>(Action<TEvent> handler, int order = 0, bool isLater = false) where TEvent : IEvent;
        /// <summary>
        /// 取消订阅特定事件类型的事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <param name="isLater">是否为延迟事件，通常为下一帧或者该帧稍后执行</param>
        void Unsubscribe<TEvent>(Action<TEvent> handler, int order = 0, bool isLater = false) where TEvent : IEvent;
        /// <summary>
        /// 发布特定事件类型的事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventData"></param>
        void Publish<TEvent>(TEvent eventData) where TEvent : IEvent;

        /// <summary>
        /// 订阅特定值类型事件，可用于大型结构体类型的事件数据
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <param name="isLater">是否为延迟事件，通常为下一帧或者该帧稍后执行</param>
        /// <returns>返回事件订阅对象，可用于管理订阅的生命周期</returns>
        EventSubscription Subscribe<TEvent>(ActionIn<TEvent> handler, int order = 0, bool isLater = false) where TEvent : struct, IEvent;
        /// <summary>
        /// 取消订阅特定值类型事件，可用于大型结构体类型的事件数据
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <param name="isLater">是否为延迟事件，通常为下一帧或者该帧稍后执行</param>
        void Unsubscribe<TEvent>(ActionIn<TEvent> handler, int order = 0, bool isLater = false) where TEvent : struct, IEvent;
        /// <summary>
        /// 发布特定值类型事件，可用于大型结构体类型的事件数据
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventData"></param>
        void Publish<TEvent>(in TEvent eventData) where TEvent : struct, IEvent;

        /// <summary>
        /// 订阅通用事件, 通过事件ID进行订阅
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <param name="isLater">是否为延迟事件，通常为下一帧或者该帧稍后执行</param>
        /// <returns>返回事件订阅对象，可用于管理订阅的生命周期</returns>
        EventSubscription SubscribeGeneric(int eventId, Delegate handler, int order = 0, bool isLater = false);
        /// <summary>
        /// 取消订阅通用事件, 通过事件ID进行取消订阅
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <param name="isLater">是否为延迟事件，通常为下一帧或者该帧稍后执行</param>
        void UnsubscribeGeneric(int eventId, Delegate handler, int order = 0, bool isLater = false);
        /// <summary>
        /// 发布通用事件, 通过事件ID进行发布
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="eventId"></param>
        /// <param name="eventData"></param>
        void Publish<TData>(int eventId, TData eventData);

        /// <summary>
        /// 发布通用事件，通过事件ID进行发布，可用于大型结构体类型的事件数据
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="eventId"></param>
        /// <param name="eventData"></param>
        void Publish<TData>(int eventId, in TData eventData) where TData : struct;

        /// <summary>
        /// 发布通用事件，通过事件ID进行发布
        /// </summary>
        /// <param name="eventId"></param>
        void Publish(int eventId);

        /// <summary>
        /// 订阅通用事件，通过事件ID进行订阅
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="eventId"></param>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <param name="isLater">是否为延迟事件，通常为下一帧或者该帧稍后执行</param>
        /// <returns>返回事件订阅对象</returns>
        EventSubscription Subscribe<TData>(int eventId, Action<TData> handler, int order = 0, bool isLater = false) => SubscribeGeneric(eventId, handler, order, isLater);
        
        /// <summary>
        /// 订阅通用事件，通过事件ID进行订阅
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="eventId"></param>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <param name="isLater">是否为延迟事件，通常为下一帧或者该帧稍后执行</param>
        /// <returns>返回事件订阅对象</returns>
        EventSubscription Subscribe<TData>(int eventId, ActionIn<TData> handler, int order = 0, bool isLater = false) where TData : struct => SubscribeGeneric(eventId, handler, order, isLater);
        /// <summary>
        /// 订阅通用事件，通过事件ID进行订阅
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <param name="isLater">是否为延迟事件，通常为下一帧或者该帧稍后执行</param>
        /// <returns>返回事件订阅对象</returns>
        EventSubscription Subscribe<TData>(int eventId, Action handler, int order = 0, bool isLater = false) => SubscribeGeneric(eventId, handler, order, isLater);
        /// <summary>
        /// 取消订阅通用事件，通过事件ID进行取消订阅
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="eventId"></param>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <param name="isLater">是否为延迟事件，通常为下一帧或者该帧稍后执行</param>
        void Unsubscribe<TData>(int eventId, Action<TData> handler, int order = 0, bool isLater = false) => UnsubscribeGeneric(eventId, handler, order, isLater);
        /// <summary>
        /// 取消订阅通用事件，通过事件ID进行取消订阅
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="eventId"></param>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <param name="isLater">是否为延迟事件，通常为下一帧或者该帧稍后执行</param>
        void Unsubscribe<TData>(int eventId, ActionIn<TData> handler, int order = 0, bool isLater = false) where TData : struct => UnsubscribeGeneric(eventId, handler, order, isLater);
        /// <summary>
        /// 取消订阅通用事件，通过事件ID进行取消订阅
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="eventId"></param>
        /// <param name="handler"></param>
        /// <param name="order">订阅的优先级顺序</param>
        /// <param name="isLater">是否为延迟事件，通常为下一帧或者该帧稍后执行</param>
        void Unsubscribe(int eventId, Action handler, int order = 0, bool isLater = false) => UnsubscribeGeneric(eventId, handler, order, isLater);
    }
}