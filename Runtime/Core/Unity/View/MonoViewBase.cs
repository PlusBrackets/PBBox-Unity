using UnityEngine;

namespace PBBox.View
{
    /// <summary>
    /// 基础的MonoBehaviour视图类，继承自MonoBehaviour并实现IView接口。
    /// </summary>
    public class MonoViewBase : MonoBehaviour, IView
    {
        protected IController m_Controller;

        public string ID => ((IView)this).Controller?.ID;
        public ViewState State => ((IView)this).Controller?.State ?? ViewState.Closed;
        public IController Controller
        {
            get => m_Controller;
            set
            {
                if (value != null && m_Controller != null && m_Controller != value)
                {
                    Log.Error($"{gameObject.name} already has a controller assigned: {m_Controller.ID}", this, nameof(MonoViewBase), Log.PBBoxLoggerName);
                    return;
                }
                m_Controller = value;
            }
        }

        public bool IsVaild()
        {
            return this != null && m_Controller != null;
        }

        void IView.OnClose()
        {
            gameObject.SetActive(false);
        }

        void IView.OnOpen()
        {
            gameObject.SetActive(true);
        }

        void IView.OnPause()
        {

        }

        void IView.OnResume()
        {

        }

        /// <summary>
        /// 关闭自身
        /// </summary>
        public virtual void CloseSelf()
        {
            if (m_Controller != null)
            {
                m_Controller.Close();
            }
            else
            {
                Log.Warning("Controller is null.", this, nameof(MonoViewBase), Log.PBBoxLoggerName);
            }
        }
    }
}