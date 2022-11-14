using UnityEngine;
using PBBox.Variables;
using UnityEditor;

namespace PBBox.CEditor
{
    [CustomPropertyDrawer(typeof(SDictionary<,>))]
    public class SDictionaryEditor : PropertyDrawer
    {
        // private SerializedProperty listProperty;

        // private SerializedProperty getListProperty(SerializedProperty property) =>
        //     listProperty ??= property.FindPropertyRelative("maps");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var listProperty = property.FindPropertyRelative("maps");
            EditorGUI.PropertyField(position, listProperty, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {   
            var listProperty = property.FindPropertyRelative("maps");
            return EditorGUI.GetPropertyHeight(listProperty, true);
        }
    }
}