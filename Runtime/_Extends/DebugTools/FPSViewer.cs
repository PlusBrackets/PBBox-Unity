/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;

namespace PBBox.Tools
{
    [AddComponentMenu("PBBox/Tools/FPSViewer")]
    public class FPSViewer : SingleBehaviour<FPSViewer>
    {
        public enum DisplayPosition
        {
            leftTop,
            middleTop,
            rightTop
        }
        public bool showOnScreen = true;
        [SerializeField]
        private int m_LockFPS = -1;
        [Range(10, 40)]
        public int fontSize = 12;
        public DisplayPosition displayPosition;
        public Color badColor = Color.red;
        public float badGoodDivide = 30f;
        public Color goodColor = Color.yellow;
        public float goodBestDivide = 60f;
        public Color bestColor = Color.green;
        public float fpsMeasuringDelta = 2.0f;

        private float m_TimePassed;
        private int m_FrameCount = 0;
        private float m_FPS = 0.0f;
        private float m_GuiWidth { get { return fontSize * 6f; } }
        private float m_GuiHeight = 20;
        private GUIStyle _guiStyle = new GUIStyle();


        private Rect m_DisplayRect
        {
            get
            {
                switch (displayPosition)
                {
                    case DisplayPosition.leftTop:
                        return new Rect(0, 0, m_GuiWidth, m_GuiHeight);
                    case DisplayPosition.rightTop:
                        return new Rect(Screen.width - m_GuiWidth, 0, m_GuiWidth, m_GuiHeight);
                    case DisplayPosition.middleTop:
                        return new Rect(Screen.width / 2 - m_GuiWidth / 2, 0, m_GuiWidth, m_GuiHeight);
                    default:
                        return default;
                }
            }
        }

        private Color m_DisplayColor
        {
            get
            {
                if (m_FPS < badGoodDivide)
                    return badColor;
                else if (m_FPS < goodBestDivide)
                    return goodColor;
                else
                    return bestColor;
            }
        }

        private void Start()
        {
            if (m_LockFPS > 0)
                Application.targetFrameRate = m_LockFPS;
            m_TimePassed = 0.0f;
            _guiStyle.normal.background = null;
            _guiStyle.normal.textColor = m_DisplayColor;
            _guiStyle.fontSize = fontSize;
        }

        private void Update()
        {
            if (!showOnScreen)
                return;
            m_FrameCount = m_FrameCount + 1;
            m_TimePassed = m_TimePassed + Time.unscaledDeltaTime;

            if (m_TimePassed > fpsMeasuringDelta)
            {
                m_FPS = m_FrameCount / m_TimePassed;
                m_TimePassed = 0.0f;
                m_FrameCount = 0;
            }
        }

        private void OnGUI()
        {
            if (showOnScreen)
            {
                _guiStyle.normal.textColor = m_DisplayColor;
                _guiStyle.fontSize = fontSize;
                GUI.Label(m_DisplayRect, $"FPS: {m_FPS.ToString("00.00")}", _guiStyle);
            }
        }
    }
}