/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.21
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.UI
{
    /// <summary>
    /// 用于UIView的一些行为,例如切换至各种状态时的动态
    /// </summary>
    [AddComponentMenu("")]
    [RequireComponent(typeof(BaseUIView))]
    public abstract class BaseUIViewMotion : MonoBehaviour
    {

        [SerializeField]
        BaseUIView m_View;
        public BaseUIView view => m_View;
        [SerializeField]
        protected UIViewState m_MotionState = UIViewState.Show | UIViewState.Hide;
        public UIViewState motionState => m_MotionState;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (!m_View)
            {
                m_View = GetComponent<BaseUIView>();
            }
        }
#endif
        protected virtual void OnEnable()
        {
            if (!m_View)
            {
                m_View = GetComponent<BaseUIView>();
            }
            m_View.onShowEvent.AddListener(OnViewShow);
            m_View.onResumeEvent.AddListener(OnViewResume);
            m_View.onPauseEvent.AddListener(OnViewPause);
            m_View.onHideEvent.AddListener(OnViewHide);
            // // var state = m_View.GetState();
            // // if (state.HasFlag(UIViewState.Show)) OnViewShow(m_View);
            // // if (state.HasFlag(UIViewState.Resume)) OnViewResume(m_View);
            // // if (state.HasFlag(UIViewState.Pause)) OnViewPause(m_View);
            // // if (state.HasFlag(UIViewState.Hide)) OnViewHide(m_View);
                
        }

        protected virtual void OnDisable()
        {
            if (m_View)
            {
                m_View.onShowEvent.RemoveListener(OnViewShow);
                m_View.onResumeEvent.RemoveListener(OnViewResume);
                m_View.onPauseEvent.RemoveListener(OnViewPause);
                m_View.onHideEvent.RemoveListener(OnViewHide);
            }
        }

        void OnViewShow(BaseUIView view)
        {
            if (motionState.HasFlag(UIViewState.Show) == false)
                return;
            OnMotion(view, UIViewState.Show);
        }

        void OnViewResume(BaseUIView view)
        {
            if (motionState.HasFlag(UIViewState.Resume) == false)
                return;
            OnMotion(view, UIViewState.Resume);
        }

        void OnViewPause(BaseUIView view)
        {
            if (motionState.HasFlag(UIViewState.Pause) == false)
                return;
            OnMotion(view, UIViewState.Pause);
        }

        void OnViewHide(BaseUIView view)
        {
            if (motionState.HasFlag(UIViewState.Hide) == false)
                return;
            OnMotion(view, UIViewState.Hide);
        }
        
        public float GetMotionDuration(UIViewState state)
        {
            if (motionState.HasFlag(state))
            {
                return OnGetMotionDur(state);
            }
            return -1f;
        }

        protected abstract float OnGetMotionDur(UIViewState state);

        protected abstract void OnMotion(BaseUIView view, UIViewState state);

    }

}