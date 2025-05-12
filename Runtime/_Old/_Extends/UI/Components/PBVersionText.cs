/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.21
 *@author: PlusBrackets
 --------------------------------------------------------*/
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
        [SerializeField]
        UEvent_String m_OnGetVersion;
        public UEvent_String onGetVersion
        {
            get
            {
                if (m_OnGetVersion == null)
                {
                    m_OnGetVersion = new UEvent_String();
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