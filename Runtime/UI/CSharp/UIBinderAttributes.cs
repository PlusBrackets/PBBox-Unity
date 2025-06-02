using System;
using System.Threading.Tasks;

namespace PBBox.UI2
{
    //[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public abstract class BindUIAttributeBase : Attribute
    {
        protected string m_Id;

        public BindUIAttributeBase(string uiid)
        {
            this.m_Id = uiid;
        }

    }
    
}