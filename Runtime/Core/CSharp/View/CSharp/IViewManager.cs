/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.30
 *@author: PlusBrackets
 --------------------------------------------------------*/

using System;

namespace PBBox.View
{
    public interface IViewManager : ISingleton<IViewManager>
    {
        void RegisterControllerFactory(string id, Type controllerType, IViewFactory viewFactory, Func<string, IViewFactory, IController> factory = null);
        void UnregisterControllerFactory(string id);
        void RegisterController(IController controller);
        void UnregisterController(string id);

        IController Get(string id);

        IController Open(string id)
        {
            var controller = Get(id);
            controller?.Open();
            return controller;
        }

        IController Close(string id)
        {
            var controller = Get(id);
            controller?.Close();
            return controller;
        }

        IController Resume(string id)
        {
            var controller = Get(id);
            controller?.Resume();
            return controller;
        }

        IController Pause(string id)
        {
            var controller = Get(id);
            controller?.Pause();
            return controller;
        }
    }
}