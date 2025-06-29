/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.30
 *@author: PlusBrackets
 --------------------------------------------------------*/

namespace PBBox.View
{
    /// <summary>
    /// 视图接口
    /// </summary>
    public interface IView
    {
        IController GetController();
        ViewState State => GetController().State;
        string ID => GetController().ID;
        
        bool IsVaild();

        void OnOpen();
        void OnClose();
        void OnResume();
        void OnPause();
    }
}