using System;

namespace PBBox.Properties
{
    /// <summary>
    /// 通用值持有者接口
    /// </summary>
    public interface IValueHolder
    {
        /// <summary>
        /// 获取值的类型
        /// </summary>
        Type ValueType { get; }
        /// <summary>
        /// 尝试获取值，若类型匹配则返回true并输出值，否则返回false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet<T>(out T value);
        bool TrySet<T>(T value);
    }

    /// <summary>
    /// 通用变量，可用于防止装箱
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InlineProperty]
#endif
    [Serializable]
    public sealed class ValueHolder<T> : IValueHolder, IReferencePoolItem
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideLabel]
#endif
#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        private T _value = default(T);
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public Type ValueType => typeof(T);
        bool IReferencePoolItem.IsUsing { get; set; }

        public ValueHolder() { }

        public ValueHolder(T value)
        {
            _value = value;
        }

        public bool TryGet<TValue>(out TValue value)
        {
            if (_value is TValue castValue)
            {
                value = castValue;
                return true;
            }
            value = default;
            return false;
        }

        public bool TrySet<TValue>(TValue value)
        {
            if (value is T castValue)
            {
                _value = castValue;
                return true;
            }
            return false;
        }

        void IReferencePoolItem.OnReferenceAcquire() { }

        void IReferencePoolItem.OnReferenceRelease()
        {
            _value = default(T);
        }

        public override string ToString()
        {
            return _value?.ToString() ?? "null";
        }
    }
}