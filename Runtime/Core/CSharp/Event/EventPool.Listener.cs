/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.24
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using PBBox.Collections;

namespace PBBox
{
    /// <summary>
    /// 事件处理器
    /// </summary>
    public sealed partial class EventPool : ISubscriptionHandler
    {
        #region 添加事件监听器

        /// <summary>
        /// 添加事件监听器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public EventSubscription On(int eventId, Action<object> handler, int order = 0) => SubscribeImpl(eventId, handler, order, false);

        /// <summary>
        /// 添加带参数的事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public EventSubscription On<T>(int eventId, Action<object,T> handler, int order = 0) => SubscribeImpl(eventId, handler, order, false);


        /// <summary>
        /// 添加事件过滤器，可通过返回false中断事件传播
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public EventSubscription On(int eventId, Func<object,bool> filter, int order = 0) => SubscribeImpl(eventId, filter, order, false);

        /// <summary>
        /// 添加带参数的事件过滤器，可通过返回false中断事件传播
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public EventSubscription On<T>(int eventId, Func<object,T,bool> filter, int order = 0) => SubscribeImpl(eventId, filter, order, false);

        #endregion

        #region 添加延迟事件监听器

        /// <summary>
        /// 添加事件监听器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public EventSubscription OnLater(int eventId, Action<object> handler, int order = 0) => SubscribeImpl(eventId, handler, order, true);

        /// <summary>
        /// 添加带参数的事件监听器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public EventSubscription OnLater<T>(int eventId, Action<object,T> handler, int order = 0) => SubscribeImpl(eventId, handler, order, true);

        /// <summary>
        /// 添加事件过滤器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public EventSubscription OnLater(int eventId, Func<object,bool> filter, int order = 0) => SubscribeImpl(eventId, filter, order, true);

        /// <summary>
        /// 添加带参数的事件过滤器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public EventSubscription OnLater<T>(int eventId, Func<object,T,bool> filter, int order = 0) => SubscribeImpl(eventId, filter, order, true);

        #endregion

        #region 移除事件监听器

        /// <summary>
        /// 移除事件监听器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void Off(int eventId, Action<object> handler, int order = 0) => UnsubscribeImpl(eventId, handler, order, false);

        /// <summary>
        /// 移除带参数的事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void Off<T>(int eventId, Action<object,T> handler, int order = 0) => UnsubscribeImpl(eventId, handler, order, false);

        /// <summary>
        /// 移除事件过滤器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void Off(int eventId, Func<object,bool> filter, int order = 0) => UnsubscribeImpl(eventId, filter, order, false);

        /// <summary>
        /// 移除带参数的事件过滤器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void Off<T>(int eventId, Func<object,T,bool> filter, int order = 0) => UnsubscribeImpl(eventId, filter, order, false);

        #endregion

        #region 移除延迟事件监听器

        /// <summary>
        /// 移除延迟事件监听器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void OffLater(int eventId, Action<object> handler, int order = 0) => UnsubscribeImpl(eventId, handler, order, true);

        /// <summary>
        /// 移除带参数的延迟事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void OffLater<T>(int eventId, Action<object,T> handler, int order = 0) => UnsubscribeImpl(eventId, handler, order, true);

        /// <summary>
        /// 移除延迟事件过滤器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void OffLater(int eventId, Func<object,bool> filter, int order = 0) => UnsubscribeImpl(eventId, filter, order, true);

        /// <summary>
        /// 移除带参数的延迟事件过滤器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void OffLater<T>(int eventId, Func<object,T,bool> filter, int order = 0) => UnsubscribeImpl(eventId, filter, order, true);

        #endregion

        #region 触发事件

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender">发送者</param>
        public void Emit(int eventId, object sender) => EmitImpl(eventId, sender);

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="eventId"></param>
        /// <param name="sender">发送者</param>
        /// <param name="args">参数</param>
        /// <param name="releaseEventArgsPool">如果传入的参数需要在触发完毕后Release，则传入</param>
        public void Emit<TEventArgs>(int eventId, object sender, TEventArgs args, IReferenceCacheBase releaseEventArgsPool = null)=>
            EmitImpl(eventId, sender, args, releaseEventArgsPool);

        EventSubscription ISubscriptionHandler.Subscribe(int eventId, Delegate handler, int order, bool isLateEvent) => SubscribeImpl(eventId, handler, order, isLateEvent);

        void ISubscriptionHandler.Unsubscripe(int eventId, Delegate listener, int order, bool isLateEvent) => UnsubscribeImpl(eventId, listener, order, isLateEvent);

        #endregion
    }
}
