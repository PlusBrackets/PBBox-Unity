/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace PBBox
{
    /// <summary>
    /// 事件分发器
    /// </summary>
    public partial class PBEvents
    {
        #region class
        internal class MyDelegate : System.IComparable<MyDelegate>, System.IComparable
        {
            public int Order { get; private set; }
            public Delegate Delegate { get; private set; }
            public bool IsOnce { get; private set; }
            public bool IsRemovable { get; set; } = false;

            public MyDelegate(Delegate _delegate, int order, bool isOnce)
            {
                this.Order = order;
                this.Delegate = _delegate;
                this.IsOnce = isOnce;
            }

            public int CompareTo(MyDelegate other)
            {
                if (other == null)
                    return -1;
                return Order - other.Order;
            }

            public int CompareTo(object other)
            {
                return CompareTo(other as MyDelegate);
            }
        }

        internal class MyDelegateArray
        {
            private Lazy<List<MyDelegate>> _temp = new Lazy<List<MyDelegate>>();
            private Lazy<HashSet<int>> _removeIndex = new Lazy<HashSet<int>>();
            private List<MyDelegate> _delegates { get; set; } = new List<MyDelegate>();
            private bool _isEmitting = false;
            private bool _shouldSort = false;

            public void HoldDelegates()
            {
                _isEmitting = true;
                if (_shouldSort) _delegates.Sort();
                _shouldSort = false;
            }

            public void ReleaseDelegates()
            {
                _isEmitting = false;
                RemoveAllRemovable();
            }

            private void QuickCheckShouldSort()
            {
                var _d = _delegates;
                if (_d.Count == 0 || _shouldSort)
                    return;
                if (_d.Count >= 2 && _d[_d.Count - 1].Order < _d[_d.Count - 2].Order)
                {
                    _shouldSort = true;
                }
                else if (_temp.IsValueCreated && _temp.Value.Count > 0)
                {
                    if (_temp.Value[0].Order < _d[_d.Count - 1].Order)
                    {
                        _shouldSort = true;
                    }
                    else if (_temp.Value.Count >= 2 && _temp.Value[_temp.Value.Count - 1].Order < _temp.Value[_temp.Value.Count - 2].Order)
                    {
                        _shouldSort = true;
                    }
                }
            }

            private void RemoveAllRemovable()
            {
                if (_removeIndex.IsValueCreated)
                {
                    foreach (int i in _removeIndex.Value)
                    {
                        _delegates.RemoveAt(i);
                    }
                    _removeIndex.Value.Clear();
                }
            }

            public void AddDelegate(MyDelegate myDelegate, bool checkSort)
            {
                if (_isEmitting)
                {
                    _temp.Value.Add(myDelegate);
                }
                else
                {
                    _delegates.Add(myDelegate);
                }
                if (checkSort)
                {
                    QuickCheckShouldSort();
                }
            }

            public bool RemoveDelegate(Delegate @delegate, int? order)
            {
                int index = _delegates.FindLastIndex(d => Check(d, @delegate, order));
                if (index < 0)
                {
                    if (!_temp.IsValueCreated || _temp.Value.Count == 0)
                        return false;
                    index = _temp.Value.FindLastIndex(d => Check(d, @delegate, order));
                    if (index < 0)
                        return false;
                    _temp.Value.RemoveAt(index);
                    return true;
                }

                if (_isEmitting)
                {
                    _removeIndex.Value.Add(index);
                    _delegates[index].IsRemovable = true;
                }
                else
                {
                    _delegates.RemoveAt(index);
                }
                return true;

                static bool Check(MyDelegate target, Delegate @delegate, int? order)
                {
                    if (target.Delegate == @delegate && (!order.HasValue || order.Value == target.Order))
                    {
                        return true;
                    }
                    return false;
                }
            }

            public void TravelForInvoke(Func<Delegate, bool> func)
            {
                for (int i = 0; i < _delegates.Count; i++)
                {
                    var d = _delegates[i];
                    if (!d.IsRemovable)
                    {
                        bool invoked = func.Invoke(d.Delegate);
                        if (invoked && d.IsOnce)
                            d.IsRemovable = true;
                    }
                    if (d.IsRemovable)
                    {
                        _removeIndex.Value.Add(i);
                    }
                }
            }
        }

        internal class MyDelegateCollection
        {
            public MyDelegateArray prefixDelegates;
            public MyDelegateArray delegates;
            public MyDelegateArray suffixDelegates;

            public void AddDelegate(Delegate listener, int order, bool isOnce)
            {
                MyDelegate md = new MyDelegate(listener, order, isOnce);
                if (order < 0)
                {
                    if (prefixDelegates == null) prefixDelegates = new MyDelegateArray();
                    prefixDelegates.AddDelegate(md, true);
                }
                else if (order > 0)
                {
                    if (suffixDelegates == null) suffixDelegates = new MyDelegateArray();
                    suffixDelegates.AddDelegate(md, true);
                }
                else
                {
                    if (delegates == null) delegates = new MyDelegateArray();
                    delegates.AddDelegate(md, false);
                }
            }

            public void RemoveDelegate(Delegate listener, int? order)
            {
                if (order.HasValue)
                {
                    if (order.Value < 0)
                    {
                        prefixDelegates?.RemoveDelegate(listener, order);
                    }
                    else if (order.Value > 0)
                    {
                        suffixDelegates?.RemoveDelegate(listener, order);
                    }
                    else
                    {
                        delegates?.RemoveDelegate(listener, order);
                    }
                }
                else
                {
                    bool goNext = true;
                    if (suffixDelegates != null)
                        goNext = !suffixDelegates.RemoveDelegate(listener, null);
                    if (goNext && delegates != null)
                        goNext = !delegates.RemoveDelegate(listener, null);
                    if (goNext && prefixDelegates != null)
                        goNext = !delegates.RemoveDelegate(listener, null);
                }
            }

            public void TravelForInvoke(Func<Delegate, bool> func)
            {
                prefixDelegates?.HoldDelegates();
                delegates?.HoldDelegates();
                suffixDelegates?.HoldDelegates();

                prefixDelegates?.TravelForInvoke(func);
                delegates?.TravelForInvoke(func);
                suffixDelegates?.TravelForInvoke(func);

                prefixDelegates?.ReleaseDelegates();
                delegates?.ReleaseDelegates();
                suffixDelegates?.ReleaseDelegates();
            }

        }
        #endregion

    }

}