/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox.Event
{
    /// <summary>
    /// 事件主题接口，定义了订阅、取消订阅和发布事件的方法
    /// </summary>
    public interface IEventSubject
    {
        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="order">handler执行的顺序，值越小越先执行</param>
        /// <returns>事件订阅Handle，可用于管理订阅的生命周期</returns>
        EventSubscription Subscribe(Delegate handler, int order = 0);
        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="order">handler执行的顺序，值越小越先执行，需要和订阅时的order保持一致</param>
        void Unsubscribe(Delegate handler, int order = 0);
    }

    /// <summary>
    /// 无参数事件主题接口，定义了订阅、取消订阅和发布无参数事件的方法
    /// </summary>
    public interface IEventSubjectNoParam : IEventSubject
    {
        /// <summary>
        /// 订阅无参数事件
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="order">handler执行的顺序，值越小越先执行</param>
        /// <returns>事件订阅对象，可用于管理订阅的生命周期</returns>
        EventSubscription Subscribe(Action handler, int order = 0);
        /// <summary>
        /// 取消订阅无参数事件
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="order">handler执行的顺序，值越小越先执行，需要和订阅时的order保持一致</param>
        void Unsubscribe(Action handler, int order = 0);
        /// <summary>
        /// 发布无参数事件
        /// </summary>
        void Publish();
    }

    /// <summary>
    /// 泛型事件主题接口，定义了针对任意类型事件的发布方法
    /// </summary>
    public interface IEventSubjectGeneric : IEventSubjectNoParam
    {
        /// <summary>
        /// 订阅任意支持参数的事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="order">handler执行的顺序，值越小越先执行</param>
        /// <returns>事件订阅对象，可用于管理订阅的生命周期</returns>
        EventSubscription Subscribe<T>(Action<T> handler, int order = 0);
        /// <summary>
        /// 取消订阅任意支持参数的事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="order">handler执行的顺序，值越小越先执行，需要和订阅时的order保持一致</param>
        void Unsubscribe<T>(Action<T> handler, int order = 0);
        /// <summary>
        /// 订阅任意支持参数的事件（按引用传递）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="order">handler执行的顺序，值越小越先执行</param>
        /// <returns>事件订阅对象，可用于管理订阅的生命周期</returns>
        EventSubscription Subscribe<T>(ActionIn<T> handler, int order = 0);
        /// <summary>
        /// 取消订阅任意支持参数的事件（按引用传递）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="order">handler执行的顺序，值越小越先执行，需要和订阅时的order保持一致</param>
        void Unsubscribe<T>(ActionIn<T> handler, int order = 0);
        
        /// <summary>
        /// 发布事件，传入事件数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventData"></param>
        void Publish<T>(T eventData);
        /// <summary>
        /// 发布事件，传入事件数据（按引用传递），用于大型结构体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventData"></param>
        void Publish<T>(in T eventData);
    }

    /// <summary>
    /// 泛型事件主题接口，定义了针对特定事件类型的订阅、取消订阅和发布事件的方法
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IEventSubject<TEvent> : IEventSubject
    {
        /// <summary>
        /// 订阅特定事件类型的事件
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="order">handler执行的顺序，值越小越先执行</param>
        /// <returns>事件订阅对象，可用于管理订阅的生命周期</returns>
        EventSubscription Subscribe(Action<TEvent> handler, int order = 0);
        /// <summary>
        /// 取消订阅特定事件类型的事件
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="order">handler执行的顺序，值越小越先执行，需要和订阅时的order保持一致</param>
        void Unsubscribe(Action<TEvent> handler, int order = 0);
        /// <summary>
        /// 发布特定事件类型的事件
        /// </summary>
        /// <param name="eventData"></param>
        void Publish(TEvent eventData);
    }

    /// <summary>
    /// 泛型事件主题接口，定义了针对特定事件类型的按引用传递的订阅、取消订阅和发布事件的方法
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IEventSubjectIn<TEvent> : IEventSubject
    {
        /// <summary>
        /// 订阅特定事件类型的按引用传递的事件
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="order">handler执行的顺序，值越小越先执行</param>
        /// <returns>事件订阅对象，可用于管理订阅的生命周期</returns>
        EventSubscription Subscribe(ActionIn<TEvent> handler, int order = 0);
        /// <summary>
        /// 取消订阅特定事件类型的按引用传递的事件
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="order">handler执行的顺序，值越小越先执行，需要和订阅时的order保持一致</param>
        void Unsubscribe(ActionIn<TEvent> handler, int order = 0);
        /// <summary>
        /// 发布特定事件类型的按引用传递的事件
        /// </summary>
        /// <param name="eventData"></param>
        /// <remarks>按引用传递的事件数据，用于大型结构体</remarks>
        void Publish(in TEvent eventData);
    }
}