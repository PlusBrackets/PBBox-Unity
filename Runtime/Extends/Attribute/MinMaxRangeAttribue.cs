/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.04.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PBBox.Attributes
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public sealed class MinMaxRangeAttribute : PropertyAttribute
    {
        public float min;
        public float max;

        /// <summary>
        /// 在inspector中将vector2类型显示为带有minmax范围的形式
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public MinMaxRangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

#if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
        public class Drawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (SerializedPropertyType.Vector2 != property.propertyType)
                {
                    EditorGUI.PropertyField(position, property);
                    return;
                }

                MinMaxRangeAttribute _attribute = (MinMaxRangeAttribute)attribute;

                float min = property.vector2Value.x;
                float max = property.vector2Value.y;
                EditorGUI.LabelField(position, property.displayName);
                //设置minmax区域
                float numWidth = Mathf.Clamp(EditorGUIUtility.currentViewWidth / 8, 40, 80);
                Rect fr = new Rect(position);
                fr.x = position.x + EditorGUIUtility.labelWidth + numWidth;
                fr.width = position.width - EditorGUIUtility.labelWidth - 2 * numWidth;
                //show minmax
                bool isChanged = false;
                EditorGUI.BeginChangeCheck();
                EditorGUI.MinMaxSlider(fr, ref min, ref max, _attribute.min, _attribute.max);
                isChanged = EditorGUI.EndChangeCheck();
                //set min value text pos
                fr.x -= numWidth;
                fr.width = numWidth - 2;
                //show min value
                EditorGUI.BeginChangeCheck();
                min = EditorGUI.FloatField(fr, min);
                if (EditorGUI.EndChangeCheck())
                {
                    min = Mathf.Clamp(min, _attribute.min, max);
                    isChanged = true;
                }
                //set max value text pos
                fr.x = position.x + position.width - fr.width;
                //show max value
                EditorGUI.BeginChangeCheck();
                max = EditorGUI.FloatField(fr, max);
                if (EditorGUI.EndChangeCheck())
                {
                    max = Mathf.Clamp(max, min, _attribute.max);
                    isChanged = true;
                }

                if (isChanged)
                {
                    property.vector2Value = new Vector2(min, max);
                }

            }
        }
#endif
    }
}