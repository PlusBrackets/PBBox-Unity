/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.02.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    /// <summary>
    /// 事件管理器，支持int类型和string类型的key值，两类型会分开处理，避免产生不必要的hash碰撞
    /// </summary>
    public class EventManager : Singleton<EventManager>, ISingletonLifecycle, ILogicUpdateHandler<LogicUpdater.Default>
    {
        private EventPool<int> m_EventPoolInt;
        private EventPool<string> m_EventPoolString;

        LogicUpdater.Default ILogicUpdateHandler<LogicUpdater.Default>.CurrentUpdater { get; set; }

        int ILogicUpdateHandler<LogicUpdater.Default>.SortedOrder => -10;

        void ISingletonLifecycle.OnCreateAsSingleton()
        {
            m_EventPoolInt = new EventPool<int>();
            m_EventPoolString = new EventPool<string>();
            LogicUpdater.Attach(this);
        }

        void ISingletonLifecycle.OnDestroyAsSingleton()
        {
            LogicUpdater.Unattach(this);
        }

        void ILogicUpdateHandler<LogicUpdater.Default>.OnUpdate(float deltaTime)
        {
            m_EventPoolInt.Update();
            m_EventPoolString.Update();
        }

        #region int key event
        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public static void On<T>(int eventId, Action<object, T> handler, int order = 0) => Instance.m_EventPoolInt.On<T>(eventId, handler, order);

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        public static void On(int eventId, Action<object> handler, int order = 0) => Instance.m_EventPoolInt.On(eventId, handler, order);
        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler">返回false时可以中断事件传递</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public static void On<T>(int eventId, Func<object, T, bool> handler, int order = 0) => Instance.m_EventPoolInt.On<T>(eventId, handler, order);

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler">返回false时可以中断事件传递</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        public static void On(int eventId, Func<object, bool> handler, int order = 0) => Instance.m_EventPoolInt.On(eventId, handler, order);
        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public static void Off<T>(int eventId, Action<object, T> handler, int order = 0) => Instance.m_EventPoolInt.Off(eventId, handler, order);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        public static void Off(int eventId, Action<object> handler, int order = 0) => Instance.m_EventPoolInt.Off(eventId, handler, order);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler">返回false时可以中断事件传递</param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public static void Off<T>(int eventId, Func<object, T, bool> handler, int order = 0) => Instance.m_EventPoolInt.Off(eventId, handler, order);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler">返回false时可以中断事件传递</param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        public static void Off(int eventId, Func<object, bool> handler, int order = 0) => Instance.m_EventPoolInt.Off(eventId, handler, order);

        /// <summary>
        /// 发送事件，事件会在下一帧触发
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        /// <param name="eventArgs">事件参数可以继承EventArgsBase获得更多的功能</param>
        /// <typeparam name="T"></typeparam>
        public static void Emit<T>(int eventId, object sender, T eventArgs, IReferenceCacheBase releaseToReferenceCache = null) => Instance.m_EventPoolInt.Emit<T>(eventId, sender, eventArgs, releaseToReferenceCache);

        /// <summary>
        /// 发送事件，事件会在下一帧触发
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        public static void Emit(int eventId, object sender) => Instance.m_EventPoolInt.Emit(eventId, sender);

        /// <summary>
        /// 立即发送事件，这种触发是非线程安全的
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        /// <param name="eventArgs">事件参数可以继承EventArgsBase获得更多的功能</param>
        /// <typeparam name="T"></typeparam>
        public static void EmitNow<T>(int eventId, object sender, T eventArgs, IReferenceCacheBase releaseToReferenceCache = null) => Instance.m_EventPoolInt.EmitNow<T>(eventId, sender, eventArgs, releaseToReferenceCache);

        /// <summary>
        /// 立即发送事件，这种事件是非线程安全的
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        public static void EmitNow(int eventId, object sender) => Instance.m_EventPoolInt.EmitNow(eventId, sender);
        #endregion

        #region string key event
        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public static void On<T>(string eventId, Action<object, T> handler, int order = 0) => Instance.m_EventPoolString.On<T>(eventId, handler, order);

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        public static void On(string eventId, Action<object> handler, int order = 0) => Instance.m_EventPoolString.On(eventId, handler, order);
        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler">返回false时可以中断事件传递</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public static void On<T>(string eventId, Func<object, T, bool> handler, int order = 0) => Instance.m_EventPoolString.On<T>(eventId, handler, order);

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler">返回false时可以中断事件传递</param>
        /// <param name="order">接收事件的顺序，默认为0，越小越早接收事件，若无必要，请保持order=0</param>
        public static void On(string eventId, Func<object, bool> handler, int order = 0) => Instance.m_EventPoolString.On(eventId, handler, order);
        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public static void Off<T>(string eventId, Action<object, T> handler, int order = 0) => Instance.m_EventPoolString.Off(eventId, handler, order);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler"></param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        public static void Off(string eventId, Action<object> handler, int order = 0) => Instance.m_EventPoolString.Off(eventId, handler, order);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler">返回false时可以中断事件传递</param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        /// <typeparam name="T">事件参数类型，可以继承EventArgsBase或者IEventArgs获得更多的支持。</typeparam>
        public static void Off<T>(string eventId, Func<object, T, bool> handler, int order = 0) => Instance.m_EventPoolString.Off(eventId, handler, order);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件唯一id</param>
        /// <param name="handler">返回false时可以中断事件传递</param>
        /// <param name="order">接收事件的顺序，默认为0，需要与添加事件时保持一致</param>
        public static void Off(string eventId, Func<object, bool> handler, int order = 0) => Instance.m_EventPoolString.Off(eventId, handler, order);

        /// <summary>
        /// 发送事件，事件会在下一帧触发
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        /// <param name="eventArgs">事件参数可以继承EventArgsBase获得更多的功能</param>
        /// <typeparam name="T"></typeparam>
        public static void Emit<T>(string eventId, object sender, T eventArgs, IReferenceCacheBase releaseToReferenceCache = null) => Instance.m_EventPoolString.Emit<T>(eventId, sender, eventArgs, releaseToReferenceCache);

        /// <summary>
        /// 发送事件，事件会在下一帧触发
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        public static void Emit(string eventId, object sender) => Instance.m_EventPoolString.Emit(eventId, sender);

        /// <summary>
        /// 立即发送事件，这种触发是非线程安全的
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        /// <param name="eventArgs">事件参数可以继承EventArgsBase获得更多的功能</param>
        /// <typeparam name="T"></typeparam>
        public static void EmitNow<T>(string eventId, object sender, T eventArgs, IReferenceCacheBase releaseToReferenceCache = null) => Instance.m_EventPoolString.EmitNow<T>(eventId, sender, eventArgs, releaseToReferenceCache);

        /// <summary>
        /// 立即发送事件，这种事件是非线程安全的
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sender"></param>
        public static void EmitNow(string eventId, object sender) => Instance.m_EventPoolString.EmitNow(eventId, sender);
        #endregion
    }
}