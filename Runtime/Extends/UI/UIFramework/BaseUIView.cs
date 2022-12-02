/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.31
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
#if USE_ODIN
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace PBBox.UI
{
    [DisallowMultipleComponent]
    public abstract class BaseUIView : MonoBehaviour, IUIView
    {
        [System.Serializable]
        public class BaseUIViewEvent : UnityEvent<BaseUIView> { }

#if USE_ODIN
        [InlineProperty, HideLabel, FoldoutGroup("View Property", true)]
#endif
        [SerializeField]
        UIViews.ViewConfigure m_Configure;

        public UIViews.ViewConfigure configure => m_Configure;
        IUIViewController IUIView.controller { get; set; } = null;
        public IUIViewController controller { get { return (this as IUIView).controller; } }

        SingleCoroutineExecuter m_WaitHideEndCoroutine;

        #region Evnets
        [SerializeField]
#if USE_ODIN
        [FoldoutGroup("Events")]
#endif
        BaseUIViewEvent m_OnShowEvent, m_OnResumeEvent, m_OnPauseEvent, m_OnHideEvent;

        public BaseUIViewEvent onShowEvent
        {
            get
            {
                return m_OnShowEvent ?? (m_OnShowEvent = new BaseUIViewEvent());
            }
        }

        public BaseUIViewEvent onResumeEvent
        {
            get
            {
                return m_OnResumeEvent ?? (m_OnResumeEvent = new BaseUIViewEvent());
            }
        }

        public BaseUIViewEvent onPauseEvent
        {
            get
            {
                return m_OnPauseEvent ?? (m_OnPauseEvent = new BaseUIViewEvent());
            }
        }

        public BaseUIViewEvent onHideEvent
        {
            get
            {
                return m_OnHideEvent ?? (m_OnHideEvent = new BaseUIViewEvent());
            }
        }
        #endregion
        string IUIView.uniqueID { get; set; }

        public string GetUniqueID()
        {
            return (this as IUIView).uniqueID;
        }
        
        public abstract string GetUIID();

        GameObject IUIView.GetGameObject() { return gameObject; }

        void IUIView.OnViewShow()
        {
            OnViewShow();
            m_OnShowEvent?.Invoke(this);
        }

        float IUIView.OnViewHide()
        {
            OnViewHide();
            m_OnHideEvent?.Invoke(this);
            return GetHideEndDelay();
        }

        void IUIView.OnViewResume()
        {
            OnViewResume();
            m_OnResumeEvent?.Invoke(this);
        }

        void IUIView.OnViewPause()
        {
            OnViewPause();
            m_OnPauseEvent?.Invoke(this);
        }

        protected virtual void OnViewShow()
        {
            m_WaitHideEndCoroutine.Stop();
            gameObject.SetActive(true);
        }

        protected virtual void OnViewHide()
        {
            if (GetHideEndDelay() <= Mathf.Epsilon)
            {
                m_WaitHideEndCoroutine.Stop();
                gameObject.SetActive(false);
            }
            else
            {
                m_WaitHideEndCoroutine.Start(WaitForHideEnd(GetHideEndDelay()), this);
            }
        }

        protected virtual void OnViewResume()
        {

        }

        protected virtual void OnViewPause()
        {

        }

        /// <summary>
        /// 返回hide后destroy view的延时,用于DestroyOnHide的时候
        /// </summary>
        /// <returns></returns>
        protected virtual float GetHideEndDelay()
        {
            return 0;
        }

        IEnumerator WaitForHideEnd(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (gameObject)
                gameObject.SetActive(false);
        }

    }
}