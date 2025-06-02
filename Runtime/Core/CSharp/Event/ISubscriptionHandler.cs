/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.02
 *@author: PlusBrackets
 --------------------------------------------------------*/
 using System;

namespace PBBox
{

    public interface ISubscriptionHandler
    {

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="handler"></param>
        /// <param name="order"></param>
        /// <param name="isLateEvent"></param>
        /// <returns></returns>
        EventSubscription Subscribe(int eventId, Delegate handler, int order = 0, bool isLateEvent = false);

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="listener"></param>
        /// <param name="order"></param>
        /// <param name="isLateEvent"></param>
        void Unsubscripe(int eventId, Delegate listener, int order = 0, bool isLateEvent = false);
    }
}