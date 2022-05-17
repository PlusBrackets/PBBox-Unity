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
        Resume = 1<<2,
        Pause = 1<<3,
        Hide = 1<<4
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class BindUIViewAttribute : Attribute
    {
        public string uiid;
        public bool dontAutoDispose = false;

        /// <summary>
        /// 对ViewCtrl进行绑定,可以绑定多个uiid
        /// </summary>
        /// <param name="uiid">绑定的UIID</param>
        /// <param name="dontAutoDispose">不要在View销毁时销毁Ctrl,此时需要手动销毁</param>
        public BindUIViewAttribute(string uiid,bool dontAutoDispose = false)
        {
            this.uiid = uiid;
            this.dontAutoDispose = dontAutoDispose;
        }
    }
    
    /// <summary>
    /// View 控制层
    /// </summary>
    public interface IUIViewController : IDisposable
    {
        IUIView view { get; }
        void OnViewCreate(IUIView view);
        void OnViewDestroy(IUIView view);
    }

    /// <summary>
    /// View 数据层
    /// </summary>
    public interface IUIViewModel : IDisposable
    {
        Action<IUIViewModel> onDispose { get; }
        Action<IUIViewModel> onRefresh { get; }
    }

    /// <summary>
    /// View 界面层
    /// </summary>
    public interface IUIView
    {
        UIViews.ViewConfigure configure { get; }
        IUIViewController controller { get; set; }
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

        public static void Refresh(this IUIViewModel model){
            model.onRefresh?.Invoke(model);
        }

        public static T GetModel<T>(this IUIViewController ctrl, bool autoCreate = true) where T : class, IUIViewModel, new()
        {
            return UIViews.GetModel<T>(autoCreate);
        }

        public static T GetModel<T>(this IUIViewController ctrl, string uid, bool autoCreate = true) where T : class, IUIViewModel, new()
        {
            return UIViews.GetModel<T>(uid, autoCreate);
        }

        public static void DisposeModel<T>(this IUIViewController ctrl, string uid = null) where T : IUIViewModel
        {
            UIViews.DisposeModel<T>(uid);
        }
    }
}