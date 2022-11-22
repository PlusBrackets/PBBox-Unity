using System.Collections.Generic;
/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
#if USE_ODIN
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using System;

namespace PBBox
{
#if USE_ODIN
    [InlineProperty]
#endif
    [Serializable]
    /// <summary>
    /// 可绑定的属性
    /// </summary>
    public class BindableValue<T>
    {
        private Action<T, T> m_OnValueChangedWithOldValue;
        private Action<T> m_OnValueChanged;
#if USE_ODIN
        [HideLabel]
#endif
        [SerializeField]
        private T m_Value = default(T);
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
                    OnValueChanged(_oldValue, m_Value);
                }
            }
        }

        public BindableValue() { }

        public BindableValue(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// 手动调用OnChanged
        /// </summary>
        public void CallChangedManual()
        {
            OnValueChanged(m_Value, m_Value);
        }

        /// <summary>
        /// 设置Value，不会触发onValueChanged
        /// </summary>
        /// <param name="value"></param>
        public void SetValueQuiet(T value)
        {
            m_Value = value;
        }

        private void OnValueChanged(T oldValue, T newValue)
        {
            if (m_OnValueChangedWithOldValue != null)
                m_OnValueChangedWithOldValue.Invoke(oldValue, newValue);
            if (m_OnValueChanged != null)
                m_OnValueChanged.Invoke(newValue);
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="action">(OldValue,NewValue)</param>
        public void Bind(Action<T, T> action, bool callBackImmediately = true)
        {
            if (callBackImmediately)
                action(Value, Value);
            m_OnValueChangedWithOldValue += action;
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="action"></param>
        public void Bind(Action<T> action, bool callBackImmediately = true)
        {
            if (callBackImmediately)
                action(Value);
            m_OnValueChanged += action;
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

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}