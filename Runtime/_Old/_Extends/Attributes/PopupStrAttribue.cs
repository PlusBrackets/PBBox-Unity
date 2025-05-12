using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PBBox.Attributes
{
    [System.Obsolete]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public sealed class PopupStrAttribute : PropertyAttribute
    {
        public string[] strs { get; private set; }
        public bool fullName { get; private set; }

        /// <summary>
        /// 在inspector中将字符串类型显示为下拉菜单的形式
        /// </summary>
        /// <param name="strs">菜单内容，使用PopupField的内容格式</param>
        /// <param name="fullName">是否使用完整的下来菜单内容。若true，多级菜单时内容会变成aa/bb/cc的格式；若false则只取最终的选项内容（false时最终选项不能有相同字符串）</param>
        public PopupStrAttribute(string[] strs, bool fullName = true)
        {
            this.strs = strs;
            this.fullName = fullName;
        }

        public PopupStrAttribute(bool fullName, params string[] strs)
        {
            this.strs = strs;
            this.fullName = fullName;
        }

        /// <summary>
        /// 在inspector中将字符串类型显示为下拉菜单的形式
        /// </summary>
        /// <param name="strType">菜单显示的类型</param>
        /// <param name="fullName">是否使用完整的下来菜单内容。若true，多级菜单时内容会变成aa/bb/cc的格式；若false则只取最终的选项内容（false时最终选项不能有相同字符串）</param>
        public PopupStrAttribute(PopupStrSetting.StrType strType, bool fullName = true)
        {
            strs = PopupStrSetting.StrDic[strType];
            this.fullName = fullName;
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(PopupStrAttribute))]
        public class Drawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (SerializedPropertyType.String != property.propertyType)
                {
                    EditorGUI.PropertyField(position, property);
                    return;
                }

                PopupStrAttribute _attribute = (PopupStrAttribute)attribute;
                int _selectingIndex = -1;

                if (_attribute.fullName)
                {
                    _selectingIndex = Array.IndexOf(_attribute.strs, property.stringValue);
                }
                else
                {
                    string _psv = property.stringValue;
                    for (int i = 0; i < _attribute.strs.Length; i++)
                    {
                        if (_attribute.strs[i].EndsWith(_psv))
                        {
                            _selectingIndex = i;
                            break;
                        }
                    }
                }

                EditorGUI.BeginChangeCheck();

                int _index = EditorGUI.Popup(position, property.displayName, Math.Max(0, _selectingIndex), _attribute.strs);

                if (EditorGUI.EndChangeCheck())
                {
                    if (_attribute.fullName)
                        property.stringValue = _attribute.strs[_index];
                    else
                    {
                        string[] _temp = _attribute.strs[_index].Split('/');
                        property.stringValue = _temp[_temp.Length - 1];
                    }
                }

            }
        }
#endif
    }
}