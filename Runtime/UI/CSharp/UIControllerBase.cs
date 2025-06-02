namespace PBBox.UI2
{
    public abstract class UIControllerBase : IUIController
    {
        public string ID { get; private set; }
        protected IUIView m_View { get; private set; }


        public IUIView GetView()
        {
            throw new System.NotImplementedException();
        }

        public T GetView<T>() where T : IUIView
        {
            throw new System.NotImplementedException();
        }

        public void Hide()
        {
            throw new System.NotImplementedException();
        }

        public void Init(string id)
        {
            throw new System.NotImplementedException();
        }

        public void Pause()
        {
            throw new System.NotImplementedException();
        }

        public void PreloadView()
        {
            throw new System.NotImplementedException();
        }

        public void Resume()
        {
            throw new System.NotImplementedException();
        }

        public void Show()
        {
            throw new System.NotImplementedException();
        }
    }
}