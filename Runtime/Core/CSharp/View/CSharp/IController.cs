/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.30
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Threading.Tasks;

namespace PBBox.View
{
    /// <summary>
    /// 控制器接口
    /// </summary>
    public interface IController
    {
        string ID { get; }

        bool HasView();
        IView GetView();
        void Open();
        void Close();
        void Resume();
        void Pause();
        void PreloadView();
    }
}