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
    public abstract class BaseUIViewModel : IUIViewModel
    {
        private event Action<IUIViewModel> m_OnRefresh, m_OnDispose;
        public Action<IUIViewModel> onRefresh => m_OnRefresh;
        public Action<IUIViewModel> onDispose => m_OnDispose;
        public bool isDisposed { get; private set; }

        protected virtual void OnDispose() { }

        public void Dispose()
        {
            if (isDisposed)
                return;
            isDisposed = true;
            OnDispose();
        }
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

        void IDisposable.Dispose()
        {
            if (m_IsDisposed)
                return;
            m_IsDisposed = true;
            OnDispose();
        }

        protected virtual void OnViewCreate(IUIView view){}
        protected virtual void OnViewDestroy(IUIView view){}
        protected virtual void OnDispose(){}
    }
}