using System;
using System.Threading.Tasks;

namespace PBBox.View
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public abstract class BindViewAttributeBase : Attribute, IViewFactory
    {
        public string Id{ get; private set; }

        public BindViewAttributeBase(string id)
        {
            this.Id = id;
        }

        public abstract IView CreateView(IController controller);
        public abstract Task<IView> CreateViewAsync(IController controller);
        public abstract void ReleaseView(IController controller);
    }
}