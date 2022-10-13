using System.Linq;
/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.21
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.UI
{

    public sealed partial class UIViews : SingleClass<UIViews>
    {
        public const string DEFAULT_CANVAS_ID = "Canvas";
        Dictionary<string, UIViewCanvas> m_ViewCanvas;

        void InitViewCanvas()
        {
            m_ViewCanvas = new Dictionary<string, UIViewCanvas>();
        }

        public static bool RegisterCnavas(UIViewCanvas c)
        {
            var viewCanvas = Instance.m_ViewCanvas;
            if (!viewCanvas.TryAdd(c.canvasID, c))
            {
                DebugUtils.LogError($"[{Instance.GetType().Name}] Canvas字典中已存在ID为[{c.canvasID}]的Canvas");
                return false;
            }
            return true;
        }

        public static bool UnregisterCanvas(UIViewCanvas c)
        {
            if (!HasInstance) return false;
            var viewCanvas = Instance.m_ViewCanvas;
            if (viewCanvas.ContainsValue(c))
            {
                UnregisterCanvas(c.canvasID);
                viewCanvas.Remove(c.canvasID);
                return true;
            }
            return false;
        }

        public static void UnregisterCanvas(string cid)
        {
            if (!HasInstance) return;
            Instance.m_ViewCanvas.Remove(cid);
            // var allInCanvasView = Instance.m_HoldingViews.Views.Where(v => v.canvasID == cid);
            // foreach(var v in allInCanvasView){
            //     Instance.m_HoldingViews.RemoveHoldingView(v);
            // }
        }

        public static UIViewCanvas GetViewCanvas(string canvasID = null)
        {
            if (string.IsNullOrEmpty(canvasID))
            {
                canvasID = DEFAULT_CANVAS_ID;
            }
            UIViewCanvas uiCanvas = null;
            Instance.m_ViewCanvas.TryGetValue(canvasID,out uiCanvas);
            return uiCanvas;
        }

        public static GameObject GetViewContainer(string canvasID, string containerName){
            UIViewCanvas viewCanvas = GetViewCanvas(canvasID);
            if(viewCanvas == null) return null;
            return viewCanvas.GetContainer(containerName);
        }

    }

}