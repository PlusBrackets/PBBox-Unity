/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.19
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;

namespace PBBox.View
{
    [SingletonPriority(-1)]
    internal class ViewManager : IViewManager
    {
        private Dictionary<string, IController> m_Controllers = new Dictionary<string, IController>();

        

        public IController Close(string id)
        {
            throw new System.NotImplementedException();
        }

        public IController Get(string id)
        {
            throw new System.NotImplementedException();
        }

        public IController Open(string id)
        {
            throw new System.NotImplementedException();
        }

        public IController Pause(string id)
        {
            throw new System.NotImplementedException();
        }

        public IController Resume(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}