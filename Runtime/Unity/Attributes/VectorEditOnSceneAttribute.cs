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
            private bool m_IsEditing => m_EditingProperty != null;
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
                Log.Debug("VectorEditOnSceneDrawer Destroy", "VectorEditOnScene", Log.PBBoxLoggerName);
                SceneView.duringSceneGui -= OnSceneGUI;
                Selection.selectionChanged -= OnSelectionChanged;
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
                        Log.Debug($"Stop Edit {property.displayName}", property.serializedObject.targetObject, "VectorEditOnScene", Log.PBBoxLoggerName);
                        m_EditingProperty = null;
                        SceneView.duringSceneGui -= OnSceneGUI;
                        Selection.selectionChanged -= OnSelectionChanged;
                    }
                    GUI.color = _color;
                }
                else
                {
                    if (GUI.Button(_buttonRect, "Edit"))
                    {
                        // Log.Debug("Start Edit Vector");
                        Log.Debug($"Start Edit {property.displayName}", property.serializedObject.targetObject, "VectorEditOnScene", Log.PBBoxLoggerName);
                        m_EditingProperty = property;
                        Selection.selectionChanged -= OnSelectionChanged;
                        Selection.selectionChanged += OnSelectionChanged;
                        SceneView.duringSceneGui -= OnSceneGUI;
                        SceneView.duringSceneGui += OnSceneGUI;
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

            private void OnSelectionChanged()
            {
                if (m_IsEditing && Selection.activeObject != null && m_EditingProperty.serializedObject != null && Selection.activeObject == m_EditingProperty.serializedObject.targetObject)
                {
                    // Log.Debug("Is Same Object");
                    return;
                }
                Log.Debug($"Stop Edit", "VectorEditOnScene", Log.PBBoxLoggerName);
                m_EditingProperty = null;
                SceneView.duringSceneGui -= OnSceneGUI;
                Selection.selectionChanged -= OnSelectionChanged;
                // Log.Debug("Stop Vector Scene Edit");
            }

            private void OnSceneGUI(SceneView view)
            {
                if (!m_IsEditing)
                {
                    UnityEditor.SceneView.duringSceneGui -= OnSceneGUI;
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
                return m_EditingProperty != null && property.serializedObject != null && m_EditingProperty.serializedObject != null && property.serializedObject.targetObject == m_EditingProperty.serializedObject.targetObject && property.propertyPath == m_EditingProperty.propertyPath;
            }

            private bool IsEditableProperty(SerializedProperty property)
            {
                if (property == null || property.serializedObject == null || property.serializedObject.targetObject == null)
                {
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

            private Vector3 GetPropertyValue(SerializedProperty property)
            {
                if (property == null || property.serializedObject == null || property.serializedObject.targetObject == null)
                {
                    return Vector3.zero;
                }
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
                        throw new Log.FetalErrorException("Not Support Property Type", "VectorEditOnScene", Log.PBBoxLoggerName);
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
                        throw new Log.FetalErrorException("Not Support Property Type", "VectorEditOnScene", Log.PBBoxLoggerName);
                }
                property.serializedObject.ApplyModifiedProperties();
            }

        }
#endif
    }
}