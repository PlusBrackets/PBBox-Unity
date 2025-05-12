/*--------------------------------------------------------
 *Copyright (c) 2016-2024 PlusBrackets
 *@update: 2024.12.07
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using System;

namespace PBBox.UI2
{
    public enum UIState{
        None,
        Show,
        Hide
    }

    public interface IController
    {
        UIState State { get; }
        event Action<IController> OnShowEvent;
        event Action<IController> OnHideEvent;

        IView GetView();
        void Init();
        void ShowView();
        void HideView();
        void Destory();
    }
}