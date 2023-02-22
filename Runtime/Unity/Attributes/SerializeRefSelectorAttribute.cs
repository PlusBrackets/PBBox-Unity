/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.02.22
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PBBox.Unity
{
    /// <summary>
    /// 给[SerializeReference]特性提供一个类型选择器
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsageAttribute(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class SerializeRefSelectorAttribute : PropertyAttribute
    {
        private static readonly string[] DEFAULT_ASSEMBLIES = new string[] { "Assembly-CSharp", "PBBox" };
        private string[] m_FromAssemblies = DEFAULT_ASSEMBLIES;
        private Type m_FromType = null;
        private Type[] m_IncludeTypes = null;

        private Type[] m_SelectableTypes = null;
        private string[] m_SelectableTypeNames = null;
        public Type[] SelectableTypes
        {
            get
            {
                if (m_SelectableTypes != null && m_SelectableTypeNames != null)
                {
                    return m_SelectableTypes;
                }
                var _list = new HashSet<Type>();
                if (m_FromType != null)
                {
                    _list.UnionWith(m_FromType.GetReferencingClass(assemblyNames: m_FromAssemblies));
                }
                if (m_IncludeTypes != null)
                {
                    _list.UnionWith(m_IncludeTypes);
                }
                m_SelectableTypes = _list.ToArray();
                m_SelectableTypeNames = new string[] { "Null" }.Concat(m_SelectableTypes.Select(t => t.FullName).ToArray()).ToArray();
                return m_SelectableTypes;
            }
        }

        public string[] SelectableTypeNames
        {
            get
            {
                if (SelectableTypes != null)
                {
                    return m_SelectableTypeNames;
                }
                return null;
            }
        }

        public SerializeRefSelectorAttribute(Type fromType, Type[] includeTypes = null, string[] fromAssemblies = null)
        {
            m_FromType = fromType;
            m_IncludeTypes = includeTypes;
            m_FromAssemblies = fromAssemblies ?? DEFAULT_ASSEMBLIES;
        }

        public SerializeRefSelectorAttribute(Type[] includeTypes, string[] fromAssemblies = null)
        {
            m_FromType = null;
            m_IncludeTypes = includeTypes;
            m_FromAssemblies = fromAssemblies ?? DEFAULT_ASSEMBLIES;
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(SerializeRefSelectorAttribute))]
        private sealed class Drawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.singleLineHeight + 3f * EditorGUIUtility.standardVerticalSpacing;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var _selector = attribute as SerializeRefSelectorAttribute;
                if (_selector == null)
                {
                    return;
                }
                float _padding = EditorGUIUtility.standardVerticalSpacing;
                float _lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                Rect _selectTypeRect = new Rect(position.x, position.y + _padding, position.width - _padding, _lineHeight);
                Rect _propertyRect = new Rect(position.x, _selectTypeRect.y + _lineHeight, position.width - _padding, position.height - _lineHeight - _padding);

                GUI.Box(position, GUIContent.none);
                EditorGUI.BeginProperty(position, label, property);

                int _selecting = GetSelectingIndex(_selector, property.managedReferenceValue);
                var _selectableTypeNames = _selector.SelectableTypeNames;
                if (_selecting < 0)
                {
                    _selectableTypeNames[0] = "[Not Include]" + property.managedReferenceValue.GetType();
                    _selecting = 0;
                }
                else
                {
                    _selectableTypeNames[0] = "Null";
                }
                int _newSelectIndex = EditorGUI.Popup(_selectTypeRect, _selecting, _selector.SelectableTypeNames);
                if (_selecting != _newSelectIndex)
                {
                    var _newType = GetSelectedType(_selector, _selector.SelectableTypeNames[_newSelectIndex]);
                    if (_newType == null)
                    {
                        property.managedReferenceValue = null;
                    }
                    else
                    {
                        var _object = Activator.CreateInstance(_newType);
                        if (property.managedReferenceValue != null)
                        {
                            var _tempJson = JsonUtility.ToJson(property.managedReferenceValue);
                            JsonUtility.FromJsonOverwrite(_tempJson, _object);
                        }
                        property.managedReferenceValue = _object;
                    }
                }

                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(_propertyRect, property, label, true);
                EditorGUI.indentLevel--;
                EditorGUI.EndProperty();
            }

            private int GetSelectingIndex(SerializeRefSelectorAttribute _selector, object curValue)
            {
                if (curValue == null)
                {
                    return 0;
                }
                int _index = Array.IndexOf(_selector.SelectableTypes, curValue.GetType());
                if (_index < 0)
                {
                    //TODO 类型不匹配的补救措施
                    return -1;
                }
                return _index + 1;
            }

            private Type GetSelectedType(SerializeRefSelectorAttribute _selector, string typeName)
            {
                if (typeName == "Null" || string.IsNullOrEmpty(typeName))
                {
                    return null;
                }
                else
                {
                    return _selector.SelectableTypes.First(t => t.FullName == typeName);
                }
            }
        }
#endif
    }
}