/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.03.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using PBBox.Event;

namespace PBBox.Properties
{
    /// <summary>
    /// 可绑定的属性
    /// </summary>
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InlineProperty]
#endif
    [Serializable]
    public sealed class BindableValue<T> : IReferencePoolItem, IEventSubject, IEventPublisher, IValueHolder
    {

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideLabel]
#endif
#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        private T _value = default(T);

        /// <summary>
        /// 值，设置时若不相等会触发OnValueChanged事件
        /// </summary>
        /// <value></value>
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    T _oldValue = _value;
                    _value = value;
                    InvokeEventImpl(_value, _oldValue);
                }
            }
        }
        
        private Action _onChangedNoParam;
        private Action<(T newValue, T oldValue)> _onChanged;
        private Action<(BindableValue<T> property, T oldValue)> _onChangedWithSelf;

        bool IReferencePoolItem.IsUsing { get; set; }
        public Type ValueType => typeof(T);


        public BindableValue() { }

        public BindableValue(T value)
        {
            _value = value;
        }

        private void InvokeEventImpl(T newValue, T oldValue)
        {
            _onChanged?.Invoke((newValue, oldValue));
            _onChangedWithSelf?.Invoke((this, oldValue));
            _onChangedNoParam?.Invoke();
        }

        void IEventPublisher.Publish()
        {
            InvokeEventImpl(_value, _value);
        }

        /// <summary>
        /// 设置Value，不触发改变事件
        /// </summary>
        /// <param name="value"></param>
        public void SetValueNoNotify(T value)
        {
            _value = value;
        }

        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        /// <param name="onValueChanged"></param>
        /// <returns></returns>
        public EventSubscription Subscribe(Action<(T newValue, T oldValue)> onValueChanged)
        {
            _onChanged += onValueChanged;
            return new EventSubscription(this, onValueChanged, 0);
        }

        /// <summary>
        /// 订阅值改变事件，带自身引用
        /// </summary>
        /// <param name="onValueChanged"></param>
        /// <returns></returns>
        public EventSubscription Subscribe(Action<(BindableValue<T> property, T oldValue)> onValueChanged)
        {
            _onChangedWithSelf += onValueChanged;
            return new EventSubscription(this, onValueChanged, 0);
        }

        /// <summary>
        /// 订阅值改变事件，无参数
        /// </summary>
        /// <param name="onValueChanged"></param>
        /// <returns></returns>
        public EventSubscription Subscribe(Action onValueChanged)
        {
            _onChangedNoParam += onValueChanged;
            return new EventSubscription(this, onValueChanged, 0);
        }

        /// <summary>
        /// 取消订阅值改变事件，带自身引用
        /// </summary>
        /// <param name="onValueChanged"></param>
        public void Unsubscribe(Action<(T newValue, T oldValue)> onValueChanged)
        {
            _onChanged -= onValueChanged;
        }

        /// <summary>
        /// 取消订阅值改变事件，带自身引用
        /// </summary>
        /// <param name="onValueChanged"></param>
        public void Unsubscribe(Action<(BindableValue<T> property, T oldValue)> onValueChanged)
        {
            _onChangedWithSelf -= onValueChanged;
        }

        /// <summary>
        /// 取消订阅值改变事件，无参数
        /// </summary>
        /// <param name="onValueChanged"></param>
        public void Unsubscribe(Action onValueChanged)
        {
            _onChangedNoParam -= onValueChanged;
        }
        
        EventSubscription IEventSubject.Subscribe(Delegate handler, int order)
        {
            if (handler is Action<(T newValue, T oldValue)> onChanged)
            {
                return Subscribe(onChanged);
            }
            if (handler is Action<(BindableValue<T> property, T oldValue)> onChangedWithSelf)
            {
                return Subscribe(onChangedWithSelf);
            }
            if (handler is Action onChangedNoParam)
            {
                return Subscribe(onChangedNoParam);
            }
            throw new Log.FetalErrorException("Handler type not supported.", $"BindableProperty<{nameof(T)}>", Log.PBBoxLoggerName);
        }

        void IEventSubject.Unsubscribe(Delegate handler, int order)
        {
            if (handler is Action<(T newValue, T oldValue)> onChanged)
            {
                Unsubscribe(onChanged);
                return;
            }
            if (handler is Action<(BindableValue<T> property, T oldValue)> onChangedWithSelf)
            {
                Unsubscribe(onChangedWithSelf);
                return;
            }
            if (handler is Action onChangedNoParam)
            {
                Unsubscribe(onChangedNoParam);
                return;
            }
        }
        

        void IReferencePoolItem.OnReferenceAcquire() { }

        void IReferencePoolItem.OnReferenceRelease()
        {
            _value = default;
            _onChangedNoParam = null;
            _onChanged = null;
            _onChangedWithSelf = null;
        }

        bool IValueHolder.TryGet<TValue>(out TValue value)
        {
            if (_value is TValue castValue)
            {
                value = castValue;
                return true;
            }
            value = default;
            return false;
        }

        bool IValueHolder.TrySet<TValue>(TValue value)
        {
            if (value is T castValue)
            {
                Value = castValue;
                return true;
            }
            return false;
        }

        public static bool operator ==(BindableValue<T> left, T right)
        {
            if (left == null)
            {
                return false;
            }
            return EqualityComparer<T>.Default.Equals(left.Value, right);
        }

        public static bool operator !=(BindableValue<T> left, T right)
        {
            if (left == null)
            {
                return right == null;
            }
            return !EqualityComparer<T>.Default.Equals(left.Value, right);
        }

        public static bool operator ==(T left, BindableValue<T> right)
        {
            if (right == null)
            {
                return false;
            }
            return EqualityComparer<T>.Default.Equals(right.Value, left);
        }

        public static bool operator !=(T left, BindableValue<T> right)
        {
            if (right == null)
            {
                return left == null;
            }
            return !EqualityComparer<T>.Default.Equals(right.Value, left);
        }
        

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return _value?.ToString() ?? "null";
        }

    }
}