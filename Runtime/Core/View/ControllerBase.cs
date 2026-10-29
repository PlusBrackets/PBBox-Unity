/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.19
 *@author: PlusBrackets
 --------------------------------------------------------*/

using System;

namespace PBBox.View
{

    public abstract class ControllerBase : IController
    {
        public string ID { get; private set; }

        public ViewState State { get; protected set; } = ViewState.Closed;

        protected IView m_View;
        protected IViewFactory m_ViewLoader;

        public event Action<IController, ViewState> OnStateChanged;

        public ControllerBase(string id, IViewFactory viewLoader)
        {
            ID = id;
            m_ViewLoader = viewLoader;
        }

        public virtual bool HasView()
        {
            return m_View != null && m_View.IsVaild();
        }
        public virtual IView GetView()
        {
            if (!HasView())
            {
                Close();
                m_View = m_ViewLoader.CreateView(this);
            }
            return m_View;
        }

        protected virtual void OnOpen(){}
        protected virtual void OnClose(){}
        protected virtual void OnResume(){}
        protected virtual void OnPause(){}

        public virtual void Open()
        {
            GetView();
            if (State != ViewState.Closed)
            {
                return;
            }
            if (HasView())
            {
                var oldState = State;
                State = ViewState.Active;
                OnOpen();
                m_View.OnOpen();
                NotifyStateChanged(oldState);
            }
        }

        public virtual void Close()
        {
            if (State == ViewState.Closed)
            {
                return;
            }
            var oldState = State;
            State = ViewState.Closed;
            OnClose();
            if (HasView())
            {
                m_View.OnClose();
            }
            NotifyStateChanged(oldState);
        }

        public virtual void Resume()
        {
            if (State != ViewState.Paused)
            {
                return;
            }
            var oldState = State;
            State = ViewState.Active;
            OnResume();
            if (HasView())
            {
                m_View.OnResume();
            }
            NotifyStateChanged(oldState);
        }
        public virtual void Pause()
        {
            if (State != ViewState.Active)
            {
                return;
            }
            var oldState = State;
            State = ViewState.Paused;
            OnPause();
            if (HasView())
            {
                m_View.OnPause();
            }
            NotifyStateChanged(oldState);
        }

        public virtual void PreloadView()
        {
            m_ViewLoader.PreloadView(this);
        }

        public virtual void ReleaseView()
        {
            if (HasView())
            {
                if (State != ViewState.Closed)
                {
                    Log.Warning($"Releasing view {ID} while it is not closed. Closing it first.", "UI", Log.PBBoxLoggerName);
                    Close();
                }
                m_ViewLoader.ReleaseView(this);
                m_View = null;
            }
        }

        protected void NotifyStateChanged(ViewState oldState)
        {
            OnStateChanged?.Invoke(this, oldState);
            //TODO 触发PBBox通用事件
            //IEventManager.Instance.Emit("event_view_state_changed", this, oldState);
        }
    }
}