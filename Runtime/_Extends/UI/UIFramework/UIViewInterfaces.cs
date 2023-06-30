/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.31
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
using System;

namespace PBBox.UI
{
    [Flags]
    public enum UIViewState
    {
        None,
        Show = 1,
        Resume = 1 << 2,
        Pause = 1 << 3,
        Hide = 1 << 4
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class BindUIViewAttribute : Attribute
    {
        public string uiid;
        // public bool dontAutoDispose = false;

        /// <summary>
        /// 对ViewCtrl进行绑定,可以绑定多个uiid
        /// </summary>
        /// <param name="uiid">绑定的UIID</param>
        /// <param name="dontAutoDispose">不要在View销毁时销毁Ctrl,此时需要手动销毁</param>
        public BindUIViewAttribute(string uiid,bool dontAutoDispose = false)
        {
            this.uiid = uiid;
            // this.dontAutoDispose = dontAutoDispose;
        }
    }
    
    /// <summary>
    /// IUIView出入栈状态的回调,可拓展至IUIView或者IUIViewController中
    /// </summary>
    public interface IUIViewOnStackState
    {
        void OnViewPushInStack(IUIView view);
        void OnViewPopFromStack(IUIView view);
    }
    
    /// <summary>
    /// View 控制层
    /// </summary>
    public interface IUIViewController
    {
        IUIView view { get; }
        void OnViewCreate(IUIView view);
        void OnViewDestroy(IUIView view);
        void Release();
    }

    /// <summary>
    /// View 数据层
    /// </summary>
    public interface IUIViewModel : IDataModel
    {
    }

    /// <summary>
    /// View 界面层
    /// </summary>
    public interface IUIView
    {
        UIViews.ViewConfigure configure { get; }
        IUIViewController controller { get; set; }
        string uniqueID { get; set; }
        string GetUIID();
        GameObject GetGameObject();
        void OnViewShow();
        float OnViewHide();
        void OnViewPause();
        void OnViewResume();

    }

    public static class IUIViewExtensions{

        public static UIViewState GetState(this IUIView view){
            var state = UIViewState.None;
            if(view.configure.isShowing.HasValue){
                state |= (view.configure.isShowing.Value ? UIViewState.Show : UIViewState.Hide);
            }
            if(view.configure.isPausing.HasValue){
                state |= (view.configure.isPausing.Value ? UIViewState.Pause : UIViewState.Resume);
            }
            return state;
        }

        public static bool IsShowing(this IUIView view){
            return view.configure.isShowing.HasValue && view.configure.isShowing.Value;
        }

        /// <summary>
        /// view是否正在隐藏中或者未初始化
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static bool IsHidingOrUninitialized(this IUIView view)
        {
            return !view.configure.isShowing.HasValue || !view.configure.isShowing.Value;
        }

        public static T Show<T>(this T view) where T : IUIView
        {
            return UIViews.Show(view);
        }

        public static IUIView Hide(this IUIView view)
        {
            return UIViews.Hide(view);
        }

        public static IUIView Pause(this IUIView view)
        {
            return UIViews.Pause(view);
        }

        public static IUIView Resume(this IUIView view)
        {
            return UIViews.Resume(view);
        }

        public static T GetController<T>(this IUIView view) where T : class, IUIViewController
        {
            return view.controller as T;
        }

        public static T GetModel<T>(this IUIViewController ctrl, bool autoCreate = true) where T : class, IUIViewModel, new()
        {
            return UIViews.GetModel<T>(autoCreate);
        }

        public static T GetModel<T>(this IUIViewController ctrl, string uid, bool autoCreate = true) where T : class, IUIViewModel, new()
        {
            return UIViews.GetModel<T>(uid, autoCreate);
        }
    }
}