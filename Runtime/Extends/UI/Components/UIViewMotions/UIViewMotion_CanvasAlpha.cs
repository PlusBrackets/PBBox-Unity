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
    [AddComponentMenu("PBBox/UI/Framework/UIView Motions/Motion - Canvas Group Alpha")]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIViewMotion_CanvasAlpha : BaseUIViewMotion
    {
        [System.Serializable]
        public struct MotionSetting
        {
            public static MotionSetting EMPTY = new MotionSetting() { duration = -1f };
            [DefaultValue(0.3f)]
            public float duration;
            [DefaultValue(0f)]
            public float from;
            [DefaultValue(1f)]
            public float to;
            public AnimationCurve lerpCurve;
        }

        public bool useUnscaleTime = true;
#if USE_ODIN
        [InlineProperty, HideLabel, FoldoutGroup("On Show")]
        public MotionSetting showMotion = new MotionSetting() { duration = 0.16f, from = 0f, to = 1f, lerpCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f) };
        [InlineProperty, HideLabel, FoldoutGroup("On Hide")]
        public MotionSetting hideMotion = new MotionSetting() { duration = 0.24f, from = -1f, to = 0f, lerpCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f) };
        [InlineProperty, HideLabel, FoldoutGroup("On Resume")]
        public MotionSetting resumeMotion;
        [InlineProperty, HideLabel, FoldoutGroup("On Pause")]
        public MotionSetting pauseMotion;
#else
        public MotionSetting showMotion, hideMotion, resumeMotion, pauseMotion;
#endif

        CanvasGroup m_CanvasGroup;
        SingleCoroutineExecuter m_Executer;
        GameDualityTimer m_Timer;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (view != null && motionState.HasFlag(UIViewState.Hide) && view is UIView _view)
            {
                if (hideMotion.duration > 0f && _view.hideDelay < hideMotion.duration)
                {
                    _view.hideDelay = hideMotion.duration;
                }
            }
        }
#endif

        void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_Timer = new GameDualityTimer();
        }

        public MotionSetting GetSettingWithState(UIViewState state)
        {
            switch (state)
            {
                case UIViewState.Show:
                    return showMotion;
                case UIViewState.Hide:
                    return hideMotion;
                case UIViewState.Resume:
                    return resumeMotion;
                case UIViewState.Pause:
                    return pauseMotion;
                default:
                    return MotionSetting.EMPTY;
            }
        }

        protected override float OnGetMotionDur(UIViewState state)
        {
            return GetSettingWithState(state).duration;
        }

        protected override void OnMotion(BaseUIView view, UIViewState state)
        {
            m_Executer.Start(DoMotion(GetSettingWithState(state)), view);
        }

        IEnumerator DoMotion(MotionSetting setting)
        {
            if (setting.duration < 0f) yield break;

            var from = setting.from < 0f ? m_CanvasGroup.alpha : setting.from;
            var to = setting.to < 0f ? m_CanvasGroup.alpha : setting.to;
            var curve = setting.lerpCurve.length < 2 ? null : setting.lerpCurve;
            from = Mathf.Clamp01(from);
            to = Mathf.Clamp01(to);
            m_CanvasGroup.alpha = from;

            m_Timer.useUnscaleTime = useUnscaleTime;
            m_Timer.Start(setting.duration);

            while (!m_Timer.IsOver())
            {
                yield return null;
                m_CanvasGroup.alpha = Mathf.Lerp(from, to, curve != null ? curve.Evaluate(m_Timer.passTime, m_Timer.duration) : m_Timer.progress);
            }
            m_CanvasGroup.alpha = to;
        }

    }
}