/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.31
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_ODIN
using Sirenix.OdinInspector;
#endif

namespace PBBox.UI
{

    public sealed partial class UIViews : SingleClass<UIViews>
    {
        [Tooltip("标识View的一些特征")]
        [System.Flags]
        public enum ViewFlags
        {
            None = 0,
            /// <summary>
            /// hide后会destroy掉，destroy的时间取决于OnViewHide返回的float(sec)
            /// </summary>
            DestroyOnHided = 1 << 0,

            #region Stack 相关
            /// <summary>
            /// 入栈管理
            /// </summary>
            Stacked = 1 << 1,
            /// <summary>
            /// 入栈时关闭上一个入栈的view
            /// </summary>
            HidePreviousAtStack = 1 << 2,
            /// <summary>
            /// 入栈时关闭之前所有入栈的view
            /// </summary>
            HideAllAtStack = 1 << 3,
            /// <summary>
            /// 当后续入栈的view中有Hide At Stack标志时仅进入暂停状态
            /// </summary>
            PauseIfHideAtStack = 1 << 4,
            /// <summary>
            /// 入栈时忽略之前的view的仅暂停标志
            /// </summary>
            Ingore_PauseIf_Flags = 1 << 5,
            /// <summary>
            /// 在入栈后被隐藏时忽略DestroyOnHide的标志
            /// </summary>
            DontDestroyOnStack = 1 << 6
            #endregion
        }

        class HoldingViewGroup
        {
            /// <summary>
            /// 当前已创建的View (Key:uniqueID)
            /// </summary>
            Dictionary<int, IUIView> m_InstantiatedViews;
            //正在保持的views，保持中的view包括正在显示中的和正在入栈中的View。
            Dictionary<int, HoldingView> m_Views; 
            //id查找表 (uiid, list - uniqueID)
            Dictionary<string, List<string>> m_UIDLookup;

            public Dictionary<int, HoldingView> views =>m_Views;

            public HoldingViewGroup()
            {
                m_InstantiatedViews = new Dictionary<int, IUIView>();
                m_Views = new Dictionary<int, HoldingView>();
                m_UIDLookup = new Dictionary<string, List<string>>();
            }

            /// <summary>
            /// 尝试添加
            /// 同时会在uidLookup列表中添加uniqueID,并且将实例化的View缓存
            /// </summary>
            /// <param name="view"></param>
            /// <returns></returns>
            public void SetHoldingView(IUIView view)
            {
                HoldingView holdingView = new HoldingView(view);
                if (m_Views.TryAdd(holdingView.GetHashCode(), holdingView))
                {
                    if (!m_UIDLookup.TryGetValue(view.GetUIID(), out var ids))
                    {
                        ids = new List<string>();
                        m_UIDLookup.Add(view.GetUIID(), ids);
                    }
                    ids.Add(view.uniqueID);
                }
                else
                {
                    var _hv = TryGetHoldingView(view);
                    if (_hv)
                    {
                        _hv.SetView(view);
                    }
                    else
                    {
                        m_Views[holdingView.GetHashCode()] = _hv;
                    }
                }
                if (holdingView)
                    m_InstantiatedViews[holdingView.GetHashCode()] = view;
            }

            public bool TryToRemoveHoldingView(string uiid, string uniqueID, IUIView view, bool removeInstantiatedViewIfFail = true)
            {
                bool removed = false;
                var hv = TryGetHoldingViewOrLast(uiid, uniqueID);
                if (hv && hv.canBeRemoveWhenUIDestroy)
                {
                    removed = RemoveHoldingView(hv);
                }
                if (!removed && removeInstantiatedViewIfFail)
                {
                    RemoveInstantiatedViewOnly(view);
                }
                return removed;
            }

            // public bool RemoveHoldingView(string uiid,string uniqueID){
            //     return RemoveHoldingView(TryGetHoldingView(uiid,uniqueID));
            // }

            // public bool RemoveHoldingView(IUIView view)
            // {
            //     return RemoveHoldingView(view.GetUIID(), view.uniqueID);//, view);
            // }

            public bool RemoveHoldingView(HoldingView view)//, IUIView view = null)
            {
                // if (view != null)
                // {
                //     m_InstantiatedViews.TryGetValue(key, out var temp);
                //     if (view != temp)
                //         return false;
                // }
                // int key = HoldingView.GetHashCode(uiid, uniqueID);
                int key = view.GetHashCode();
                m_Views.Remove(key);
                List<string> ids = null;
                m_UIDLookup?.TryGetValue(view.uiid, out ids);
                if (ids != null)
                {
                    ids.Remove(view.uniqueID);
                }
                m_InstantiatedViews.Remove(key);
                return true;
            }

            public HoldingView TryGetHoldingView(string uiid, string uniqueID)
            {
                int key = HoldingView.GetHashCode(uiid, uniqueID);
                m_Views.TryGetValue(key, out var v);
                return v;
            }

            public HoldingView TryGetHoldingView(IUIView view)
            {
                return TryGetHoldingView(view.GetUIID(), view.uniqueID);
            }

            public HoldingView TryGetHoldingView(string uiid)
            {
                if (m_UIDLookup.TryGetValue(uiid, out var uniqueIds))
                {
                    if (uniqueIds != null && uniqueIds.TryPeek(out var uniqueID))
                    {
                        return TryGetHoldingView(uiid, uniqueID);
                    }
                }
                return null;
            }

            public HoldingView TryGetHoldingViewOrLast(string uiid, string uniqueID)
            {
                if (uniqueID == null)
                {
                    return TryGetHoldingView(uiid);
                }
                else
                {
                    return TryGetHoldingView(uiid, uniqueID);
                }
            }

            public IUIView GetInstantiatedView(HoldingView holdingView)
            {
                if (holdingView)
                    return GetInstantiatedView(holdingView.uiid, holdingView.uniqueID);
                return null;
            }

            public IUIView GetInstantiatedView(string uiid, string uniqueID)
            {
                m_InstantiatedViews.TryGetValue(HoldingView.GetHashCode(uiid, uniqueID), out var view);
                return view;
            }

            public bool HasInstantiatedView(string uiid, string uniqueID)
            {
                return m_InstantiatedViews.ContainsKey(HoldingView.GetHashCode(uiid, uniqueID));
            }

            public void RemoveInstantiatedViewOnly(IUIView view)
            {
                if (view == null)
                    return;
                var key = HoldingView.GetHashCode(view);
                m_InstantiatedViews.Remove(key, out var temp);
                // m_InstantiatedViews.TryGetValue(key, out var temp);
                // if (view == temp)
                // {
                //     m_InstantiatedViews.Remove(key);
                // }
            }
        }

        class HoldingView
        {
            public string uiid { get; private set; }
            public string uniqueID { get; private set; }
            public ViewFlags flags { get; private set; }
            // public UIViewState state;
            public string canvasID { get; private set; }
            public string containerName { get; private set; }
            public bool isStacked { get; internal set; }
            public bool canBeRemoveWhenUIDestroy => !isStacked;
            public MutiTags tags{get; internal set;}

            public HoldingView(string uiid, string uniqueID)
            {
                this.uiid = uiid;
                this.uniqueID = uniqueID;
                flags = ViewFlags.None;
                canvasID = null;
                containerName = null;
                isStacked = false;
                tags = default;
            }

            public HoldingView(IUIView view)
            {
                uiid = view.GetUIID();
                uniqueID = view.uniqueID;
                flags = view.configure.uiFlags;
                canvasID = view.configure.canvasID;
                containerName = view.configure.containerName;
                isStacked = false;
                tags = view.configure.viewTags;
            }

            public void SetView(IUIView view)
            {
                if (!IsView(view))
                    return;
                view.configure.m_UiFlags = flags;
                view.configure.canvasID = canvasID;
                view.configure.containerName = containerName;
                view.configure.viewTags = tags;
            }

            public bool IsView(IUIView view){
                return view!=null&&uiid == view.GetUIID() && uniqueID == view.uniqueID;
            }
            
            public static implicit operator bool(HoldingView exists)
            {
                return exists != null && !string.IsNullOrEmpty(exists.uiid) && !string.IsNullOrEmpty(exists.uniqueID);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(uiid, uniqueID);
            }

            public static int GetHashCode(IUIView view)
            {
                return GetHashCode(view.GetUIID(), view.uniqueID);
            }

            public static int GetHashCode(string uiid, string uniqueID)
            {
                return HashCode.Combine(uiid, uniqueID);
            }

            public override bool Equals(object obj)
            {
                return obj is HoldingView _obj &&
                        _obj.uiid == uiid &&
                        _obj.uniqueID == uniqueID;
            }
        }

        [System.Serializable]
        public class ViewConfigure
        {
            [Tooltip("若为空，则默认取ID为 Canvas 的canvas")]
            public string canvasID;
            [Tooltip("若为空或者container不存在，则默认取canvas默认组")]
            public string containerName;
#if USE_ODIN
            [OnValueChanged("_OnFlagChanged")]
#endif
            [SerializeField]
            internal ViewFlags m_UiFlags;
            public ViewFlags uiFlags => m_UiFlags;
            public MutiTags viewTags;
            public bool? isPausing { get; internal set; } = null;
            public bool? isShowing { get; internal set; } = null;

            public ViewConfigure(string canvasID = null, string containerName = null, ViewFlags flags = ViewFlags.None)
            {
                this.canvasID = canvasID;
                this.containerName = containerName;
                m_UiFlags = flags;
            }

#if USE_ODIN
            void _OnFlagChanged()
            {
                if (m_UiFlags.HasFlag(ViewFlags.HidePreviousAtStack) || m_UiFlags.HasFlag(ViewFlags.HideAllAtStack))
                {
                    m_UiFlags = m_UiFlags | ViewFlags.Stacked;
                }
            }
#endif
        }

    }

}