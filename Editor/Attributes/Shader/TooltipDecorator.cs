/*--------------------------------------------------------
 *Copyright (c) 2016-2024 PlusBrackets
 *@update: 2024.07.18
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
using UnityEditor;

namespace PBBox.CEditor.ShaderGUI
{
    /// <summary>
    /// Enable or disable the property based on the keyword
    /// Usage: [EnableIfKeyword(_ALPHA_TEST_ON)] _Vector2("Vector 2", Vector) = (0,0,0,0)
    /// </summary>
    public class TooltipDecorator : MaterialPropertyDrawer
    {
        private readonly string m_Tooltip;

        public TooltipDecorator(string keyword)
        {
            this.m_Tooltip = keyword;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            label.tooltip = m_Tooltip;
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return 0;
        }
    }
}