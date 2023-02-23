/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.02.22
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;

namespace PBBox.Unity
{
    /// <summary>
    /// 给[SerializeReference]特性提供一个类型选择器
    /// </summary>
    public sealed partial class SerializeRefSelectorAttribute
    {
        private struct NameSroter : IComparer<KeyValuePair<int, GUIContent>>
        {
            public static NameSroter DEFAULT = new NameSroter();
            public int Compare(KeyValuePair<int, GUIContent> x, KeyValuePair<int, GUIContent> y)
            {
                if (x.Key == y.Key)
                {
                    return x.Value.text.CompareTo(y.Value.text);
                }
                else
                {
                    return x.Key.CompareTo(y.Key);
                }
            }
        }

        private GUIContent[] m_SelectableNameAndTypes = null;
        private GUIContent[] SelectableNameAndTypes
        {
            get
            {
                if (SelectableTypes != null && m_SelectableNameAndTypes == null)
                {
                    var _sortedList =
                        new KeyValuePair<int, GUIContent>[] { new KeyValuePair<int, GUIContent>(int.MinValue, new GUIContent("Null")) }
                        .Concat(m_SelectableTypes.Select(kvp =>
                        {
                            int _order = 0;
                            kvp.Value.TryGetCustomDisplayName(out var _name, ref _order);
                            return new KeyValuePair<int, GUIContent>(_order, new GUIContent(kvp.Key, kvp.Value.FullName));
                        })).ToList();
                    _sortedList.Sort(NameSroter.DEFAULT);
                    m_SelectableNameAndTypes = _sortedList.Select(t => t.Value).ToArray();
                    return m_SelectableNameAndTypes;
                }
                return m_SelectableNameAndTypes;
            }
        }

        [CustomPropertyDrawer(typeof(SerializeRefSelectorAttribute))]
        private sealed class Drawer : PropertyDrawer
        {
            private static Dictionary<string, bool> s_IsFoldoutDict = new Dictionary<string, bool>();
            private static GUIStyle __HintStyle = null;
            private static GUIStyle s_HintStyle
            {
                get
                {
                    if (__HintStyle == null)
                    {
                        __HintStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                        __HintStyle.alignment = TextAnchor.MiddleRight;
                    }
                    return __HintStyle;
                }
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                s_IsFoldoutDict.TryGetValue(property.propertyPath, out bool _isFoldout);
                if (property.managedReferenceValue == null)
                {
                    _isFoldout = false;
                }
                float _propertyHeight = EditorGUIUtility.singleLineHeight;
                string _curPath = property.propertyPath + ".";
                bool _enter = true;
                while (_isFoldout && property.NextVisible(_enter))
                {
                    if (!property.propertyPath.StartsWith(_curPath))
                    {
                        _propertyHeight += EditorGUIUtility.standardVerticalSpacing;
                        break;
                    }
                    _enter = false;
                    _propertyHeight += EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.standardVerticalSpacing;
                }
                return _propertyHeight;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var _selector = attribute as SerializeRefSelectorAttribute;
                if (_selector == null)
                {
                    return;
                }
                float _lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                Rect _selectTypeRect = new Rect(position.x, position.y, position.width, _lineHeight);
                Rect _propertyRect = new Rect(position.x, _selectTypeRect.y + _lineHeight, position.width - EditorGUIUtility.standardVerticalSpacing, position.height - _lineHeight);

                EditorGUI.BeginProperty(position, label, property);

                int _selecting = GetSelectingIndex(_selector, property.managedReferenceValue);
                var _selectableTypeNames = _selector.SelectableNameAndTypes;
                if (_selecting < 0)
                {
                    _selectableTypeNames[0].text = "[Not Include]" + property.managedReferenceValue.GetType();
                    _selecting = 0;
                }
                else
                {
                    _selectableTypeNames[0].text = "Null";
                }
                int _newSelectIndex = EditorGUI.Popup(_selectTypeRect, label, _selecting, _selectableTypeNames);
                if (_selecting != _newSelectIndex)
                {
                    var _newType = GetSelectedType(_selector, _selectableTypeNames[_newSelectIndex]);
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
                    _selecting = _newSelectIndex;
                }

                if (EditorGUIUtility.currentViewWidth > 250f)
                {
                    var _IndentedHintTempRect = EditorGUI.IndentedRect(_selectTypeRect);
                    Rect _hintLabelRect = new Rect(_IndentedHintTempRect.x, _IndentedHintTempRect.y, _IndentedHintTempRect.width - 15f, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(_hintLabelRect, "Select Class", s_HintStyle);
                }

                s_IsFoldoutDict.TryGetValue(property.propertyPath, out bool _isFoldout);
                if (property.managedReferenceValue != null)
                {
                    var _isFoldoutNew = EditorGUI.Foldout(_selectTypeRect, _isFoldout, GUIContent.none, true);
                    if (_isFoldoutNew == false && _isFoldout == true)
                    {
                        s_IsFoldoutDict.Remove(property.propertyPath);
                    }
                    else if (_isFoldoutNew == true && _isFoldout == false)
                    {
                        s_IsFoldoutDict.TryAdd(property.propertyPath, _isFoldoutNew);
                    }
                    _isFoldout = _isFoldoutNew;
                }
                else
                {
                    _isFoldout = false;
                }
                if (_isFoldout)
                {
                    Rect _boxRect = position;
                    _boxRect.y += EditorGUIUtility.singleLineHeight;
                    _boxRect.height -= EditorGUIUtility.singleLineHeight;
                    GUI.Box(EditorGUI.IndentedRect(_boxRect), GUIContent.none);
                    EditorGUI.indentLevel++;
                    DrawInlineProperty(_propertyRect, property);
                    // EditorGUI.PropertyField(_propertyRect, property, true);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.EndProperty();
            }

            private void DrawInlineProperty(Rect position, SerializedProperty property)
            {
                string _curPath = property.propertyPath + ".";
                bool _enter = true;
                while (property.NextVisible(_enter))
                {
                    if (!property.propertyPath.StartsWith(_curPath))
                    {
                        break;
                    }
                    _enter = false;
                    position.height = EditorGUI.GetPropertyHeight(property, true);
                    EditorGUI.PropertyField(position, property, new GUIContent(property.displayName), true);
                    position.y = position.y + position.height + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            private int GetSelectingIndex(SerializeRefSelectorAttribute _selector, object curValue)
            {
                if (curValue == null)
                {
                    return 0;
                }
                string _fullName = curValue.GetType().FullName;
                int _index = Array.FindIndex(_selector.SelectableNameAndTypes, content => content.tooltip == _fullName);
                if (_index < 0)
                {
                    // if (!curValue.GetType().TryGetCustomDisplayName(out var _customName))
                    // {
                    //     _customName = _fullName;
                    // }
                    // _index = Array.FindIndex(_selector.SelectableNameAndTypes, content => content.text == _customName);
                    //TODO 类型不匹配的补救措施
                    return -1;
                }

                return _index;
            }

            private Type GetSelectedType(SerializeRefSelectorAttribute _selector, GUIContent typeInfo)
            {
                if (typeInfo.text == "Null" || typeInfo == null)
                {
                    return null;
                }
                else
                {
                    _selector.SelectableTypes.TryGetValue(typeInfo.text, out var _type);
                    return _type;
                }
            }
        }
    }
}
#endif