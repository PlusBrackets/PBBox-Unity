/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PBBox.Tools
{
    [AddComponentMenu("PBBox/Tools/LogViewer")]
    public class LogViewer : SingleBehaviour<LogViewer>
    {
        private static List<string> m_Lines = new List<string>();
        private static List<string> m_WriteTxt = new List<string>();

        [Tooltip("是否在屏幕上显示")]
        public bool isLogToScreen = true;
        [Range(10, 40)]
        public int fontSize = 12;
        [Tooltip("是否记录非错误的的log")]
        public bool isLogNormal = true;
        [Tooltip("是否将log文件保存在本地")]
        public bool isSaveLogFile = true;
        [Tooltip("是否在start时删除上一次的log文件")]
        public bool isDelFileWhenStart = true;
        [SerializeField, Tooltip("保存的文件名")]
        private string m_LogFileName = "Log";

        private string m_Outpath;
        private GUIStyle _guiStyle = new GUIStyle();

        private void Start()
        {
            m_Outpath = Application.persistentDataPath + $"/{m_LogFileName}.txt";

            if (isDelFileWhenStart)
            {
                if (File.Exists(m_Outpath))
                {
                    File.Delete(m_Outpath);
                }
            }
            Application.logMessageReceived += HandleLog;
            _guiStyle.normal.background = null;
            _guiStyle.fontSize = fontSize;
        }

        private void Update()
        {
            if (isSaveLogFile)
            {
                if (m_WriteTxt.Count > 0)
                {
                    string[] temp = m_WriteTxt.ToArray();
                    foreach (string t in temp)
                    {
                        using (StreamWriter writer = new StreamWriter(m_Outpath, true, Encoding.UTF8))
                        {
                            writer.WriteLine(t);
                        }
                        m_WriteTxt.Remove(t);
                    }
                }
            }
        }

        private void HandleLog(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                if (isSaveLogFile)
                    m_WriteTxt.Add(condition + "\n" + ("\t" + stackTrace.Replace("\n", "\n\t")));
                LogInScreen(condition);
                LogInScreen(stackTrace);
            }
            else
            {
                if (isSaveLogFile)
                    m_WriteTxt.Add(condition);
                if (isLogNormal)
                    LogInScreen(condition);
            }
        }

        public static void LogInScreen(params object[] objs)
        {
            string text = "";
            for (int i = 0; i < objs.Length; ++i)
            {
                if (i == 0)
                {
                    text += objs[i].ToString();
                }
                else
                {
                    text += ", " + objs[i].ToString();
                }
            }
            if (Application.isPlaying)
            {
                if (m_Lines.Count > 20)
                {
                    m_Lines.RemoveAt(0);
                }
                m_Lines.Add(text);

            }
        }

        void OnGUI()
        {
            if (!isLogToScreen)
                return;
            _guiStyle.normal.textColor = Color.red;
            _guiStyle.fontSize = fontSize;
            for (int i = 0, imax = m_Lines.Count; i < imax; ++i)
            {
                GUILayout.Label(m_Lines[i], _guiStyle);
            }
        }
    }
}