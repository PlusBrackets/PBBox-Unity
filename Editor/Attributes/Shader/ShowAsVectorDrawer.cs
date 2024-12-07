using UnityEngine;
using UnityEditor;

namespace PBBox.CEditor.ShaderGUI
{

    /// <summary>
    /// Draws a vector2 field for shader vector properties.
    /// Usage: [ShowAsVector2] _Vector2("Vector 2", Vector) = (0,0,0,0)
    /// </summary>
    public class ShowAsVector2Drawer : MaterialPropertyDrawer
    {
        protected virtual Vector4 ShowVectorField(Rect position, MaterialProperty prop, GUIContent label)
        {
            return EditorGUI.Vector2Field(position, label, prop.vectorValue);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (prop.type == MaterialProperty.PropType.Vector)
            {
                EditorGUIUtility.labelWidth = 0f;
                // EditorGUIUtility.fieldWidth = 0f;

                // if (!EditorGUIUtility.wideMode)
                // {
                //     EditorGUIUtility.wideMode = true;
                //     EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212;
                // }

                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = prop.hasMixedValue;
                Vector4 vec = ShowVectorField(position, prop, label);
                if (EditorGUI.EndChangeCheck())
                {
                    prop.vectorValue = vec;
                }
            }
            else
            {
                editor.DefaultShaderProperty(prop, label.text);
            }

        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            var height = base.GetPropertyHeight(prop, label, editor);
            if (!EditorGUIUtility.wideMode)
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            return height;
        }
    }

    /// <summary>
    /// Draws a vector3 field for vector properties.
    /// Usage: [ShowAsVector3] _Vector3("Vector 3", Vector) = (0,0,0,0)
    /// </summary>
    public class ShowAsVector3Drawer : ShowAsVector2Drawer
    {
        protected override Vector4 ShowVectorField(Rect position, MaterialProperty prop, GUIContent label)
        {
            return EditorGUI.Vector3Field(position, label, prop.vectorValue);
        }
    }
}