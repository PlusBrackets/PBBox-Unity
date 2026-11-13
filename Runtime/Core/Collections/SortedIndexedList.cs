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

namespace PBBox.Collections
{
    /// <summary>
    /// <para>有序的索引列表集合</para>
    /// <para>对遍历时操作进行了延迟处理优化，不在遍历中的List不受影响，会改变集合本身(新建list或removelist)的除外</para>
    /// <para>需要在遍历前后调用相应方法确保操作正常，或使用ForEach(handle)方法进行遍历</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortedIndexedList<T> : IEnumerable<KeyValuePair<int, IndexedList<T>>>, IEnumerable where T : Delegate
    {
        private const int MAX_CACHED_SUBSCRIPTION_BUCKET_DEFAULT = 8;
        private SortedDictionary<int, IndexedList<T>> _orderedBuckets = new SortedDictionary<int, IndexedList<T>>();
        private bool _keepInBucketItemOrderly = true;
        private int? _inBucketListCapacity;
        /// <summary>
        /// 最大缓存的订阅桶数量，超过该数量时会移除空的订阅桶
        /// </summary>
        public int MaxCachedSubscriptionsBucket { get; set; } = MAX_CACHED_SUBSCRIPTION_BUCKET_DEFAULT;
        /// <summary>
        /// 是否保持订阅桶内的项有序
        /// </summary>
        public bool KeepInBucketItemOrderly => _keepInBucketItemOrderly;

        public int Count { get; private set; }
        // 是否正在遍历集合，用于延迟处理对集合的修改
        private bool _isIterating = false;
        private int _currentIteratingOrder;
        private List<AddRemoveOperation> _pendingOperations;

        public SortedIndexedList()
        {
            _keepInBucketItemOrderly = true;
            _inBucketListCapacity = null;
        }

        public SortedIndexedList(bool keepInBucketItemOrderly = true)
        {
            _keepInBucketItemOrderly = keepInBucketItemOrderly;
            _inBucketListCapacity = null;
        }

        public SortedIndexedList(int inBucketListCapacity, bool keepInBucketItemOrderly = true)
        {
            _keepInBucketItemOrderly = keepInBucketItemOrderly;
            _inBucketListCapacity = inBucketListCapacity;
        }

        public static SortedIndexedList<T> Create(bool keepInBucketItemOrderly = true, int? inBucketListCapacity = null)
        {
            return inBucketListCapacity.HasValue
                ? new SortedIndexedList<T>(inBucketListCapacity.Value, keepInBucketItemOrderly)
                : new SortedIndexedList<T>(keepInBucketItemOrderly);
        }

        /// <summary>
        /// 设置集合的参数
        /// </summary>
        /// <param name="keepInBucketItemOrderly"></param>
        /// <param name="inBucketListCapacity"></param>
        /// <returns>False表示未能完全应用参数，存在已填充的集合</returns>
        public bool SetCollectionParameter(bool keepInBucketItemOrderly, int? inBucketListCapacity = null, int? maxCachedSubscriptionsBucket = null)
        {
            bool result = true;
            if (_orderedBuckets != null && _orderedBuckets.Count > 0)
            {
                //提示无法将参数应用到已存在的indexedList中，新建的集合将使用新的参数，最好是在集合为空时设置参数，或者Clear后设置
                Log.Warning("Cannot apply collection parameters to an already populated collection. But new collections will use the new parameters. It is recommended to set parameters when the collection is empty or after calling Clear.", nameof(SortedIndexedList<T>), Log.PBBoxLoggerName);
                result = false;
            }
            _keepInBucketItemOrderly = keepInBucketItemOrderly;
            _inBucketListCapacity = inBucketListCapacity;
            MaxCachedSubscriptionsBucket = maxCachedSubscriptionsBucket ?? MAX_CACHED_SUBSCRIPTION_BUCKET_DEFAULT;
            return result;
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
                Log.Error("Cannot apply pending operations while iterating.", nameof(SortedIndexedList<T>), Log.PBBoxLoggerName);
                return;
            }
            foreach (var op in _pendingOperations)
            {
                if (op.IsAdd)
                {
                    TryAdd(op.Handler, op.Order);
                }
                else
                {
                    TryRemove(op.Handler, op.Order);
                }
            }
            _pendingOperations.Clear();
        }

        /// <summary>
        /// <para>遍历当前集合中的所有桶，并对每个桶的原始IndexedList项目列表执行指定的操作</para>
        /// <para>在遍历过程中，对集合的修改会被延迟处理，直到遍历结束后应用</para>
        /// <para>根据列表是否保持有序的设置，传入的List<T>中的项目可能为null(已被移除但仍占据位置)</para>
        /// </summary>
        /// <param name="handle"></param>
        public void ForEach(Action<List<T>> handle)
        {
            foreach (var kvp in _orderedBuckets)
            {
                SetIterating(true, kvp.Key);
                handle(kvp.Value.RawItems);
            }
            SetIterating(false, 0);
            ApplyPendingOperations();
        }

        /// <summary>
        /// 遍历当前集合中的所有有效项
        /// </summary>
        /// <param name="handle"></param>
        public void ForEach(Action<T> handle)
        {
            foreach (var kvp in _orderedBuckets)
            {
                SetIterating(true, kvp.Key);
                var rawItems = kvp.Value.RawItems;
                var count = rawItems.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = rawItems[i];
                    if (item != null)
                    {
                        handle(item);
                    }
                }
            }
            SetIterating(false, 0);
            ApplyPendingOperations();
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
            Log.Warning($"处于遍历中，尝试添加AddRemoveOperation: IsAdd={true}, Handler={handler}, Order={order} QueueSuccess={result}，此操作性能消耗较高，建议避免在遍历过程中频繁进行", nameof(SortedIndexedList<T>), Log.PBBoxLoggerName);
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
            Log.Warning($"处于遍历中，尝试添加AddRemoveOperation: IsAdd={false}, Handler={handler}, Order={order} QueueSuccess={result}，此操作性能消耗较高，建议避免在遍历过程中频繁进行", nameof(SortedIndexedList<T>), Log.PBBoxLoggerName);
#endif
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindLastPendingOperation(bool? isAdd, T handler, int order)
        {
            return _pendingOperations.FindLastIndex(op => (!isAdd.HasValue || op.IsAdd == isAdd) && EqualityComparer<T>.Default.Equals(op.Handler, handler) && op.Order == order);
        }

        public bool TryAdd(T handler, int order = 0)
        {
            if (!_orderedBuckets.TryGetValue(order, out var list))
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
                _orderedBuckets.Add(order, list);
            }
            if (_isIterating && order == _currentIteratingOrder)
            {
                return TryQueueAddOperation(handler, order, list);
            }
            if (list.TryAdd(handler))
            {
                Count++;
                return true;
            }
            return false;
        }

        public bool TryRemove(T handler, int order = 0)
        {
            if (_orderedBuckets.TryGetValue(order, out var list))
            {
                if (_isIterating && order == _currentIteratingOrder)
                {
                    return TryQueueRemoveOperation(handler, order, list);
                }
                if (list.TryRemove(handler))
                {
                    if (list.Count == 0 && _orderedBuckets.Count > MaxCachedSubscriptionsBucket)
                    {
                        _orderedBuckets.Remove(order);
                    }
                    Count--;
                    return true;
                }
            }
            return false;
        }

        public bool Contains(T handler, int order = 0)
        {
            if (_orderedBuckets.TryGetValue(order, out var list))
            {
                return list.Contains(handler);
            }
            return false;
        }

        public void Clear()
        {
            _orderedBuckets.Clear();
            Count = 0;
            _pendingOperations.Clear();
        }

        public SortedDictionary<int, IndexedList<T>>.Enumerator GetEnumerator()
        {
            return _orderedBuckets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<int, IndexedList<T>>> IEnumerable<KeyValuePair<int, IndexedList<T>>>.GetEnumerator()
        {
            return GetEnumerator();
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