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
            public string uiid => holdingView.uiid;
            public string uniqueID => holdingView.uniqueID;
            public ViewFlags flags => holdingView.flags;
            public HoldingView holdingView;
            public int hideCount;
            public int forceHideCount;

            public StackedViewState(HoldingView viewHoldingData)
            {
                holdingView = viewHoldingData;
                hideCount = 0;
                forceHideCount = 0;
            }

            public void FlatHideCount(int flat, int force)
            {
                hideCount += flat;
                forceHideCount += force;
                hideCount = Mathf.Max(hideCount, 0);
            }

            public bool IsView(IUIView view)
            {
                return holdingView.IsView(view);
            }

            public bool IsView(HoldingView view)
            {
                return holdingView == view || (holdingView.uiid == view.uiid && holdingView.uniqueID == view.uniqueID);
            }

            public bool IsHide()
            {
                if (forceHideCount > 0)
                    return true;
                if (hideCount > 0 && !holdingView.flags.HasFlag(ViewFlags.PauseIfHideAtStack))
                    return true;
                return false;
            }

            public static implicit operator bool(StackedViewState exists)
            {
                return exists.holdingView;
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

            internal bool IsViewInStackButNoOnTop(HoldingView view)
            {
                if (view.isStacked)
                {
                    if (viewStates.TryPeek(out var vs) && !vs.IsView(view))
                    {
                        return true;
                    }
                }
                return false;
            }


            internal void PushBeforeShow(HoldingView view, bool hideStackBehind)
            {
                //Check
                bool isInStackButNoTop = IsViewInStackButNoOnTop(view);
                if (isInStackButNoTop)
                {
                    int index = viewStates.FindLastIndex(vs => vs.IsView(view));
                    if (hideStackBehind)
                    {
                        PopAfterHide(viewStates[index + 1].holdingView, true);
                    }
                    return;
                }
                view.isStacked = true;
                StackedViewState targetView = new StackedViewState(view);
                bool hasStacked = viewStates.TryPeek(out StackedViewState previousViewState);
                viewStates.Push(targetView);
                var uiFlags = view.flags;

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
                        //暂停前一个
                        TryHideOrPauseView(previousViewState, 0, 0);
                    }
                }

                void TryHideOrPauseView(StackedViewState state, int hideCount, int forceCount)
                {
                    state.FlatHideCount(hideCount, forceCount);
                    if (state.IsHide())
                    {
                        _instance.HideView(Get(state.uiid, state.uniqueID), state.holdingView, false);
                    }
                    else
                    {
                        _instance.PauseView(Get(state.uiid, state.uniqueID));
                    }
                }
            }

            internal void PopAfterHide(HoldingView holdingView, bool hideStackBehind)
            {
                StackedViewState lastViewState;
                bool hasStacked = viewStates.TryPeek(out lastViewState);
                if (!hasStacked || (!hideStackBehind && !lastViewState.IsView(holdingView)))
                    return;
                int hideCount = 0;
                int forceCount = 0;
                if (!lastViewState.IsView(holdingView))
                {   
                    //将在该view之后入栈的view全部关闭
                    int index = viewStates.FindLastIndex(s => s.IsView(holdingView));
                    if (index == -1) return;
                    lastViewState = viewStates[index];

                    for (int i = viewStates.Count - 1; i > index; i--)
                    {
                        var hv = viewStates[i].holdingView;
                        //设置hideCount 和forceCount的数量
                        if (hv.flags.HasFlag(ViewFlags.HideAllAtStack))
                        {
                            hideCount += 1;
                            forceCount += hv.flags.HasFlag(ViewFlags.Ingore_PauseIf_Flags) ? 1 : 0;
                        }
                        else if (i == index + 1 && hv.flags.HasFlag(ViewFlags.HidePreviousAtStack))//如果是目标view的上一个，则判断是否有隐藏前一个的flag
                        {
                            hideCount += 1;
                            forceCount += hv.flags.HasFlag(ViewFlags.Ingore_PauseIf_Flags) ? 1 : 0;
                        }
                        //将hv移出栈，并隐藏
                        viewStates.RemoveAt(i);
                        // var _hv = _instance.m_HoldingViews.TryGetHoldingView(hv.uiid, hv.uniqueID);
                        hv.isStacked = false;
                        _instance.HideView(Get(hv.uiid, hv.uniqueID), hv, false, true, false);
                        //end
                    }
                }
                if (lastViewState.IsView(holdingView))
                {
                    viewStates.Pop();
                    holdingView.isStacked = false;
                    hasStacked = viewStates.TryPeek(out var previousViewState);
                    if (hasStacked)
                    {
                        //取消隐藏所有
                        if (holdingView.flags.HasFlag(ViewFlags.HideAllAtStack))
                        {
                            hideCount += 1;
                            forceCount += lastViewState.flags.HasFlag(ViewFlags.Ingore_PauseIf_Flags) ? 1 : 0;
                            //尝试显示之前的
                            foreach (var _sv in viewStates)
                            {
                                TryResumeOrShowView(_sv, hideCount, forceCount, _sv == previousViewState);
                            }
                        }
                        //取消隐藏上一个
                        else if (holdingView.flags.HasFlag(ViewFlags.HidePreviousAtStack))
                        {
                            hideCount += 1;
                            forceCount += lastViewState.flags.HasFlag(ViewFlags.Ingore_PauseIf_Flags) ? 1 : 0;
                            TryResumeOrShowView(previousViewState, hideCount, forceCount, true);
                        }
                        //单纯把上一个resume
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
                        var v = GetOrCreate(state.uiid, state.uniqueID);
                        if (v.IsShowing() && resume)
                        {
                            _instance.ResumeView(v);
                        }
                        else
                        {
                            _instance.ShowView(v, false, !resume);
                        }
                    }
                }
            }
        }

        Dictionary<string, UIStack> m_ViewStacks;

        // //更新Stack，排除无效的HoldingView，维护stack安全
        // void UpdateStacks(){

        // }

        void InitViewStacked()
        {
            m_ViewStacks = new Dictionary<string, UIStack>();
            m_ViewStacks.Add(UIStack.DEFAULT_STACK_ID, new UIStack(UIStack.DEFAULT_STACK_ID));
        }

        void PushView(HoldingView view, bool hideStackBehind)
        {
            m_ViewStacks[UIStack.DEFAULT_STACK_ID].PushBeforeShow(view, hideStackBehind);
        }

        void PopView(HoldingView view, bool hideStackBehind)
        {
            m_ViewStacks[UIStack.DEFAULT_STACK_ID].PopAfterHide(view, hideStackBehind);
        }

        public bool IsViewInStack(IUIView view)
        {
            var holdingView = m_HoldingViews.TryGetHoldingView(view);
            if (holdingView)
                return holdingView.isStacked;
            else
                return false;
            // int i = m_ViewGroups[UIStack.DEFAULT_STACK_ID].viewStates.FindIndex(state => state.uiid == view.GetUIID());
            // return i>= 0;
        }

    }

}