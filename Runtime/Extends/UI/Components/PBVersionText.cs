 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PBBox.UI
{

    [AddComponentMenu("PBBox/UI/Components/Version Text")]
    [DisallowMultipleComponent]
    public sealed class PBVersionText : MonoBehaviour
    {
        [System.Serializable]
        public class TextEvent : UnityEvent<string> { }
        [SerializeField]
        TextEvent m_OnGetVersion;
        public TextEvent onGetVersion
        {
            get
            {
                if (m_OnGetVersion == null)
                {
                    m_OnGetVersion = new TextEvent();
                }
                return m_OnGetVersion;
            }
        }
        
        public string prefix,suffix;
        public string text{get;private set;}

        void OnEnable()
        {
            text = prefix + Application.version + suffix;
            m_OnGetVersion?.Invoke(text);
        }
    }
}