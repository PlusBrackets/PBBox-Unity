/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
using UnityEngine.Events;
using System;

namespace PBBox.UI
{
    public abstract class BaseUIViewModel : BaseDataModel, IUIViewModel
    {
    }

    /// <summary>
    /// UIViewCtrl 基类,需要添加[BindUIView(uiid)]特性来绑定UIView
    /// </summary>
    public abstract class BaseUIViewController : IUIViewController
    {
        private IUIView m_View;
        protected bool m_IsDisposed = false;
        public IUIView view { get => m_View; }

        void IUIViewController.OnViewCreate(IUIView view)
        {
            m_View = view;
            OnViewCreate(view);
        }

        void IUIViewController.OnViewDestroy(IUIView view)
        {
            OnViewDestroy(view);
            m_View = null;
        }

        void IUIViewController.Release()
        {
            if (m_IsDisposed)
                return;
            m_IsDisposed = true;
            OnRelease();
        }

        protected virtual void OnViewCreate(IUIView view){}
        protected virtual void OnViewDestroy(IUIView view){}
        protected virtual void OnRelease(){}
    }
}