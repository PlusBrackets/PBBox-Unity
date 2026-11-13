/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.12
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PBBox.Collections
{
    /// <summary>
    /// <para>带索引优化的列表，当元素数量超过一定阈值时，会自动维护索引以加快查找速度和删除速度，同时支持有序和无序两种模式。</para>
    /// <para>平衡遍历，添加和删除操作以及查找的性能，自动优化前开销和List相近，开始优化后会提高内存占用(使用字典维护索引);</para>
    /// <para>当keepOrderly为true时，列表会保持添加顺序，删除操作会使用墓碑法延迟压缩。</para>
    /// <para>当keepOrderly为false时，删除操作会直接用最后一个元素覆盖被删除元素的位置。</para>
    /// <para>不能重复添加同一个元素，元素不能为null</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class IndexedList<T> :ICollection<T>, IEnumerable<T> where T : class
    {
        /// <summary>
        /// 列表Count超过该值时开始优化
        /// </summary>
        private const int OPTIMIZE_THRESHOLD = 48;
        /// <summary>
        /// 当列表中有效元素占比低于该值时进行压缩
        /// </summary>
        private const float COMPACT_THRESHOLD = 0.68f;

        private readonly List<T> _items;
        private Dictionary<T, int> _indexMap = null;
        private readonly bool _keepOrderly;

        /// <summary>
        /// 列表顺序是否保持Add的顺序
        /// </summary>
        public bool IsOrderly => _keepOrderly;
        /// <summary>
        /// 当前有效元素数量
        /// </summary>
        public int Count { get; private set; } = 0;
        public int Capacity => _items.Capacity;
        /// <summary>
        /// 列表的实际容量，包括已删除的元素
        /// </summary>
        public int RawCount => _items.Count;
        /// <summary>
        /// 原始列表，包含所有元素，包括已删除的元素(已设置为null)，不要对它进行修改。使用 RawItems 进行for循环遍历可提高遍历性能
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public List<T> RawItems => _items;

        bool ICollection<T>.IsReadOnly => false;

        public IndexedList(bool keepOrderly = true)
        {
            _items = new List<T>();
            _keepOrderly = keepOrderly;
        }

        public IndexedList(bool keepOrderly, int capacity)
        {
            _items = new List<T>(capacity);
            _keepOrderly = keepOrderly;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TryInitOptimize()
        {
            if (_items.Count >= OPTIMIZE_THRESHOLD && _indexMap == null)
            {
                _indexMap = new Dictionary<T, int>(_items.Count);
                for (int i = 0; i < _items.Count; i++)
                {
                    _indexMap[_items[i]] = i;
                }
            }
        }

        /// <summary>
        /// 压缩列表，移除已删除的元素并更新索引映射
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Compact()
        {
            if (_indexMap == null || _items.Count == 0)
            {
                return;
            }
            int newIndex = 0;
            var count = _items.Count;
            for (int i = 0; i < count; i++)
            {
                if (_items[i] != null)
                {
                    if (i != newIndex)
                    {
                        _items[newIndex] = _items[i];
                        _indexMap[_items[i]] = newIndex;
                    }
                    newIndex++;
                }
            }
            _items.RemoveRange(newIndex, _items.Count - newIndex);
        }

        /// <summary>
        /// 尝试添加元素到列表，如果元素为null或已存在则返回false，否则返回true
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryAdd(T item)
        {
            if (item == null)
            {
                return false;
            }
            if (_indexMap != null)
            {
                //已开始优化，使用索引映射添加
                if (_indexMap.TryAdd(item, _items.Count))
                {
                    _items.Add(item);
                    Count++;
                    return true;
                }
            }
            else
            {
                //未开始优化，使用列表检查添加
                if (!_items.Contains(item))
                {
                    _items.Add(item);
                    Count++;
                    TryInitOptimize();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 尝试从列表中移除元素
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryRemove(T item)
        {
            if (item == null)
            {
                return false;
            }
            if (_indexMap != null)
            {
                //已开始优化，使用索引映射快速删除
                if (_indexMap.Remove(item, out int index))
                {
                    RemoveListImpl(index);
                    return true;
                }
            }
            else
            {
                //未开始优化，使用列表检查删除
                if (_items.Remove(item))
                {
                    Count--;
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveListImpl(int index)
        {
            if (_keepOrderly)
            {
                //使用墓碑法
                _items[index] = null;
                Count--;
                //当当前有效元素占比低于一定阈值时，进行压缩
                if (Count / (float)_items.Count < COMPACT_THRESHOLD)
                {
                    Compact();
                }
            }
            else
            {
                //不保持顺序，直接用最后一个元素覆盖被删除元素的位置
                int lastIndex = _items.Count - 1;
                if (index != lastIndex)
                {
                    T lastItem = _items[lastIndex];
                    _items[index] = lastItem;
                    _indexMap[lastItem] = index;
                }
                _items.RemoveAt(lastIndex);
                Count--;
            }
        }

        public bool Contains(T item)
        {
            if (item == null)
            {
                return false;
            }
            if (_indexMap != null)
            {
                return _indexMap.ContainsKey(item);
            }
            else
            {
                return _items.Contains(item);
            }
        }

        public void Clear()
        {
            _items.Clear();
            _indexMap?.Clear();
            _indexMap = null;
            Count = 0;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_items);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        void ICollection<T>.Add(T item)
        {
            if (!TryAdd(item))
            {
                Log.Error($"Failed to add item: {item}, it may already exist in the list.", nameof(IndexedList<T>), Log.PBBoxLoggerName);
            }
        }

        bool ICollection<T>.Remove(T item) => TryRemove(item);

        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            public T Current => _curValue;
            object IEnumerator.Current => _curValue;
            private T _curValue;
            private int _curIndex;
            private List<T> _items;
            private int _count;

            public Enumerator(List<T> items)
            {
                _items = items;
                _curIndex = 0;
                _curValue = null;
                _count = items.Count;
            }

            public void Dispose()
            {
                _curValue = null;
                _curIndex = 0;
                _items = null;
            }

            public bool MoveNext()
            {
                if (_curIndex < _count)
                {
                    _curValue = _items[_curIndex];
                    _curIndex++;
                    if(_curValue == null)
                    {
                        return MoveNext();
                    }
                    return true;
                }
                return false;
                //_curValue = NextVaildIitem();
                //return _curValue != null;
                
            }

            public void Reset()
            {
                _curIndex = 0;
                _curValue = null;
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            //private T NextVaildIitem()
            //{
            //    _curIndex++;
            //    while (_curIndex < _items.Count)
            //    {
            //        if (_items[_curIndex] != null)
            //        {
            //            return _items[_curIndex];
            //        }
            //        _curIndex++;
            //    }
            //    return null;
            //}
        }
    }
}