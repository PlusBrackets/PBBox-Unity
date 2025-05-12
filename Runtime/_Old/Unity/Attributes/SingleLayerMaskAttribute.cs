/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.08.31
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PBBox.Attributes
{
    /// <summary>
    /// 将int类型绘制为单选的Layer
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public sealed class SingleLayerMaskAttribute : PropertyAttribute
    {
        public SingleLayerMaskAttribute() { }

#if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(SingleLayerMaskAttribute))]
        public class Drawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                SingleLayerMaskAttribute _attribute = (SingleLayerMaskAttribute)attribute;
                GUIContent _lable = new GUIContent(label);
                property.intValue = EditorGUI.LayerField(position, _lable, property.intValue);
            }
        }
#endif
    }
}