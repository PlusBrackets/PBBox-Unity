using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;
using System;

namespace PBBox.CEditor
{

    public class BitmapFontExporter : ScriptableWizard
    {
        [MenuItem("Tools/PBBox/UI/Create Font From BMFont")]
        private static void CreateFont()
        {
            ScriptableWizard.DisplayWizard<BitmapFontExporter>("Create Font");
        }


        public TextAsset fontFile;
        public Texture2D textureFile;
        public float xadvanceScaler = 1;
        public bool overrideXadvance = false;
        public int xadvance = 0;
        public Vector2Int offsetAddition = Vector2Int.zero;



        private void OnWizardCreate()
        {
            if (fontFile == null || textureFile == null)
            {
                return;
            }

            string path = EditorUtility.SaveFilePanelInProject("Save Font", fontFile.name, "", "");

            if (!string.IsNullOrEmpty(path))
            {
                ResolveFont(path);
            }
        }


        private void ResolveFont(string exportPath)
        {
            if (!fontFile) throw new UnityException(fontFile.name + "is not a valid font-xml file");

            Font font = new Font();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(fontFile.text);

            XmlNode info = xml.GetElementsByTagName("info")[0];
            XmlNodeList chars = xml.GetElementsByTagName("chars")[0].ChildNodes;

            CharacterInfo[] charInfos = new CharacterInfo[chars.Count];

            for (int cnt = 0; cnt < chars.Count; cnt++)
            {
                XmlNode node = chars[cnt];
                CharacterInfo charInfo = new CharacterInfo();

                charInfo.index = ToInt(node, "id");
                charInfo.advance = ToInt(node, "xadvance");
                if (overrideXadvance)
                    charInfo.advance = xadvance;
                charInfo.advance = (int)(charInfo.advance * xadvanceScaler);


                // charInfo.glyphHeight = (int)ToFloat(node, "height");
                // charInfo.glyphWidth = (int)ToFloat(node, "width");
                Rect uv = GetUV(node);
                charInfo.uvTopLeft = new Vector2(uv.xMin, uv.yMax);
                charInfo.uvTopRight = new Vector2(uv.xMax, uv.yMax);
                charInfo.uvBottomLeft = new Vector2(uv.xMin, uv.yMin);
                charInfo.uvBottomRight = new Vector2(uv.xMax, uv.yMin);
                Rect vert = GetVert(node);
                charInfo.minX = (int)vert.x + offsetAddition.x;
                charInfo.minY = (int)vert.y + offsetAddition.y;
                charInfo.maxX = (int)vert.xMax + offsetAddition.x;
                charInfo.maxY = (int)vert.yMax + offsetAddition.y;

                charInfos[cnt] = charInfo;
            }


            Shader shader = Shader.Find("Unlit/Transparent");
            Material material = new Material(shader);
            material.mainTexture = textureFile;
            //检测是否存在同名文件
            //覆盖同名文件
            AssetDatabase.CreateAsset(material, exportPath + ".mat");


            font.material = material;
            font.name = info.Attributes.GetNamedItem("face").InnerText;
            font.characterInfo = charInfos;
            AssetDatabase.CreateAsset(font, exportPath + ".fontsettings");
        }


        private Rect GetUV(XmlNode node)
        {
            Rect uv = new Rect();

            uv.x = ToFloat(node, "x") / textureFile.width;
            uv.y = ToFloat(node, "y") / textureFile.height;
            uv.width = ToFloat(node, "width") / textureFile.width;
            uv.height = ToFloat(node, "height") / textureFile.height;
            uv.y = 1f - uv.y - uv.height;

            return uv;
        }


        private Rect GetVert(XmlNode node)
        {
            Rect uv = new Rect();

            uv.x = ToFloat(node, "xoffset");
            uv.y = ToFloat(node, "yoffset");
            uv.width = ToFloat(node, "width");
            uv.height = ToFloat(node, "height");
            // uv.y = -uv.y;
            // uv.height = -uv.height;

            return uv;
        }


        private int ToInt(XmlNode node, string name)
        {
            return Convert.ToInt32(node.Attributes.GetNamedItem(name).InnerText);
        }


        private float ToFloat(XmlNode node, string name)
        {
            return (float)ToInt(node, name);
        }
    }
}
