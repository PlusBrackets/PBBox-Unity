/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.21
 *@author: PlusBrackets
 --------------------------------------------------------*/
//Define USE Odin
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
#if USE_ODIN
using Sirenix.OdinInspector;
#endif
//Define End
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.UI
{
    [Tooltip("提供简单的显示、关闭UI的触发器")]
    [AddComponentMenu("PBBox/UI/Framework/UI View Trigger")]
    /// <summary>
    /// 提供简单的显示、关闭UI的触发器
    /// </summary>
    public sealed class UIViewTrigger : MonoBehaviour
    {
        [System.Serializable]
        public struct Params
        {
            public string[] handleUIID;
            [Space]
            public MutiTags handleTags;
            public bool tagAny;
            public bool tagIngoreCase;
            [Space]
            public bool showOnEnable;
            public bool hideOnDisable;
            [Space]
            public bool hideOnEnable;
            public bool showOnDisable;
            [Space]
            public bool triggerOnce;
        }
#if USE_ODIN
        [HideLabel, FoldoutGroup("Trigger Params")]
#endif
        [SerializeField]
        Params m_TriggerParam;
        bool m_Triggered = false;

        bool CanTrigger()
        {
            return !m_Triggered || !m_TriggerParam.triggerOnce;
        }

        void OnEnable()
        {
            if (!CanTrigger())
                return;
            if (m_TriggerParam.showOnEnable)
            {
                m_Triggered = true;
                ShowHandles();
            }
            else if (m_TriggerParam.hideOnEnable)
            {
                m_Triggered = true;
                HideHandles();
            }
        }

        void OnDisable()
        {
            if (!CanTrigger())
                return;
            if (m_TriggerParam.showOnDisable)
            {
                m_Triggered = true;
                ShowHandles();
            }
            else if (m_TriggerParam.hideOnDisable)
            {
                m_Triggered = true;
                HideHandles();
            }
        }

        public void ShowHandles()
        {
            if (m_TriggerParam.handleUIID != null)
            {
                foreach (var id in m_TriggerParam.handleUIID)
                {
                    UIViews.Show(id);
                }
            }
            //TODO ShowWithTags等待实装
        }

        public void HideHandles()
        {
            if (m_TriggerParam.handleUIID != null)
            {
                foreach (var id in m_TriggerParam.handleUIID)
                {
                    UIViews.Hide(id);
                }
            }
            if (m_TriggerParam.handleTags)
            {
                Debug.Log(m_TriggerParam.handleTags.ToString());
                string[] tags = m_TriggerParam.handleTags.ToArray();
                UIViews.HideWithTag(t =>
                {
                    if (m_TriggerParam.tagAny)
                    {
                        return t.HasAnyTags(tags, m_TriggerParam.tagIngoreCase ? System.StringComparison.OrdinalIgnoreCase : System.StringComparison.Ordinal);
                    }
                    else
                    {
                        return t.HasAllTags(tags, m_TriggerParam.tagIngoreCase ? System.StringComparison.OrdinalIgnoreCase : System.StringComparison.Ordinal);
                    }
                }, true);
            }
        }

        public void ShowUIView(string uiid)
        {
            UIViews.Show(uiid, null);
        }

        public void HideUIView(string uiid)
        {
            UIViews.Hide(uiid, null);
        }

        public void PauseUIView(string uiid)
        {
            UIViews.Pause(uiid, null);
        }

        public void ResumeUIView(string uiid)
        {
            UIViews.Resume(uiid, null);
        }

        // public void ShowUIView(string uiid, string uniqueID)
        // {
        //     UIViews.Show(uiid, uniqueID);
        // }

        // public void HideUIView(string uiid, string uniqueID)
        // {
        //     UIViews.Hide(uiid, uniqueID);
        // }

        // public void PauseUIView(string uiid, string uniqueID)
        // {
        //     UIViews.Pause(uiid, uniqueID);
        // }

        // public void ResumeUIView(string uiid, string uniqueID)
        // {
        //     UIViews.Resume(uiid, uniqueID);
        // }

        // public void HideSelf()
        // {
        //     GetComponent<IUIView>()?.Hide();
        // }

        // public void PauseSelf()
        // {
        //     GetComponent<IUIView>()?.Pause();
        // }

        // public void ResumeSelf()
        // {
        //     GetComponent<IUIView>()?.Resume();
        // }

        // public void ShowSelf()
        // {
        //     GetComponent<IUIView>()?.Show();
        // }
    }
}