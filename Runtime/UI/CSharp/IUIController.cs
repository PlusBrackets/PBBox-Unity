/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.30
 *@author: PlusBrackets
 --------------------------------------------------------*/

namespace PBBox.UI2
{

    public interface IUIController
    {
        string ID { get; }
        void Init(string id);
        IUIView GetView();
        T GetView<T>() where T : IUIView;
        void Show();
        void Hide();
        void Resume();
        void Pause();
        void PreloadView();
    }
}