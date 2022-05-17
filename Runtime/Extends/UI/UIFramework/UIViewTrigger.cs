using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.UI
{
    [AddComponentMenu("PBBox/UI/Framework/UI View Trigger")]
    /// <summary>
    /// 提供简单的显示、关闭UI的触发器
    /// </summary>
    public sealed class UIViewTrigger : MonoBehaviour
    {
        public void ShowUIView(string uiid){
            UIViews.Show(uiid);
        }

        public void HideUIView(string uiid){
            UIViews.Hide(uiid);
        }

        public void PauseUIView(string uiid){
            UIViews.Pause(uiid);
        }

        public void ResumeUIView(string uiid){
            UIViews.Resume(uiid);
        }

        public void HideSelf(){
            GetComponent<IUIView>()?.Hide();
        }
    }
}