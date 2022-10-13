using System;
/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.25
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PBBox
{
    internal partial class PBBoxSettings
    {
        [Space]
        public UIViewSettings uiViewSettings;
        
        [System.Serializable]
        public class UIViewSettings
        {

            [Tooltip("允许反射绑定viewController的程序集名称,为空则反射全部程序集(耗时)")]
            public string[] reflectAssemblies = new string[] { "Assembly-CSharp" };
        }
    }
}

namespace PBBox.UI
{
    //TODO v0.5版，比较乱，等待重构
    /// <summary>
    /// UIView管理
    /// </summary>
    public sealed partial class UIViews : SingleClass<UIViews>
    {

        protected override void Init()
        {
            base.Init();
            InitLogic();
            InitViewStacked();
            InitViewCanvas();
            InitViewLoader();
            InitMVC();
        }

        /// <summary>
        /// 显示View
        /// </summary>
        /// <param name="uiid"></param>
        /// <param name="uniqueID">如果uniqueID为null则总打开最新创建的</param>
        /// <param name="hideStackBehind">如果view已在栈中且不是顶层，则关闭之后入栈的直到打开该view</param>
        /// <returns></returns>
        public static IUIView Show(string uiid, string uniqueID = null, bool hideStackBehind = true)
        {
            IUIView view = GetOrCreate(uiid, uniqueID);
            return Show(view, hideStackBehind);
        }

        /// <summary>
        /// 显示View
        /// </summary>
        /// <param name="uiid"></param>
        /// <param name="uniqueID">如果uniqueID为null则总打开最新创建的</param>
        /// <param name="hideStackBehind">如果view已在栈中且不是顶层，则关闭之后入栈的直到打开该view</param>
        /// <typeparam name="T"></typeparam>
        public static T Show<T>(string uiid, string uniqueID = null, bool hideStackBehind = true) where T : class, IUIView
        {
            return Show(uiid, uniqueID, hideStackBehind) as T;
        }

        /// <summary>
        /// 显示View
        /// </summary>
        /// <param name="view"></param>
        /// <param name="hideStackBehind">如果view已在栈中且不是顶层，则关闭之后入栈的直到打开该view</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Show<T>(T view, bool hideStackBehind = true) where T : IUIView
        {
            Instance.ShowView(view, true, hideStackBehind: hideStackBehind);
            return view;
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="uiid"></param>
        /// <param name="uniqueID">如果uniqueID为null则总隐藏最新创建的</param>
        /// <param name="hideStackBehind">如果view在栈中则会关闭该view以及之后入栈的</param>
        public static void Hide(string uiid, string uniqueID = null, bool hideStackBehind = true)
        {
            Instance.HideView(Get(uiid, uniqueID), Instance.m_HoldingViews.TryGetHoldingViewOrLast(uiid, uniqueID), true, hideStackBehind: hideStackBehind);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="hideStackBehind">如果view在栈中则会关闭该view以及之后入栈的</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Hide<T>(T view, bool hideStackBehind = true) where T : IUIView
        {
            Instance.HideView(view, Instance.m_HoldingViews.TryGetHoldingView(view), true, hideStackBehind: hideStackBehind);
            return view;
        }

        /// <summary>
        /// 暂停view
        /// </summary>
        /// <param name="uiid">view的基础Id</param>
        /// <param name="uniqueID">view实例的唯一标识</param>
        /// <returns></returns>
        public static IUIView Pause(string uiid, string uniqueID = null)
        {
            IUIView view = Get(uiid, uniqueID);
            return Pause(view);
        }

        /// <summary>
        /// 暂停view
        /// </summary>
        /// <param name="uiid">view的基础Id</param>
        /// <param name="uniqueID">view实例的唯一标识</param>
        /// <typeparam name="T"></typeparam>
        public static T Pause<T>(string uiid, string uniqueID = null) where T : class, IUIView
        {
            return Pause(uiid, uniqueID) as T;
        }

        /// <summary>
        /// 恢复显示中的已暂停的view
        /// </summary>
        /// <param name="uiid">view的基础Id</param>
        /// <param name="uniqueID">view实例的唯一标识</param>
        /// <returns></returns>
        public static IUIView Resume(string uiid, string uniqueID = null)
        {
            IUIView view = Get(uiid, uniqueID);
            return Resume(view);
        }

        /// <summary>
        /// 恢复显示中的已暂停的view
        /// </summary>
        /// <param name="uiid">view的基础Id</param>
        /// <param name="uniqueID">view实例的唯一标识</param>
        /// <typeparam name="T"></typeparam>
        public static T Resume<T>(string uiid, string uniqueID = null) where T : class, IUIView
        {
            return Resume(uiid, uniqueID) as T;
        }


        /// <summary>
        /// 暂停view
        /// </summary>
        /// <param name="view"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Pause<T>(T view) where T : IUIView => Instance.PauseView(view);

        /// <summary>
        /// 恢复显示中的已暂停的view
        /// </summary>
        /// <param name="view"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resume<T>(T view) where T : IUIView => Instance.ResumeView<T>(view);


        /// <summary>
        /// 获取或创建一个view,如果uniqueID为null则总是返回最新holding的View
        /// </summary>
        /// <param name="uiid">view的基础Id</param>
        /// <param name="uniqueID">view实例的唯一标识</param>
        /// <returns></returns>
        public static IUIView GetOrCreate(string uiid, string uniqueID = null)
        {
            IUIView view = Get(uiid, uniqueID);
            if (view == null)
            {
                view = Instance.CreateView(uiid, uniqueID);
            }
            return view;
        }

        /// <summary>
        /// 获取或创建一个view,如果uniqueID为null则总是返回最新holding的View
        /// </summary>
        /// <param name="uiid">view的基础Id</param>
        /// <param name="uniqueID">view实例的唯一标识</param>
        /// <typeparam name="T"></typeparam>
        public static T GetOrCreate<T>(string uiid, string uniqueID = null) where T : class, IUIView
        {
            return GetOrCreate(uiid, uniqueID) as T;
        }

        /// <summary>
        /// 获取实例化中的View,如果uniqueID为null则总是返回最新holding的View
        /// </summary>
        /// <param name="uiid">view的基础Id</param>
        /// <param name="uniqueID">view实例的唯一标识</param>
        /// <returns></returns>
        public static IUIView Get(string uiid, string uniqueID = null) => Instance.GetView(uiid, uniqueID);

        /// <summary>
        /// 获取实例化中的View,如果uniqueID为null则总是返回最新holding的View
        /// </summary>
        /// <param name="uiid">view的基础Id</param>
        /// <param name="uniqueID">view实例的唯一标识</param>
        /// <typeparam name="T"></typeparam>
        public static T Get<T>(string uiid, string uniqueID = null) where T : class, IUIView
        {
            return Get(uiid, uniqueID) as T;
        }

        //TODO 提供手动关闭View的功能，解决场景切换等导致UI直接Destroy的情况时正确处理Stack逻辑
        /// <summary>
        /// 无视flags直接删除view，可用于手动清理被隐藏但没有destroy的view
        /// </summary>
        /// <param name="uiid"></param>
        /// <param name="uniqueID"></param>
        // public static void DisposeView(string uiid, string uniqueID)
        // {

        // }

        /// <summary>
        /// 隐藏所有带tag的View
        /// </summary>
        /// <param name="filter">符合的tag返回true</param>
        /// <param name="hideFirstStack">ture则隐藏最先入栈的UIView并隐藏后续入栈的所有，false则只隐藏最顶层(如果符合tag)</param>
        public static void HideWithTag(System.Func<MutiTags, bool> filter, bool hideFirstStack = true)
        {
            if (!HasInstance) return;
            var self = _instance;
            var views = self.m_HoldingViews.views.Where(v => !v.Value.isStacked && filter(v.Value.tags)).ToArray();
            foreach (var v in views)
            {
                self.HideView(self.GetView(v.Value.uiid, v.Value.uniqueID), v.Value, true, true, true);
            }
            if (hideFirstStack)
            {
                var stackedStates = self.m_ViewStacks[UIStack.DEFAULT_STACK_ID].viewStates;
                HoldingView fsView = stackedStates.Find(s => filter(s.holdingView.tags))?.holdingView;
                if (fsView)
                {
                    self.HideView(self.GetView(fsView.uiid, fsView.uniqueID), fsView, true, true, true);
                }
            }
            else
            {
                if (self.m_ViewStacks[UIStack.DEFAULT_STACK_ID].viewStates.TryPeek(out var view))
                {
                    if (filter(view.holdingView.tags))
                    {
                        self.HideView(self.GetView(view.uiid, view.uniqueID), view.holdingView, true, true, false);
                    }
                }
            }
        }

        /// <summary>
        /// 隐藏所有带tag的View
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="hideFirstStack">ture则隐藏最先入栈的UIView并隐藏后续入栈的所有，false则只隐藏最顶层(如果符合tag)</param>
        public static void HideWithTag(string tag, bool hideFirstStack = true, StringComparison? stringComparison = null)
        {
            HideWithTag(mTags => stringComparison.HasValue ? mTags.HasTag(tag, stringComparison.Value) : mTags.HasTag(tag), hideFirstStack);
        }

    }

}