/*--------------------------------------------------------
 *Copyright (c) 2016-2024 PlusBrackets
 *@update: 2024.07.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
using UnityEditor;

namespace PBBox.CEditor.ShaderGUI
{

    /// <summary>
    /// Enable or disable the property based on the pass tag, default is "SRPDefaultUnlit".
    /// Usage: [EnableIfPassTag(ShadowCaster)] _Vector2("Vector 2", Vector) = (0,0,0,0)
    /// </summary>
    public class EnableIfPassTagDecorator : MaterialPropertyDrawer
    {
        private readonly string m_PassLightModeTag;

        public EnableIfPassTagDecorator(){
            this.m_PassLightModeTag = "SRPDefaultUnlit";
        }

        public EnableIfPassTagDecorator(string passLightModeTag)
        {
            this.m_PassLightModeTag = passLightModeTag;
        }

        private bool GetPassEnabled(Material material)
        {
            return material.GetShaderPassEnabled(m_PassLightModeTag);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!GetPassEnabled(editor.target as Material))
            {
                GUI.enabled = false;
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return 0;
        }
    }

    public class DisableIfPassTagDecorator : MaterialPropertyDrawer
    {
        private readonly string m_PassLightModeTag;

        public DisableIfPassTagDecorator(){
            this.m_PassLightModeTag = "SRPDefaultUnlit";
        }

        public DisableIfPassTagDecorator(string passLightModeTag)
        {
            this.m_PassLightModeTag = passLightModeTag;
        }

        private bool GetPassEnabled(Material material)
        {
            return material.GetShaderPassEnabled(m_PassLightModeTag);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (GetPassEnabled(editor.target as Material))
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