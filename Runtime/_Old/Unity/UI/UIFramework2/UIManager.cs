using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.UI2
{

    public class UIManager : Singleton<UIManager>
    {
        public static IController GetController(string uiid){
            return null;
        }

        public static void Show(string uiid)
        {
            GetController(uiid)?.ShowView();
        }

        public static void Hide(string uiid)
        {
            GetController(uiid)?.HideView();
        }
    }
}