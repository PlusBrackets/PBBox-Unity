/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.UI
{
    /// <summary>
    /// UIViewCtrl 基类,需要添加[BindUIView(uiid)]特性来绑定UIView,对应UIView或是继承UIView的脚本
    /// </summary>
    public abstract class UIViewController : BaseUIViewController
    {
        public new UIView view { get; private set; }

        protected override void OnViewCreate(IUIView _view)
        {
            base.OnViewCreate(_view);
            view = _view as UIView;
            if (view == null)
            {
                Log.Error($"View[{base.view.GetUIID()}] 并非UIView类型，请继承BaseUIViewCtrl而不是UIViewCtrl", "UIViews", Log.PBBoxLoggerName);
            }
            OnViewCreate(view);
            view.onShowEvent.AddListener(_OnViewShow);
            view.onResumeEvent.AddListener(_OnViewResume);
            view.onPauseEvent.AddListener(_OnViewPause);
            view.onHideEvent.AddListener(_OnViewHide);
        }

        protected override void OnViewDestroy(IUIView _view)
        {
            base.OnViewDestroy(_view);
            view.onShowEvent.RemoveListener(_OnViewShow);
            view.onResumeEvent.RemoveListener(_OnViewResume);
            view.onPauseEvent.RemoveListener(_OnViewPause);
            view.onHideEvent.RemoveListener(_OnViewHide);
            OnViewDestroy(view);
        }

        void _OnViewShow(BaseUIView v)=>OnViewShow(v as UIView);
        void _OnViewResume(BaseUIView v)=>OnViewResume(v as UIView);
        void _OnViewPause(BaseUIView v)=>OnViewPause(v as UIView);
        void _OnViewHide(BaseUIView v)=>OnViewHide(v as UIView);
        
        protected virtual void OnViewCreate(UIView view){}
        protected virtual void OnViewDestroy(UIView view){}
        protected virtual void OnViewShow(UIView view){}
        protected virtual void OnViewResume(UIView view){}
        protected virtual void OnViewPause(UIView view){}
        protected virtual void OnViewHide(UIView view){}
    }
}