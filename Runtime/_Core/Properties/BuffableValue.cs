/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.04.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
#if USE_ODIN
using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PBBox
{
    /// <summary>
    /// BuffableValue中可以改变数值的类型
    /// </summary>
    public enum BuffType
    {   /// <summary>
        /// buff类型, value = Constant || ∏(Modifys) * ( base + ∑(Additons) + base * ∑(Multiply) )
        /// </summary>
        Addition = 0,
        /// <summary>
        /// buff类型, value = Constant || ∏(Modifys) * ( base + ∑(Additons) + base * ∑(Multiply) )
        /// </summary>
        Multiply = 1,
        /// <summary>
        /// buff类型, value = Constant || ∏(Modifys) * ( base + ∑(Additons) + base * ∑(Multiply) )
        /// </summary>
        Modify = 2,
        /// <summary>
        /// buff类型, value = Constant || ∏(Modifys) * ( base + ∑(Additons) + base * ∑(Multiply) )
        /// </summary>
        Constant = 3
    }
    /// <summary>
    /// 可buff值类, value = Constant || ∏(Modifys) * ( base + ∑(Additons) + base * ∑(Multiply) )
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public abstract class BuffableValue<T> : ISerializationCallbackReceiver where T : struct
    {
        [System.Serializable]
        protected struct ConstantBuff
        {
            public int priority;
            public T buff;
        }

        // protected T m_BuffValue;
        // [System.NonSerialized]
        // [HideInInspector, SerializeField]
        // private string buffedValueStr;
        [SerializeField]
        protected T m_BaseValue = default(T);
        /// <summary>
        /// 基础属性
        /// </summary>
        /// <value></value>
        public T baseValue
        {
            get
            {
                return m_BaseValue;
            }
            set
            {
                m_NoBuffChanged = m_BaseValue.Equals(value);
                m_BaseValue = value;
            }
        }
        /// <summary>
        /// addition类型buff总值
        /// </summary>
        /// <returns></returns>
        public T additionBuff { get; private set; } = default(T);
        /// <summary>
        /// multiply类型buff总值
        /// </summary>
        /// <returns></returns>
        public T multiplyBuff { get; private set; } = default(T);
        /// <summary>
        /// modify类型buff总值
        /// </summary>
        /// <returns></returns>
        public T modifyBuff { get; private set; } = default(T);
        /// <summary>
        /// constant类型当前值
        /// </summary>
        /// <returns></returns>
        public T constantBuff { get; private set; } = default(T);
        [SerializeField, HideInInspector]
        private Dictionary<string, T> m_Additions;
        [SerializeField, HideInInspector]
        private Dictionary<string, T> m_Multilys;
        [SerializeField, HideInInspector]
        private Dictionary<string, T> m_Modifys;
        [SerializeField, HideInInspector]
        private Dictionary<string, ConstantBuff> m_Constants;
        // private List<string> m_ConstantBuffPriority;
        [System.NonSerialized]
        private bool m_NoBuffChanged = false;
        [HideInInspector, SerializeField]
        private T m_BuffedValue = default(T);
        /// <summary>
        /// 计算后的值，value = Constant || ∏(Modifys) * ( base + ∑(Additons) + base * ∑(Multiply) )
        /// </summary>
        /// <value></value>
        public T value
        {
            get
            {
                UpdateBuffValue();
                return m_BuffedValue;
            }
        }

        public BuffableValue(T baseValue)
        {
            m_BaseValue = baseValue;
        }

        public BuffableValue()
        {
        }

        private Dictionary<string, T> GetBuffDict(BuffType type, bool createNew = false)
        {
            switch (type)
            {
                case BuffType.Addition:
                    if (createNew && m_Additions == null)
                        m_Additions = new Dictionary<string, T>();
                    return m_Additions;
                case BuffType.Multiply:
                    if (createNew && m_Multilys == null)
                        m_Multilys = new Dictionary<string, T>();
                    return m_Multilys;
                case BuffType.Modify:
                    if (createNew && m_Modifys == null)
                        m_Modifys = new Dictionary<string, T>();
                    return m_Modifys;
            }
            return null;
        }

        /// <summary>
        /// 添加一个buff
        /// </summary>
        /// <param name="key">buff的标识</param>
        /// <param name="type">buff类型</param>
        /// <param name="value">buff值</param>
        /// <param name="priority">优先度，constant类型使用，取最优先的应用，值越大优先度越高</param>
        public void AddBuff(string key, BuffType type, T value, int priority = 0)
        {
            if (BuffType.Constant == type)
            {
                if (m_Constants == null)
                    m_Constants = new Dictionary<string, ConstantBuff>();
                ConstantBuff buff = new ConstantBuff { priority = priority, buff = value };
                CommonUtils.AddToDictionary(m_Constants, key, buff);
            }
            else
            {
                var buffs = GetBuffDict(type, true);
                CommonUtils.AddToDictionary(buffs, key, value);
            }
            m_NoBuffChanged = false;
        }

        /// <summary>
        /// 移除buff
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        public void RemoveBuff(string key, BuffType type)
        {
            if (BuffType.Constant == type)
            {
                if (m_Constants == null)
                    return;
                if (m_Constants.ContainsKey(key))
                {
                    m_Constants.Remove(key);
                    m_NoBuffChanged = false;
                }
            }
            else
            {
                var buffs = GetBuffDict(type);
                if (buffs == null)
                    return;
                if (buffs.ContainsKey(key))
                {
                    buffs.Remove(key);
                    m_NoBuffChanged = false;
                }
            }
        }

        /// <summary>
        /// 清除buff
        /// </summary>
        /// <param name="type"></param>
        public void ClearBuff(BuffType type)
        {
            if (BuffType.Constant == type)
            {
                if (m_Constants == null || m_Constants.Count == 0)
                    return;
                m_Constants.Clear();
                m_NoBuffChanged = false;
            }
            else
            {
                var buffs = GetBuffDict(type);
                if (buffs == null || buffs.Count == 0)
                    return;
                buffs.Clear();
                m_NoBuffChanged = false;
            }

        }

        public bool ContainsBuff(string key, BuffType type)
        {
            if (type == BuffType.Constant)
                return m_Constants != null ? m_Constants.ContainsKey(key) : false;
            var buffs = GetBuffDict(type);
            return buffs != null ? buffs.ContainsKey(key) : false;
        }

        /// <summary>
        /// buff数量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int CountBuff(BuffType type)
        {
            if (type == BuffType.Constant)
                return m_Constants != null ? m_Constants.Count : 0;
            var buffs = GetBuffDict(type);
            return buffs != null ? buffs.Count : 0;
        }

        /// <summary>
        /// 更新buff数值
        /// </summary>
        public void UpdateBuffValue()
        {
            if (!m_NoBuffChanged)
            {
                m_NoBuffChanged = true;
                Dictionary<string, T> dict = null;
                dict = GetBuffDict(BuffType.Addition);
                additionBuff = dict != null ? ComputeAdditionBuff(dict) : default(T);

                dict = GetBuffDict(BuffType.Multiply);
                multiplyBuff = dict != null ? ComputeMultiplyBuff(dict) : default(T);

                dict = GetBuffDict(BuffType.Modify);
                modifyBuff = dict != null ? ComputeModifyBuff(dict) : default(T);

                constantBuff = m_Constants != null ? ComputeConstantBuff(m_Constants) : default(T);

                bool hasMod = m_Modifys != null && m_Modifys.Count > 0;
                bool hasCon = m_Constants != null && m_Constants.Count > 0;
                m_BuffedValue = CalculateBuffValue(additionBuff, multiplyBuff, modifyBuff, hasMod, constantBuff, hasCon);
                // #if UNITY_EDITOR
                //                 buffedValueStr = m_BuffedValue.ToString();
                // #endif
            }
        }

        protected virtual T ComputeAdditionBuff(Dictionary<string, T> dict)
        {
            T v = default(T);
            ICollection<T> values = dict.Values;
            foreach (T value in values)
            {
                v = add(v, value);
            }
            return v;
        }

        protected virtual T ComputeMultiplyBuff(Dictionary<string, T> dict)
        {
            return ComputeAdditionBuff(dict);
        }

        protected virtual T ComputeModifyBuff(Dictionary<string, T> dict)
        {
            int i = 0;
            T v = default(T);
            ICollection<T> values = dict.Values;
            foreach (T value in values)
            {
                if (i == 0)
                {
                    v = value;
                    i++;
                    continue;
                }
                v = mul(v, value);
            }
            return v;
        }

        protected virtual T ComputeConstantBuff(Dictionary<string, ConstantBuff> dict)
        {
            T v = default(T);
            ICollection<ConstantBuff> values = dict.Values;
            int p = int.MinValue;
            foreach (ConstantBuff b in values)
            {
                if (b.priority >= p)
                {
                    p = b.priority;
                    v = b.buff;
                }
            }
            return v;
        }

        protected virtual T CalculateBuffValue(T add, T mul, T mod, bool hasMod, T con, bool hasCon)
        {
            if (hasCon)
                return (T)con;
            var v1 = this.add(this.add(baseValue, this.mul(baseValue, mul)), add);
            if (hasMod)
                return this.mul(mod, v1);
            return (T)v1;
        }

        protected abstract T mul(T a, T b);

        protected abstract T add(T a, T b);

        public override string ToString()
        {
            return value.ToString();
        }

        #region Serialization
        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            m_NoBuffChanged = false;
        }

        #endregion

    }

#if USE_ODIN
    [DrawWithUnity]
#endif
    [System.Serializable]
    public class BuffableFloat : BuffableValue<float>
    {
        public BuffableFloat(float baseValue) : base(baseValue)
        {

        }

        public BuffableFloat() : base()
        {

        }

        protected override float mul(float a, float b)
        {
            return a * b;
        }

        protected override float add(float a, float b)
        {
            return a + b;
        }
    }

    #if UNITY_EDITOR
     [CustomPropertyDrawer(typeof(BuffableValue<>),true)]
    public class BuffableFloatDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty baseValueProp = property.FindPropertyRelative("m_BaseValue");
            // int fieldCount = 1;
            return EditorGUI.GetPropertyHeight(baseValueProp,label,true); //fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty baseValueProp = property.FindPropertyRelative("m_BaseValue");
            // SerializedProperty buffValueProp = property.FindPropertyRelative("buffedValueStr");
            SerializedProperty buffedValueProp = property.FindPropertyRelative("m_BuffedValue");

            Rect singleFiledRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);            
            if (EditorApplication.isPlaying)
            {
                Rect baseValueRect = new Rect(singleFiledRect);
                baseValueRect.width -= EditorGUIUtility.currentViewWidth / 4f;
                EditorGUI.PropertyField(baseValueRect, baseValueProp, label,true);

                Rect resultPropRect = new Rect(singleFiledRect);
                resultPropRect.width = position.width - baseValueRect.width - 2f; //EditorGUIUtility.currentViewWidth/4f - 2f;
                resultPropRect.x += baseValueRect.width + 2f;
                // var str = buffedValueProp.va;
                GUI.enabled = false;
                EditorGUI.PropertyField(resultPropRect, buffedValueProp, new GUIContent(""),true);
                GUI.enabled = true;
                // EditorGUI.LabelField(resultPropRect, String.IsNullOrEmpty(str)?"Not use":$"=>{str}", EditorStyles.textField);
            }
            else
            {
                EditorGUI.PropertyField(singleFiledRect, baseValueProp, label);
            }
            
        }
    }
    #endif
}