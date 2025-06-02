/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.30
 *@author: PlusBrackets
 --------------------------------------------------------*/

using System;

namespace PBBox.UI2
{
    public interface IUIManager
    {
        IUIController Get(string uiid);
        T Get<T>(string uiid) where T : IUIController;
        IUIController Show(string uiid);
        IUIController Hide(string uiid);
        IUIController Resume(string uiid);
        IUIController Pause(string uiid);
    }
}