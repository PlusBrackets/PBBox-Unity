/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.12
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using PBBox.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections;

namespace PBBox.Event
{
    //public delegate void EventHandlerCollectionForeachHandle<T, in T2>(in T eventData, T2 handlers);

    public class EventHandlerCollection<T> :IEnumerable<KeyValuePair<int, IndexedList<T>>>, IEnumerable where T : Delegate
    {
        public delegate void ForeachHandleIn<TValue>(in TValue eventData, List<T> handlers);
        
        private SortedDictionary<int, IndexedList<T>> _subscriptions = new SortedDictionary<int, IndexedList<T>>();
        private bool _keepInBucketItemOrderly;
        private int? _inBucketListCapacity;
        /// <summary>
        /// 最大缓存的订阅桶数量，超过该数量时会移除空的订阅桶
        /// </summary>
        public int MaxCachedSubscriptionsBucket { get; set; } = 8;
        /// <summary>
        /// 是否保持订阅桶内的项有序
        /// </summary>
        public bool KeepInBucketItemOrderly => _keepInBucketItemOrderly;

        public int Count { get; private set; }
        // 是否正在遍历集合，用于延迟处理对集合的修改
        private bool _isIterating = false;
        private int _currentIteratingOrder;
        private List<AddRemoveOperation> _pendingOperations;

        public EventHandlerCollection(bool keepInBucketItemOrderly = true)
        {
            _keepInBucketItemOrderly = keepInBucketItemOrderly;
            _inBucketListCapacity = null;
        }

        public EventHandlerCollection(int inBucketListCapacity, bool keepInBucketItemOrderly = true)
        {
            _keepInBucketItemOrderly = keepInBucketItemOrderly;
            _inBucketListCapacity = inBucketListCapacity;
        }

        public bool SetCollectionParameter(bool keepInBucketItemOrderly, int? inBucketListCapacity = null)
        {
            if (_subscriptions != null && _subscriptions.Count > 0)
            {
                Log.Error("Cannot change collection parameters while there are existing subscriptions.", nameof(EventHandlerCollection<T>), Log.PBBoxLoggerName);
                return false;
            }
            _keepInBucketItemOrderly = keepInBucketItemOrderly;
            _inBucketListCapacity = inBucketListCapacity;
            return true;
        }

        public static EventHandlerCollection<T> Create(bool keepInBucketItemOrderly = true, int? inBucketListCapacity = null)
        {
            return inBucketListCapacity.HasValue
                ? new EventHandlerCollection<T>(inBucketListCapacity.Value, keepInBucketItemOrderly)
                : new EventHandlerCollection<T>(keepInBucketItemOrderly);
        }

        /// <summary>
        /// 设置当前集合的正在被遍历的状态，处于遍历状态时对集合的修改会被延迟处理
        /// </summary>
        /// <param name="iterating"></param>
        /// <param name="order"></param>
        public void SetIterating(bool iterating, int order)
        {
            _isIterating = iterating;
            _currentIteratingOrder = order;
        }

        /// <summary>
        /// 应用所有待处理的添加和移除操作
        /// </summary>
        public void ApplyPendingOperations()
        {
            if (_pendingOperations == null) return;
            if (_isIterating)
            {
                Log.Error("Cannot apply pending operations while iterating.", nameof(EventHandlerCollection<T>), Log.PBBoxLoggerName);
                return;
            }
            foreach (var op in _pendingOperations)
            {
                if (op.IsAdd)
                {
                    Add(op.Handler, op.Order);
                }
                else
                {
                    Remove(op.Handler, op.Order);
                }
            }
            _pendingOperations.Clear();
        }

        private bool TryQueueAddOperation(T handler, int order, IndexedList<T> list)
        {
            bool result = true;
            if (_pendingOperations == null)
            {
                _pendingOperations = new List<AddRemoveOperation>();
            }
            int existingIndex = FindLastPendingOperation(null, handler, order);
            if (existingIndex < 0)
            {
                if (list != null && list.Contains(handler))
                {
                    //此前没有同类的待处理操作，但列表中已经存在该处理器，不能重复添加
                    result = false;
                }
                else
                {
                    //之前没有同类操作，可以添加
                    _pendingOperations.Add(new AddRemoveOperation(true, handler, order));
                }
            }
            else
            {
                var existingOp = _pendingOperations[existingIndex];
                if (!existingOp.IsAdd)
                {
                    //最近的是移除操作，可以添加
                    _pendingOperations.Add(new AddRemoveOperation(true, handler, order));
                }
                else
                {
                    //最近的是添加操作，不能重复添加
                    result = false;
                }
            }
#if PB_TEST_LOG || UNITY_EDITOR
            Log.Warning($"处于遍历中，尝试添加AddRemoveOperation: IsAdd={true}, Handler={handler}, Order={order} QueueSuccess={result}，此操作性能消耗较高，建议避免在遍历过程中频繁进行", nameof(EventHandlerCollection<T>), Log.PBBoxLoggerName);
#endif
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryQueueRemoveOperation(T handler, int order, IndexedList<T> list)
        {
            bool result = true;
            if (_pendingOperations == null)
            {
                _pendingOperations = new List<AddRemoveOperation>();
            }
            int existingIndex = FindLastPendingOperation(null, handler, order);
            if (existingIndex < 0)
            {
                if (list != null && list.Contains(handler))
                {
                    //此前没有同类的待处理操作，但列表中存在该处理器，可以移除
                    _pendingOperations.Add(new AddRemoveOperation(false, handler, order));
                }
                else
                {
                    //此前没有同类的待处理操作，且列表中不存在该处理器，无法移除
                    result = false;
                }
            }
            else
            {
                var existingOp = _pendingOperations[existingIndex];
                if (existingOp.IsAdd)
                {
                    //最近的是添加操作，可以移除
                    _pendingOperations.Add(new AddRemoveOperation(false, handler, order));
                }
                else
                {
                    //最近的是移除操作，不能重复移除
                    result = false;
                }
            }
#if PB_TEST_LOG || UNITY_EDITOR
            Log.Warning($"处于遍历中，尝试添加AddRemoveOperation: IsAdd={false}, Handler={handler}, Order={order} QueueSuccess={result}，此操作性能消耗较高，建议避免在遍历过程中频繁进行", nameof(EventHandlerCollection<T>), Log.PBBoxLoggerName);
#endif
            return result;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindLastPendingOperation(bool? isAdd, T handler, int order)
        {
            return _pendingOperations.FindLastIndex(op => (!isAdd.HasValue || op.IsAdd == isAdd) && EqualityComparer<T>.Default.Equals(op.Handler, handler) && op.Order == order);
        }

        public bool Add(T handler, int order = 0)
        {
            if (!_subscriptions.TryGetValue(order, out var list))
            {
                if (_isIterating)
                {
                    return TryQueueAddOperation(handler, order, list);
                }
                if (_inBucketListCapacity.HasValue)
                {
                    list = new IndexedList<T>(_keepInBucketItemOrderly, _inBucketListCapacity.Value);
                }
                else
                {
                    list = new IndexedList<T>(_keepInBucketItemOrderly);
                }
                _subscriptions.Add(order, list);
            }
            if(_isIterating && order == _currentIteratingOrder)
            {
                return TryQueueAddOperation(handler, order, list);
            }
            if (list.Add(handler))
            {
                Count++;
                return true;
            }
            return false;
        }

        public bool Remove(T handler, int order = 0)
        {
            if (_subscriptions.TryGetValue(order, out var list))
            {
                if (_isIterating && order == _currentIteratingOrder)
                {
                    return TryQueueRemoveOperation(handler, order, list);
                }
                if (list.Remove(handler))
                {
                    if (list.Count == 0 && _subscriptions.Count > MaxCachedSubscriptionsBucket)
                    {
                        _subscriptions.Remove(order);
//#if UNITY_EDITOR || PB_TEST_LOG
//                        Log.Warning($"因超出最大缓存订阅桶数量({MAX_CACHED_SUBSCRIPTIONS_BUCKET})而移除订阅桶 {order}，当前订阅桶数量为 {_subscriptions.Count}，请注意开销", nameof(EventHandlerCollection<T>), Log.PBBoxLoggerName);
//#endif
                    }
                    Count--;
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            _subscriptions.Clear();
            Count = 0;
            _pendingOperations.Clear();
        }

        public SortedDictionary<int, IndexedList<T>>.Enumerator GetEnumerator()
        {
            return _subscriptions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<int, IndexedList<T>>> IEnumerable<KeyValuePair<int, IndexedList<T>>>.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public void ForEachIn<TValue>(in TValue eventData, ForeachHandleIn<TValue> handle)
        {

        }

        public void ForEach(Action<List<T>> handle)
        {
            foreach (var kvp in _subscriptions)
            {
                SetIterating(true, kvp.Key);
                handle(kvp.Value.RawItems);
            }
            SetIterating(false, 0);
            ApplyPendingOperations();
        }

        public void ForEach<TValue>(TValue eventData, Action<TValue, List<T>> handle)
        {
            
        }


        [StructLayout(LayoutKind.Auto)]
        private struct AddRemoveOperation
        {
            public bool IsAdd;
            public T Handler;
            public int Order;

            public AddRemoveOperation(bool isAdd, T handler, int order)
            {
                IsAdd = isAdd;
                Handler = handler;
                Order = order;
            }
        }
    }
}