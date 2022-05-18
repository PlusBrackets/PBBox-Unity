/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.15
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
#if USE_ODIN
using Sirenix.OdinInspector;
#endif
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.UI
{
    [AddComponentMenu("PBBox/UI/Framework/UIView Motions/Motion - Animtor")]
    [RequireComponent(typeof(Animator))]
    public class UIViewMotion_Animator : BaseUIViewMotion
    {
        Animator m_Animator;

        public string showTrigger = "show";
        public string hideTrigger = "hide";
        // public string resumeTrigger = "resume";
        // public string pauseTrigger = "pause";

        void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }


        protected override float OnGetMotionDur(UIViewState state)
        {
            return 0f;
        }

        public string GetTriggerName(UIViewState state)
        {
            switch (state)
            {
                case UIViewState.Show:
                    return showTrigger;
                case UIViewState.Hide:
                    return hideTrigger;
                default:
                    return null;
            }
        }

        protected override void OnMotion(BaseUIView view, UIViewState state)
        {
            string trigger = GetTriggerName(state);
            if (string.IsNullOrEmpty(trigger))
                return;
            m_Animator.ResetTrigger(trigger);
            m_Animator.SetTrigger(trigger);
        }
    }
}