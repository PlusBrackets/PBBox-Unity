/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.24
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using PBBox.Collections;
using UnityEngine;

namespace PBBox
{
    /// <summary>
    /// 事件处理器
    /// </summary>
    public sealed partial class EventPool
    {
        /// <summary>
        /// 事件处理器
        /// </summary>
        /// <param name="sender">发送者</param>
        public delegate void Handler(object sender);
        /// <summary>
        /// 事件处理器，T为事件参数类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sender">发送者</param>
        /// <param name="arg">参数</param>
        public delegate void Handler<T>(object sender, T arg);
        /// <summary>
        /// 事件处理器，返回值为bool，表示是否继续触发后续的事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <returns>是否继续触发后续事件</returns>
        public delegate bool EventFilter(object sender);
        /// <summary>
        /// 事件处理器，返回值为bool，表示是否继续触发后续的事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sender">发送者</param>
        /// <param name="arg">参数</param>
        /// <returns></returns>
        public delegate bool EventFilter<T>(object sender, T arg);

        #region 添加事件监听器

        /// <summary>
        /// 添加事件监听器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription On(int eventId, Handler handler, int order = 0) => SubscribeImpl(eventId, handler, order, false);

        /// <summary>
        /// 添加事件监听器
        /// </summary>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription On(string eventName, Handler handler, int order = 0) => SubscribeImpl(GetEventId(eventName), handler, order, false);

        /// <summary>
        /// 添加带参数的事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription On<T>(int eventId, Handler<T> handler, int order = 0) => SubscribeImpl(eventId, handler, order, false);

        /// <summary>
        /// 添加带参数的事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription On<T>(string eventName, Handler<T> handler, int order = 0) => SubscribeImpl(GetEventId(eventName), handler, order, false);

        /// <summary>
        /// 添加事件过滤器，可通过返回false中断事件传播
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription On(int eventId, EventFilter filter, int order = 0) => SubscribeImpl(eventId, filter, order, false);

        /// <summary>
        /// 添加事件过滤器，可通过返回false中断事件传播
        /// </summary>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription On(string eventName, EventFilter filter, int order = 0) => SubscribeImpl(GetEventId(eventName), filter, order, false);

        /// <summary>
        /// 添加带参数的事件过滤器，可通过返回false中断事件传播
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription On<T>(int eventId, EventFilter<T> filter, int order = 0) => SubscribeImpl(eventId, filter, order, false);

        /// <summary>
        /// 添加带参数的事件过滤器，可通过返回false中断事件传播
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription On<T>(string eventName, EventFilter<T> filter, int order = 0) => SubscribeImpl(GetEventId(eventName), filter, order, false);

        #endregion

        #region 添加延迟事件监听器

        /// <summary>
        /// 添加事件监听器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription OnLater(int eventId, Handler handler, int order = 0) => SubscribeImpl(eventId, handler, order, true);

        /// <summary>
        /// 添加事件监听器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription OnLater(string eventName, Handler handler, int order = 0) => SubscribeImpl(GetEventId(eventName), handler, order, true);

        /// <summary>
        /// 添加带参数的事件监听器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription OnLater<T>(int eventId, Handler<T> handler, int order = 0) => SubscribeImpl(eventId, handler, order, true);

        /// <summary>
        /// 添加带参数的事件监听器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription OnLater<T>(string eventName, Handler<T> handler, int order = 0) => SubscribeImpl(GetEventId(eventName), handler, order, true);

        /// <summary>
        /// 添加事件过滤器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription OnLater(int eventId, EventFilter filter, int order = 0) => SubscribeImpl(eventId, filter, order, true);

        /// <summary>
        /// 添加事件过滤器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription OnLater(string eventName, EventFilter filter, int order = 0) => SubscribeImpl(GetEventId(eventName), filter, order, true);

        /// <summary>
        /// 添加带参数的事件过滤器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription OnLater<T>(int eventId, EventFilter<T> filter, int order = 0) => SubscribeImpl(eventId, filter, order, true);

        /// <summary>
        /// 添加带参数的事件过滤器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        public Subscription OnLater<T>(string eventName, EventFilter<T> filter, int order = 0) => SubscribeImpl(GetEventId(eventName), filter, order, true);

        #endregion

        #region 移除事件监听器

        /// <summary>
        /// 移除事件监听器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void Off(int eventId, Handler handler, int order = 0) => UnsubscribeImpl(eventId, handler, order, false);

        /// <summary>
        /// 移除事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void Off(string eventName, Handler handler, int order = 0) => UnsubscribeImpl(GetEventId(eventName), handler, order, false);

        /// <summary>
        /// 移除带参数的事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void Off<T>(int eventId, Handler<T> handler, int order = 0) => UnsubscribeImpl(eventId, handler, order, false);

        /// <summary>
        /// 移除带参数的事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void Off<T>(string eventName, Handler<T> handler, int order = 0) => UnsubscribeImpl(GetEventId(eventName), handler, order, false);

        /// <summary>
        /// 移除事件过滤器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void Off(int eventId, EventFilter filter, int order = 0) => UnsubscribeImpl(eventId, filter, order, false);

        /// <summary>
        /// 移除事件过滤器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void Off(string eventName, EventFilter filter, int order = 0) => UnsubscribeImpl(GetEventId(eventName), filter, order, false);

        /// <summary>
        /// 移除带参数的事件过滤器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void Off<T>(int eventId, EventFilter<T> filter, int order = 0) => UnsubscribeImpl(eventId, filter, order, false);

        /// <summary>
        /// 移除带参数的事件过滤器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void Off<T>(string eventName, EventFilter<T> filter, int order = 0) => UnsubscribeImpl(GetEventId(eventName), filter, order, false);

        #endregion

        #region 移除延迟事件监听器

        /// <summary>
        /// 移除延迟事件监听器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void OffLater(int eventId, Handler handler, int order = 0) => UnsubscribeImpl(eventId, handler, order, true);

        /// <summary>
        /// 移除延迟事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void OffLater(string eventName, Handler handler, int order = 0) => UnsubscribeImpl(GetEventId(eventName), handler, order, true);

        /// <summary>
        /// 移除带参数的延迟事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void OffLater<T>(int eventId, Handler<T> handler, int order = 0) => UnsubscribeImpl(eventId, handler, order, true);

        /// <summary>
        /// 移除带参数的延迟事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void OffLater<T>(string eventName, Handler<T> handler, int order = 0) => UnsubscribeImpl(GetEventId(eventName), handler, order, true);

        /// <summary>
        /// 移除延迟事件过滤器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void OffLater(int eventId, EventFilter filter, int order = 0) => UnsubscribeImpl(eventId, filter, order, true);

        /// <summary>
        /// 移除延迟事件过滤器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void OffLater(string eventName, EventFilter filter, int order = 0) => UnsubscribeImpl(GetEventId(eventName), filter, order, true);

        /// <summary>
        /// 移除带参数的延迟事件过滤器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void OffLater<T>(int eventId, EventFilter<T> filter, int order = 0) => UnsubscribeImpl(eventId, filter, order, true);

        /// <summary>
        /// 移除带参数的延迟事件过滤器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        public void OffLater<T>(string eventName, EventFilter<T> filter, int order = 0) => UnsubscribeImpl(GetEventId(eventName), filter, order, true);

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
        /// <param name="eventName"></param>
        /// <param name="sender">发送者</param>
        public void Emit(string eventName, object sender) => EmitImpl(GetEventId(eventName), sender);

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

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="eventName"></param>
        /// <param name="sender">发送者</param>
        /// <param name="args">参入</param>
        /// <param name="releaseEventArgsPool">如果传入的参数需要在触发完毕后Release，则传入</param>
        public void Emit<TEventArgs>(string eventName, object sender, TEventArgs args, IReferenceCacheBase releaseEventArgsPool = null)=>
            EmitImpl(GetEventId(eventName), sender, args, releaseEventArgsPool);

        #endregion
    }
}
