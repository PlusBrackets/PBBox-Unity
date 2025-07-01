using PBBox.View;
using UnityEngine;

namespace PBBox.UI
{

    public class UIView2 : MonoViewBase, IView
    {
        [SerializeField, Tooltip("UIRoot的ID，若为空则取默认的UIRootID，场景中需要配置UIRoot")]
        private string m_UIRootId = null;
        [SerializeField, Tooltip("处于该UIRoot下的哪个容器，为空则取默认容器")]
        private string m_ContainerId = null;

        public string UIRootId => string.IsNullOrEmpty(m_UIRootId) ? UIRoot.DefaultRootId : m_UIRootId;
        public string ContainerId => m_ContainerId;

        private void Awake()
        {
            var uiRoot = UIRoot.Get(m_UIRootId);
            var container = uiRoot.GetContainer(ContainerId);
            transform.SetParent(container.transform, false);
            if (State == ViewState.Closed)
            {
                gameObject.SetActive(false);
            }
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


    }
}