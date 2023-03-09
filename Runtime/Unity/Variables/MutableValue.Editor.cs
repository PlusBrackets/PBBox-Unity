using System.Collections;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


namespace PBBox.Variables
{
    public abstract partial class MutableValue<T> where T : struct, IEquatable<T>
    {
        protected abstract class Drawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                SerializedProperty baseValueProp = property.FindPropertyRelative("m_BaseValue");
                return EditorGUI.GetPropertyHeight(baseValueProp, label, true);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                SerializedProperty baseValueProp = property.FindPropertyRelative("m_BaseValue");

                EditorGUI.BeginProperty(position, label, property);
                Rect singleFiledRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                if (EditorApplication.isPlaying)
                {
                    Rect baseValueRect = new Rect(singleFiledRect);
                    baseValueRect.width -= EditorGUIUtility.currentViewWidth / 4f;
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(baseValueRect, baseValueProp, label, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        SetNeedRecompute(property);
                        property.serializedObject.ApplyModifiedProperties();
                    }

                    Rect resultPropRect = new Rect(singleFiledRect);
                    resultPropRect.width = position.width - baseValueRect.width - 2f;
                    resultPropRect.x += baseValueRect.width + 2f;

                    GUI.enabled = false;
                    EditorGUI.TextField(resultPropRect, GUIContent.none, GetModdedValue(property).ToString());
                    GUI.enabled = true;
                }
                else
                {
                    EditorGUI.PropertyField(singleFiledRect, baseValueProp, label);
                }
                EditorGUI.EndProperty();

            }

            private MutableValue<T> GetSelf(SerializedProperty property){
                var _type = property.serializedObject.targetObject.GetType();
                var _target = _type.GetField(property.propertyPath);
                var _self = _target.GetValue(property.serializedObject.targetObject) as MutableValue<T>;
                return _self;
            }

            private void SetNeedRecompute(SerializedProperty property)
            {
                GetSelf(property).m_NeedRecompute = true;
            }

            private T GetModdedValue(SerializedProperty property)
            {
                return GetSelf(property).Value;
            }
        }
    }
}
#endif