// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor.UI;
// using UnityEditor;
// using PBBox.UI;

// namespace PBBox.CEditor
// {
//     [CustomEditor(typeof(LerpNumText))]
//     [CanEditMultipleObjects]
//     public class LerpNumTextEditor : UnityEditor.UI.TextEditor
//     {
//         private SerializedProperty lerpTime;
//         private SerializedProperty lerpCurve;
//         private SerializedProperty m_UseUnscaleTime;
//         private SerializedProperty prefix;
//         private SerializedProperty suffix;
//         private SerializedProperty intNumber;
//         private SerializedProperty numFormat;

//         protected override void OnEnable()
//         {
//             base.OnEnable();
//             lerpTime = serializedObject.FindProperty(nameof(lerpTime));
//             lerpCurve = serializedObject.FindProperty(nameof(lerpCurve));
//             m_UseUnscaleTime = serializedObject.FindProperty(nameof(m_UseUnscaleTime));
//             prefix = serializedObject.FindProperty(nameof(prefix));
//             suffix = serializedObject.FindProperty(nameof(suffix));
//             intNumber = serializedObject.FindProperty(nameof(intNumber));
//             numFormat = serializedObject.FindProperty(nameof(numFormat));
//         }
 
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//             serializedObject.Update();
//             EditorGUILayout.PropertyField(lerpTime);
//             EditorGUILayout.PropertyField(lerpCurve);
//             EditorGUILayout.PropertyField(m_UseUnscaleTime);
//             EditorGUILayout.PropertyField(prefix);
//             EditorGUILayout.PropertyField(suffix);
//             EditorGUILayout.PropertyField(intNumber);
//             EditorGUILayout.PropertyField(numFormat);
//             serializedObject.ApplyModifiedProperties();
//         }
//     }
// }