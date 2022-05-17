using UnityEngine;
using PBBox.Variables;
using UnityEditor;

namespace PBBox.CEditor
{
    [CustomPropertyDrawer(typeof(SDictionary<,>))]
    public class SDictionaryEditor : PropertyDrawer
    {
        private SerializedProperty listProperty;

        private SerializedProperty getListProperty(SerializedProperty property) =>
            listProperty ??= property.FindPropertyRelative("maps");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, getListProperty(property), label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(getListProperty(property), true);
        }
    }
}