using System;

namespace PBBox.Properties
{
    /// <summary>
    /// 通用变量
    /// </summary>
    public abstract class GenericVariable : IReferencePoolItem
    {
        bool IReferencePoolItem.IsUsing { get; set; }

        void IReferencePoolItem.OnReferenceAcquire() { }

        void IReferencePoolItem.OnReferenceRelease()
        {
            OnRelease();
        }

        public abstract object GetValue();

        protected abstract void OnRelease();
    }

    /// <summary>
    /// 通用变量，通常用于防止装箱
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class GenericVariable<T> : GenericVariable
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideLabel]
#endif
#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        private T m_Value = default(T);
        public T Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }

        public GenericVariable()
        {

        }

        public GenericVariable(T value)
        {
            m_Value = value;
        }

        public override object GetValue()
        {
            return m_Value;
        }

        protected override void OnRelease()
        {
            m_Value = default(T);
        }
    }
}