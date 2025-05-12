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
    public interface IView
    {
        IController GetController();
        void Init(IController controller);
        void OnShow();
        void OnHide();
    }
}
