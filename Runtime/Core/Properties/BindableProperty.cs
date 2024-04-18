/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.03.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;

namespace PBBox.Properties
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InlineProperty]
#endif
    [Serializable]
    /// <summary>
    /// 可绑定的属性
    /// </summary>
    public sealed class BindableProperty<T> : IReferencePoolItem
    {

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideLabel]
#endif
#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        private T m_Value = default(T);

        /// <summary>
        /// 值，设置时若不相等会触发OnValueChanged事件
        /// </summary>
        /// <value></value>
        public T Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(m_Value, value))
                {
                    T _oldValue = m_Value;
                    m_Value = value;
                    InvokeEventImpl(_oldValue, m_Value);
                }
            }
        }
        /// <summary>
        /// 当值改变时
        /// </summary>
        private event Action<T> m_OnValueChanged;
        /// <summary>
        /// 当值改变时，Action(oldValue, newValue);
        /// </summary>
        private event Action<T, T> m_OnValueChangedWithOldValue;
        /// <summary>
        /// 当值改变时，Action(oldValue, newValue, sender);
        /// </summary>
        private event Action<T, T, BindableProperty<T>> m_OnValueChangedWithOldValueAndSender;

        bool IReferencePoolItem.IsUsing { get; set; }

        public BindableProperty() { }

        public BindableProperty(T value)
        {
            this.Value = value;
        }

        private void InvokeEventImpl(T oldValue, T newValue)
        {
            m_OnValueChangedWithOldValueAndSender?.Invoke(oldValue, newValue, this);
            m_OnValueChangedWithOldValue?.Invoke(oldValue, newValue);
            m_OnValueChanged?.Invoke(newValue);
        }

        /// <summary>
        /// 手动调用OnChanged
        /// </summary>
        public void InvokeValueChangedEvent()
        {
            InvokeEventImpl(m_Value, m_Value);
        }

        /// <summary>
        /// 设置Value，不会触发事件
        /// </summary>
        /// <param name="value"></param>
        public void SetValueQuiet(T value)
        {
            m_Value = value;
        }

        /// <summary>
        /// 绑定<oldVal, newVal, sender>
        /// </summary>
        /// <param name="action"><oldVal, newVal, sender></param>
        /// <param name="InvokeImmediately"></param>
        public void Bind(Action<T, T, BindableProperty<T>> action, bool InvokeImmediately = true)
        {
            m_OnValueChangedWithOldValueAndSender += action;
            if (InvokeImmediately)
            {
                action(Value, Value, this);
            }
        }

        /// <summary>
        /// 绑定<oldVal, newVal>
        /// </summary>
        /// <param name="action">(OldValue,NewValue)</param>
        public void Bind(Action<T, T> action, bool InvokeImmediately = true)
        {
            m_OnValueChangedWithOldValue += action;
            if (InvokeImmediately)
            {
                action(Value, Value);
            }
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="action"></param>
        public void Bind(Action<T> action, bool InvokeImmediately = true)
        {
            m_OnValueChanged += action;
            if (InvokeImmediately)
            {
                action(Value);
            }
        }

        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="action"></param>
        public void UnBind(Action<T, T, BindableProperty<T>> action)
        {
            m_OnValueChangedWithOldValueAndSender -= action;
        }

        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="action"></param>
        public void UnBind(Action<T, T> action)
        {
            m_OnValueChangedWithOldValue -= action;
        }

        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="action"></param>
        public void UnBind(Action<T> action)
        {
            m_OnValueChanged -= action;
        }

        void IReferencePoolItem.OnReferenceAcquire() { }

        void IReferencePoolItem.OnReferenceRelease()
        {
            m_Value = default;
            m_OnValueChanged = null;
            m_OnValueChangedWithOldValue = null;
        }

        public static bool operator ==(BindableProperty<T> left, T right)
        {
            if (left == null)
            {
                return false;
            }
            return EqualityComparer<T>.Default.Equals(left.Value, right);
        }

        public static bool operator !=(BindableProperty<T> left, T right)
        {
            if (left == null)
            {
                return right == null;
            }
            return !EqualityComparer<T>.Default.Equals(left.Value, right);
        }

        public static bool operator ==(T left, BindableProperty<T> right)
        {
            if (right == null)
            {
                return false;
            }
            return EqualityComparer<T>.Default.Equals(right.Value, left);
        }

        public static bool operator !=(T left, BindableProperty<T> right)
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
            return Value.ToString();
        }

    }
}