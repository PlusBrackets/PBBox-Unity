/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.UI
{
    public sealed partial class UIViews : SingleClass<UIViews>
    {
        class StackedViewState
        {
            public string uiid;
            public ViewFlags flags;
            public int hideCount;
            public int forceHideCount;

            public StackedViewState(IUIView view)
            {
                uiid = view.GetUIID();
                hideCount = 0;
                forceHideCount = 0;
                this.flags = view.configure.uiFlags;
            }

            public StackedViewState(string uiid, ViewFlags flags)
            {
                this.uiid = uiid;
                hideCount = 0;
                forceHideCount = 0;
                this.flags = flags;
            }

            public void FlatHideCount(int flat, int force)
            {
                hideCount += flat;
                forceHideCount += force;
                hideCount = Mathf.Max(hideCount, 0);
            }

            public bool IsView(IUIView view)
            {
                return view.GetUIID() == uiid;
            }

            public bool IsHide()
            {
                if (forceHideCount > 0)
                    return true;
                if (hideCount > 0 && !flags.HasFlag(ViewFlags.PauseIfHideAtStack))
                    return true;
                return false;
            }

            public static implicit operator bool(StackedViewState exists)
            {
                return !string.IsNullOrEmpty(exists.uiid);
            }
        }

        class UIStack
        {
            public const string DEFAULT_STACK_ID = "__Default";
            public string stackID { get; private set; }
            Lazy<List<StackedViewState>> m_ViewStates;
            internal List<StackedViewState> viewStates => m_ViewStates.Value;

            public UIStack(string id)
            {
                stackID = id;
                m_ViewStates = new Lazy<List<StackedViewState>>();
            }


            internal void PushBeforeShow(IUIView view)
            {
                StackedViewState targetView = new StackedViewState(view);
                bool hasStacked = viewStates.TryPeek(out StackedViewState previousViewState);
                viewStates.Push(targetView);
                var uiFlags = view.configure.uiFlags;

                if (hasStacked)
                {
                    if (uiFlags.HasFlag(ViewFlags.HideAllAtStack))
                    {
                        int forceCount = targetView.flags.HasFlag(ViewFlags.Ingore_PauseIf_Flags) ? 1 : 0;
                        //尝试倒序隐藏前面全部
                        for (int i = viewStates.Count - 2; i >= 0; i--)
                        {
                            var _sv = viewStates[i];
                            TryHideOrPauseView(_sv, 1, forceCount);
                        }
                    }
                    else if (uiFlags.HasFlag(ViewFlags.HidePreviousAtStack))
                    {
                        //隐藏前一个
                        TryHideOrPauseView(previousViewState, 1, targetView.flags.HasFlag(ViewFlags.Ingore_PauseIf_Flags) ? 1 : 0);
                    }
                    else
                    {
                        Pause(previousViewState.uiid);
                    }
                }

                void TryHideOrPauseView(StackedViewState state, int hideCount, int forceCount)
                {
                    state.FlatHideCount(hideCount, forceCount);
                    if (state.IsHide())
                    {
                        _instance.HideView(Get(state.uiid), false);
                    }
                    else
                    {
                        Pause(state.uiid);
                    }
                }
            }

            internal void PopAfterHide(IUIView view)
            {
                StackedViewState lastViewState;
                bool hasStacked = viewStates.TryPeek(out lastViewState);
                if (!hasStacked)
                    return;
                int hideCount = 0;
                int forceCount = 0;
                if (lastViewState.IsView(view))
                {
                    viewStates.Pop();
                    hasStacked = viewStates.TryPeek(out var previousViewState);
                    if (hasStacked)
                    {
                        if (view.configure.uiFlags.HasFlag(ViewFlags.HideAllAtStack))
                        {
                            hideCount += 1;
                            forceCount += lastViewState.flags.HasFlag(ViewFlags.Ingore_PauseIf_Flags) ? 1 : 0;
                            //尝试显示之前的
                            foreach (var _sv in viewStates)
                            {
                                TryResumeOrShowView(_sv, hideCount, forceCount, _sv == previousViewState);
                            }
                        }
                        else if (view.configure.uiFlags.HasFlag(ViewFlags.HidePreviousAtStack))
                        {
                            hideCount += 1;
                            forceCount += lastViewState.flags.HasFlag(ViewFlags.Ingore_PauseIf_Flags) ? 1 : 0;
                            TryResumeOrShowView(previousViewState, hideCount, forceCount, true);
                        }
                        else
                        {
                            TryResumeOrShowView(previousViewState, hideCount, forceCount, true);
                        }
                    }
                }
                void TryResumeOrShowView(StackedViewState state, int hideCount, int forceCount, bool resume)
                {
                    state.FlatHideCount(-hideCount, -forceCount);
                    if (!state.IsHide())
                    {
                        var v = GetOrCreate(state.uiid);
                        if (v.IsShowing() && resume)
                        {
                            Resume(v);
                        }
                        else
                        {
                            _instance.ShowView(v, false, !resume);
                        }
                    }
                }
            }
        }

        Dictionary<string, UIStack> m_ViewGroups;

        void InitViewStacked()
        {
            m_ViewGroups = new Dictionary<string, UIStack>();
            m_ViewGroups.Add(UIStack.DEFAULT_STACK_ID, new UIStack(UIStack.DEFAULT_STACK_ID));
        }

        void PushView(IUIView view)
        {
            m_ViewGroups[UIStack.DEFAULT_STACK_ID].PushBeforeShow(view);
        }

        void PopView(IUIView view)
        {
            m_ViewGroups[UIStack.DEFAULT_STACK_ID].PopAfterHide(view);
        }


    }

}