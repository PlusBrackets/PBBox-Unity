using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Properties
{

    /// <summary>
    /// 可动态替换特定字符串的string
    /// </summary>
    [System.Serializable]
    public class ReplaceableString
    {
        [SerializeField, Multiline]
        string m_OriginalText;
        public string originalText => m_OriginalText;
        Dictionary<string, string> m_ReplaceDict;
        public Dictionary<string, string> replaceDict
        {
            get
            {
                // if (m_ReplaceDict == null)
                // {
                //     m_ReplaceDict = new Dictionary<string, string>();
                // }
                return m_ReplaceDict;
            }
        }

        StringBuilder m_StringCache;
        string m_ReplacedText;
        public string text
        {
            get
            {
                m_ReplacedText = TryUpdateResult();
                if (string.IsNullOrEmpty(m_ReplacedText))
                    return m_OriginalText;
                return m_ReplacedText;
            }
        }
        bool m_NeedUpdate = false;

        public ReplaceableString(string original)
        {
            m_OriginalText = original;
        }

        /// <summary>
        /// 更新替换的字符串组
        /// </summary>
        /// <param name="key">需要替换的字符串</param>
        /// <param name="content"></param>
        public void UpdateReplacePair(string key, string content)
        {
            if(replaceDict==null){
                m_ReplaceDict = new Dictionary<string, string>();
            }
            if (replaceDict.ContainsKey(key))
            {
                string oldContent = replaceDict[key];
                replaceDict[key] = content;
                if (oldContent != content && false == m_NeedUpdate)
                {
                    m_NeedUpdate = true;
                }
            }
            else
            {
                replaceDict.Add(key, content);
                m_NeedUpdate = true;
            }
        }

        string TryUpdateResult()
        {
            if (!m_NeedUpdate)
                return m_ReplacedText;
            if (m_StringCache == null)
            {
                m_StringCache = new StringBuilder(m_OriginalText);
            }
            else
            {
                m_StringCache.Clear();
                m_StringCache.Append(m_OriginalText);
            }
            foreach (var pair in replaceDict)
            {
                m_StringCache.Replace(pair.Key, pair.Value);
            }
            m_NeedUpdate = false;
            return m_StringCache.ToString();
        }
    }
}