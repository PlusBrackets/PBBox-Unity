/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.04.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR 
using UnityEditor;
#endif

namespace PBBox.Attributes
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public sealed class DisplayNameAttribute : PropertyAttribute
    {
        public string displayName;

        public DisplayNameAttribute(string displayName)
        {
            this.displayName = displayName;
        }

#if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(DisplayNameAttribute))]
        public class Drawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                DisplayNameAttribute _attribute = (DisplayNameAttribute)attribute;
                GUIContent _lable = new GUIContent(label);
                _lable.text = _attribute.displayName;
                EditorGUI.PropertyField(position, property, _lable, true);
            }
        }
#endif
    }
}