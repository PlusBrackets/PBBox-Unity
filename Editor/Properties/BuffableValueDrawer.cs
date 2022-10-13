/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.04.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using UnityEngine;
using UnityEditor;
using PBBox.Variables;

namespace PBBox.CEditor
{ 
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
}