/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.24
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using PBBox.Collections;

namespace PBBox
{
    /// <summary>
    /// 事件管理器接口
    /// </summary>
    public interface IEventManager : ISingleton<IEventManager>
    {

        #region 添加事件监听器
        /// <summary>
        /// 添加事件监听器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription On(int eventId, Action<object> handler, int order = 0);

        /// <summary>
        /// 添加事件监听器
        /// </summary>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription On(string eventName, Action<object> handler, int order = 0)=> On(eventName.GetStableHashCode(), handler, order);

        /// <summary>
        /// 添加带参数的事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription On<T>(int eventId, Action<object, T> handler, int order = 0);

        /// <summary>
        /// 添加带参数的事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription On<T>(string eventName, Action<object, T> handler, int order = 0) => On(eventName.GetStableHashCode(), handler, order);

        /// <summary>
        /// 添加事件过滤器，可通过返回false中断事件传播
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription On(int eventId, Func<object, bool> filter, int order = 0);

        /// <summary>
        /// 添加事件过滤器，可通过返回false中断事件传播
        /// </summary>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription On(string eventName, Func<object, bool> filter, int order = 0) => On(eventName.GetStableHashCode(), filter, order);

        /// <summary>
        /// 添加带参数的事件过滤器，可通过返回false中断事件传播
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription On<T>(int eventId, Func<object, T, bool> filter, int order = 0);

        /// <summary>
        /// 添加带参数的事件过滤器，可通过返回false中断事件传播
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription On<T>(string eventName, Func<object, T, bool> filter, int order = 0) => On(eventName.GetStableHashCode(), filter, order);
        #endregion

        #region 添加延迟事件监听器
        /// <summary>
        /// 添加事件监听器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription OnLater(int eventId, Action<object> handler, int order = 0);

        /// <summary>
        /// 添加事件监听器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription OnLater(string eventName, Action<object> handler, int order = 0)=> OnLater(eventName.GetStableHashCode(), handler, order);

        /// <summary>
        /// 添加带参数的事件监听器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription OnLater<T>(int eventId, Action<object, T> handler, int order = 0);

        /// <summary>
        /// 添加带参数的事件监听器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription OnLater<T>(string eventName, Action<object, T> handler, int order = 0)=> OnLater(eventName.GetStableHashCode(), handler, order);

        /// <summary>
        /// 添加事件过滤器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription OnLater(int eventId, Func<object, bool> filter, int order = 0);

        /// <summary>
        /// 添加事件过滤器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription OnLater(string eventName, Func<object, bool> filter, int order = 0) => OnLater(eventName.GetStableHashCode(), filter, order);

        /// <summary>
        /// 添加带参数的事件过滤器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription OnLater<T>(int eventId, Func<object, T, bool> filter, int order = 0);

        /// <summary>
        /// 添加带参数的事件过滤器，触发事件时不会立刻调用，而是会等到Update时调用
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称，将被转换为事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        EventSubscription OnLater<T>(string eventName, Func<object, T, bool> filter, int order = 0) => OnLater(eventName.GetStableHashCode(), filter, order);
        #endregion

        #region 移除事件监听器
        /// <summary>
        /// 移除事件监听器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void Off(int eventId, Action<object> handler, int order = 0);

        /// <summary>
        /// 移除事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void Off(string eventName, Action<object> handler, int order = 0) => Off(eventName.GetStableHashCode(), handler, order);

        /// <summary>
        /// 移除带参数的事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void Off<T>(int eventId, Action<object, T> handler, int order = 0);

        /// <summary>
        /// 移除带参数的事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void Off<T>(string eventName, Action<object, T> handler, int order = 0) => Off(eventName.GetStableHashCode(), handler, order);

        /// <summary>
        /// 移除事件过滤器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void Off(int eventId, Func<object, bool> filter, int order = 0);

        /// <summary>
        /// 移除事件过滤器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void Off(string eventName, Func<object, bool> filter, int order = 0) => Off(eventName.GetStableHashCode(), filter, order);

        /// <summary>
        /// 移除带参数的事件过滤器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void Off<T>(int eventId, Func<object, T, bool> filter, int order = 0);

        /// <summary>
        /// 移除带参数的事件过滤器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void Off<T>(string eventName, Func<object, T, bool> filter, int order = 0) => Off(eventName.GetStableHashCode(), filter, order);
        #endregion

        #region 移除延迟事件监听器
        /// <summary>
        /// 移除延迟事件监听器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void OffLater(int eventId, Action<object> handler, int order = 0);

        /// <summary>
        /// 移除延迟事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void OffLater(string eventName, Action<object> handler, int order = 0) => OffLater(eventName.GetStableHashCode(), handler, order);

        /// <summary>
        /// 移除带参数的延迟事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void OffLater<T>(int eventId, Action<object, T> handler, int order = 0);

        /// <summary>
        /// 移除带参数的延迟事件监听器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件处理器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void OffLater<T>(string eventName, Action<object, T> handler, int order = 0) => OffLater(eventName.GetStableHashCode(), handler, order);

        /// <summary>
        /// 移除延迟事件过滤器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void OffLater(int eventId, Func<object, bool> filter, int order = 0);

        /// <summary>
        /// 移除延迟事件过滤器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void OffLater(string eventName, Func<object, bool> filter, int order = 0) => OffLater(eventName.GetStableHashCode(), filter, order);

        /// <summary>
        /// 移除带参数的延迟事件过滤器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void OffLater<T>(int eventId, Func<object, T, bool> filter, int order = 0);

        /// <summary>
        /// 移除带参数的延迟事件过滤器
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="filter">事件过滤器</param>
        /// <param name="order">注册时使用的顺序值</param>
        void OffLater<T>(string eventName, Func<object, T, bool> filter, int order = 0) => OffLater(eventName.GetStableHashCode(), filter, order);
        #endregion

        #region 触发事件
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="sender">发送者</param>
        void Emit(int eventId, object sender);

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="sender">发送者</param>
        void Emit(string eventName, object sender) => Emit(eventName.GetStableHashCode(), sender);

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="TEventArgs">事件参数类型</typeparam>
        /// <param name="eventId">事件ID</param>
        /// <param name="sender">发送者</param>
        /// <param name="args">参数</param>
        /// <param name="releaseEventArgsPool">如果传入的参数需要在触发完毕后Release，则传入</param>
        void Emit<TEventArgs>(int eventId, object sender, TEventArgs args, IReferenceCacheBase releaseEventArgsPool = null);

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="TEventArgs">事件参数类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="sender">发送者</param>
        /// <param name="args">参入</param>
        /// <param name="releaseEventArgsPool">如果传入的参数需要在触发完毕后Release，则传入</param>
        void Emit<TEventArgs>(string eventName, object sender, TEventArgs args, IReferenceCacheBase releaseEventArgsPool = null) => Emit(eventName.GetStableHashCode(), sender, args, releaseEventArgsPool);
        #endregion

    }
}