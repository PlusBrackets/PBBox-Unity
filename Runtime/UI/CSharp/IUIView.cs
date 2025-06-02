/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.30
 *@author: PlusBrackets
 --------------------------------------------------------*/

namespace PBBox.UI2
{

    public interface IUIView
    {
        IUIController GetController();
        string ID { get; }
        void OnShow();
        void OnHide();
        void OnResume();
        void OnPause();
    }
}