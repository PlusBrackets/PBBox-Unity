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
        //void RegisterController(string id, IController controller);

        IController Get(string id);
        IController Open(string id);
        IController Close(string id);
        IController Resume(string id);
        IController Pause(string id);
    }
}