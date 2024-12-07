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
    public class EnableIfKeywordDecorator : MaterialPropertyDrawer
    {
        private readonly string m_Keyword;

        public EnableIfKeywordDecorator(string keyword)
        {
            this.m_Keyword = keyword;
        }

        private bool GetKeywordDefined(Material material)
        {
            return material.IsKeywordEnabled(m_Keyword);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!GetKeywordDefined(editor.target as Material))
            {
                GUI.enabled = false;
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return 0;
        }
    }

    public class DisableIfKeywordDecorator : MaterialPropertyDrawer
    {
        private readonly string m_Keyword;

        public DisableIfKeywordDecorator(){
            this.m_Keyword = "_";
        }

        public DisableIfKeywordDecorator(string keyword)
        {
            this.m_Keyword = keyword;
        }
        
        private bool GetKeywordDefined(Material material)
        {
            return material.IsKeywordEnabled(m_Keyword);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (GetKeywordDefined(editor.target as Material))
            {
                GUI.enabled = false;
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return 0;
        }
    }

}