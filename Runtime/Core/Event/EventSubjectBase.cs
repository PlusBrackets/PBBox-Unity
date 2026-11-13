/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.10
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using PBBox.Collections;

namespace PBBox.Event
{   
    /// <summary>
    /// 事件主题的基类，管理事件处理器的订阅和取消订阅
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    public abstract class EventSubjectBase<THandler> : IReferencePoolItem where THandler : Delegate
    {
        protected SortedIndexedList<THandler> _handlersCollection = null;

        protected bool _keepInBucketItemOrderly = true;
        protected int? _inBucketListCapacity = null;
        protected int? _maxCachedBucketCount = null;

        /// <summary>
        /// 当前集合是否严格按照处理器的添加顺序执行
        /// </summary>
        public bool KeepInBucketItemOrderly => _keepInBucketItemOrderly;

        bool IReferencePoolItem.IsUsing { get; set; }

        protected abstract IEventSubject Subject { get; }
        protected abstract string LogTag { get; }

        public EventSubjectBase()
        {
        }

        protected virtual EventSubscription SubscribeInternal(THandler handler, int order = 0)
        {
            if(handler == null)
            {
                return EventSubscription.Empty;
            }
            if (GetOrCreateHandlers().TryAdd(handler, order))
            {
                return new EventSubscription(Subject, handler, order);
            }
            else
            {
#if UNITY_EDITOR || PB_TEST_LOG
                Log.Error($"Failed to add handler to EventSubject, may be already added, handler: {handler}, order: {order}", LogTag, Log.PBBoxLoggerName);
#endif
                return EventSubscription.Empty;
            }
        }

        protected virtual void UnsubscribeInternal(THandler handler, int order = 0)
        {
            if (handler == null)
            {
                return;
            }
            if (GetOrCreateHandlers().TryRemove(handler, order))
            {
                return;
            }
#if UNITY_EDITOR || PB_TEST_LOG
            else
            {
                Log.Error($"Failed to remove handler from EventSubject, may be not added or already removed, handler: {handler}, order: {order}", LogTag, Log.PBBoxLoggerName);
            }
#endif
        }

        /// <summary>
        /// 设置集合的参数，包括是否保持桶内项目有序以及桶内列表的初始容量
        /// </summary>
        /// <param name="keepInBucketItemOrderly"></param>
        /// <param name="inBucketListCapacity"></param>
        /// <param name="maxCachedBucketCount"></param>
        /// <returns>是否成功应用参数，False表示未能应用全部参数</returns>
        public bool SetSubjectParameter(bool keepInBucketItemOrderly, int? inBucketListCapacity = null, int? maxCachedBucketCount = null)
        {
            bool result = true;
            if (_handlersCollection != null)
            {
                result = _handlersCollection.SetCollectionParameter(keepInBucketItemOrderly, inBucketListCapacity, maxCachedBucketCount);
            }
            _keepInBucketItemOrderly = keepInBucketItemOrderly;
            _inBucketListCapacity = inBucketListCapacity;
            _maxCachedBucketCount = maxCachedBucketCount;
            return result;
        }

        protected SortedIndexedList<THandler> GetOrCreateHandlers()
        {
            if (_handlersCollection == null)
            {
                _handlersCollection = _inBucketListCapacity.HasValue
                    ? new SortedIndexedList<THandler>(_inBucketListCapacity.Value, _keepInBucketItemOrderly)
                    : new SortedIndexedList<THandler>(_keepInBucketItemOrderly);
                _handlersCollection.SetCollectionParameter(_keepInBucketItemOrderly, _inBucketListCapacity, _maxCachedBucketCount);
            }
            return _handlersCollection;
        }

        void IReferencePoolItem.OnReferenceAcquire()
        {
            OnReferenceAcquire();
        }

        void IReferencePoolItem.OnReferenceRelease()
        {
            OnReferenceRelease();
        }

        protected virtual void OnReferenceAcquire()
        {
        }

        protected virtual void OnReferenceRelease()
        {
            _handlersCollection.Clear();
            _keepInBucketItemOrderly = true;
            _inBucketListCapacity = null;
        }

    }
}