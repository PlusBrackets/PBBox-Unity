/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.19
 *@author: PlusBrackets
 --------------------------------------------------------*/
 
namespace PBBox.View
{

    public abstract class ControllerBase : IController
    {
        public string ID { get; private set; }
        protected IViewFactory m_ViewLoader;

        public ControllerBase(string id, IViewFactory viewLoader)
        {
            ID = id;
            m_ViewLoader = viewLoader;
        }

        public abstract bool HasView();
        public abstract IView GetView();

        public virtual void Open() { }
        public virtual void Close() { }
        public virtual void Resume() { }
        public virtual void Pause() { }
        public virtual void PreloadView() { }
    }
}