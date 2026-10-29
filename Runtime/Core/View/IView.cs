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
        IController Controller { get; set; }
        ViewState State => Controller?.State ?? ViewState.Closed;
        string ID => Controller?.ID;

        bool IsVaild();

        void OnOpen();
        void OnClose();
        void OnResume();
        void OnPause();
    }
}