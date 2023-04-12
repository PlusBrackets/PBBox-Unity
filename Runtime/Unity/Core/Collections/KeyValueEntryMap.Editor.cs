/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.04.12
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PBBox.Collections
{
    [CustomPropertyDrawer(typeof(KeyValueEntryMap<,,>), true)]
    internal class KeyValueEntryMapDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var listProperty = property.FindPropertyRelative("m_Maps");
            EditorGUI.PropertyField(position, listProperty, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var listProperty = property.FindPropertyRelative("m_Maps");
            return EditorGUI.GetPropertyHeight(listProperty, true);
        }
    }
}
#endif