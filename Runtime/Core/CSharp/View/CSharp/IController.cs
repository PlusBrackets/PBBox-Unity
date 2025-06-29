/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.05.30
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Threading.Tasks;

namespace PBBox.View
{
    /// <summary>
    /// 控制器接口
    /// </summary>
    public interface IController
    {
        string ID { get; }
        /// <summary>
        /// 当前视图状态
        /// </summary>
        ViewState State { get; }
        /// <summary>
        /// 事件：当视图状态发生变化时触发
        /// <param name="controller">触发事件的控制器</param>
        /// <param name="state">旧视图状态</param>
        /// </summary>
        public event Action<IController, ViewState> OnStateChanged;

        bool HasView();
        IView GetView();
        void Open();
        void Close();
        void Resume();
        void Pause();
        void PreloadView();
        void ReleaseView();
    }
}