using System.Net.Http.Headers;
/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.03.23
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace PBBox.Unity.UI
{
    /// <summary>
    /// 带有尺寸范围的ContentSizeFitter
    /// </summary>
    public partial class ContentSizeFitterWithRange
    {
        [CustomEditor(typeof(ContentSizeFitterWithRange), true)]
        [CanEditMultipleObjects]
        private class Editor : UnityEditor.UI.ContentSizeFitterEditor
        {
            private SerializedProperty m_MaxWidth;
            private SerializedProperty m_MinWidth;
            private SerializedProperty m_MaxHeight;
            private SerializedProperty m_MinHeight;

            protected override void OnEnable()
            {
                base.OnEnable();
                m_MaxWidth = serializedObject.FindProperty("m_MaxWidth");
                m_MinWidth = serializedObject.FindProperty("m_MinWidth");
                m_MaxHeight = serializedObject.FindProperty("m_MaxHeight");
                m_MinHeight = serializedObject.FindProperty("m_MinHeight");
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                serializedObject.Update();
                ContentSizeFitterWithRange _target = target as ContentSizeFitterWithRange;
                if (DrawProperty(m_MaxWidth))
                {
                    m_MaxWidth.floatValue = _target.rectTransform.rect.width;
                }
                if (DrawProperty(m_MinWidth))
                {
                    m_MinWidth.floatValue = 0;
                }
                if (DrawProperty(m_MaxHeight))
                {
                    m_MaxHeight.floatValue = _target.rectTransform.rect.height;
                }
                if (DrawProperty(m_MinHeight))
                {
                    m_MinHeight.floatValue = 0;
                }
                serializedObject.ApplyModifiedProperties();
            }

            private bool DrawProperty(SerializedProperty property)
            {
                bool _enable = property.floatValue >= 0;
                EditorGUILayout.BeginHorizontal();
                GUIContent _guiContent = new GUIContent(property.displayName,null,property.tooltip);
                _enable = EditorGUILayout.Toggle(_guiContent, _enable, GUILayout.ExpandWidth(false));
                if (_enable)
                {
                    var _lableWidthTemp = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 2f;
                    if (EditorGUILayout.PropertyField(property,new GUIContent(" "), GUILayout.ExpandWidth(true)))
                    {
                        if (property.floatValue < 0)
                        {
                            property.floatValue = 0;
                        }
                    }
                    EditorGUIUtility.labelWidth = _lableWidthTemp;
                }
                else if (property.floatValue >= 0)
                {
                    property.floatValue = -1;
                }
                EditorGUILayout.EndHorizontal();
                return _enable && property.floatValue < 0;
            }
        }

    }
}

#endif