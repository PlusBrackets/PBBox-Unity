/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.07.18
 *@author: PlusBrackets
 --------------------------------------------------------*/
using PBBox;
using UnityEngine;
using System.Diagnostics;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PBBox.Attributes
{
    /// <summary>
    /// 在Scene视图中编辑Vector3/Vector2/Vector4/Vector3Int/Vector2Int
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class VectorEditOnSceneAttribute : PropertyAttribute
    {
        public VectorEditOnSceneAttribute() { }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(VectorEditOnSceneAttribute))]
        private class VectorEditOnSceneDrawer : PropertyDrawer
        {
            private SerializedProperty m_EditingProperty = null;
            private string m_EditingPropertyPath = null;
            private string m_EditingPropertyDisplayName = null;
            private Lazy<GUIStyle> m_EditingButtonStyle = new Lazy<GUIStyle>(
                () =>
                {
                    var _style = new GUIStyle(GUI.skin.button);
                    _style.fontSize = 10;
                    return _style;
                }
            );

            ~VectorEditOnSceneDrawer()
            {
                StopEdit();
                //Log.Debug("VectorEditOnSceneDrawer Destroy "+ testCC, "VectorEditOnScene", Log.PBBoxLoggerName);
            }

            private bool IsEditingPropertyEditable()
            {
                return IsEditableProperty(m_EditingProperty);
            }

            private bool IsEditableProperty(SerializedProperty property)
            {
                try
                {
                    if (property == null || property.serializedObject == null || property.serializedObject.targetObject == null)
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    //Log.Debug($"Property is not editable", "VectorEditOnScene", Log.PBBoxLoggerName);
                    return false;
                }
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Vector3:
                    case SerializedPropertyType.Vector2:
                    case SerializedPropertyType.Vector4:
                    case SerializedPropertyType.Vector3Int:
                    case SerializedPropertyType.Vector2Int:
                        return true;
                    default:
                        return false;
                }
            }


            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return base.GetPropertyHeight(property, label);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!IsEditableProperty(property))
                {
                    EditorGUI.PropertyField(position, property, label);
                    return;
                }

                EditorGUI.BeginProperty(position, label, property);

                //考虑Indented的宽度
                // var _labelWidth = EditorGUIUtility.labelWidth;    

                var _labelWidth = EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 12f;
                Rect _labelRect = new Rect(position.x, position.y, _labelWidth, position.height);
                EditorGUI.PrefixLabel(_labelRect, label);

                Rect _valueRect = new Rect(position.x + _labelWidth, position.y, position.width - _labelWidth - 70f, position.height);
                EditorGUI.PropertyField(_valueRect, property, GUIContent.none);

                Rect _buttonRect = new Rect(position.x + position.width - 65f, position.y, 45f, position.height);
                if (IsSameProperty(property))
                {
                    var _color = GUI.color;
                    GUI.color = Color.green;
                    //绘制绿色按钮
                    if (GUI.Button(_buttonRect, "Editing", m_EditingButtonStyle.Value))
                    {
                        GUI.color = _color;
                        StopEdit();
                    }
                    GUI.color = _color;
                }
                else
                {
                    if (GUI.Button(_buttonRect, "Edit"))
                    {
                        // Log.Debug("Start Edit Vector");
                        StartEdit(property);
                    }
                }

                Rect _focusRect = new Rect(position.x + position.width - 20f, position.y, 20f, position.height);
                if (GUI.Button(_focusRect, "F"))
                {
                    var _value = GetPropertyValue(property);
                    SceneView.lastActiveSceneView.pivot = _value;
                    SceneView.lastActiveSceneView.Repaint();
                }
                EditorGUI.EndProperty();
            }

            private void StartEdit(SerializedProperty property)
            {
                m_EditingProperty = property;
                m_EditingPropertyPath = property.propertyPath;
                m_EditingPropertyDisplayName = property.displayName;
                Log.Debug($"Start Edit: '{m_EditingPropertyDisplayName}', Path: '{m_EditingPropertyPath}'", property.serializedObject.targetObject, "VectorEditOnScene", Log.PBBoxLoggerName);
                Selection.selectionChanged -= OnSelectionChanged;
                Selection.selectionChanged += OnSelectionChanged;
                SceneView.duringSceneGui -= OnSceneGUI;
                SceneView.duringSceneGui += OnSceneGUI;
            }

            private void StopEdit()
            {
                if (m_EditingProperty != null)
                {
                    Log.Debug($"Stop Edit: '{m_EditingPropertyDisplayName}', Path: '{m_EditingPropertyPath}'", "VectorEditOnScene", Log.PBBoxLoggerName);
                    m_EditingProperty = null;
                    m_EditingPropertyPath = null;
                    m_EditingPropertyDisplayName = null;
                }
                SceneView.duringSceneGui -= OnSceneGUI;
                Selection.selectionChanged -= OnSelectionChanged;
            }

            private void OnSelectionChanged()
            {
                StopEdit();
            }

            private void OnSceneGUI(SceneView view)
            {
                if (!IsEditingPropertyEditable())
                {
                    StopEdit();
                    return;
                }
                var _value = GetPropertyValue(m_EditingProperty);
                Handles.Label(_value + Vector3.up * 0.05f, m_EditingProperty.displayName);
                EditorGUI.BeginChangeCheck();
                _value = Handles.PositionHandle(_value, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    SetPropertyValue(m_EditingProperty, _value);
                }
            }

            private bool IsSameProperty(SerializedProperty property)
            {
                return IsEditingPropertyEditable() && property.serializedObject != null && property.serializedObject.targetObject == m_EditingProperty.serializedObject.targetObject && property.propertyPath == m_EditingProperty.propertyPath;
            }

            private Vector3 GetPropertyValue(SerializedProperty property)
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Vector3:
                        return property.vector3Value;
                    case SerializedPropertyType.Vector2:
                        return property.vector2Value;
                    case SerializedPropertyType.Vector4:
                        return property.vector4Value;
                    case SerializedPropertyType.Vector3Int:
                        return property.vector3IntValue;
                    case SerializedPropertyType.Vector2Int:
                        return new Vector3(property.vector2IntValue.x, property.vector2IntValue.y, 0f);
                    default:
                        throw new Log.FetalErrorException($"Not Support Property Type, Property:{property.displayName}", "VectorEditOnScene", Log.PBBoxLoggerName);
                }
            }

            private void SetPropertyValue(SerializedProperty property, Vector3 value)
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Vector3:
                        property.vector3Value = value;
                        break;
                    case SerializedPropertyType.Vector2:
                        property.vector2Value = value;
                        break;
                    case SerializedPropertyType.Vector4:
                        property.vector4Value = value;
                        break;
                    case SerializedPropertyType.Vector3Int:
                        property.vector3IntValue = new Vector3Int((int)value.x, (int)value.y, (int)value.z);
                        break;
                    case SerializedPropertyType.Vector2Int:
                        property.vector2IntValue = new Vector2Int((int)value.x, (int)value.y);
                        break;
                    default:
                        throw new Log.FetalErrorException($"Not Support Property Type, Property:{property.displayName}", "VectorEditOnScene", Log.PBBoxLoggerName);
                }
                property.serializedObject.ApplyModifiedProperties();
            }

        }
#endif
    }
}