/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.31
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_ODIN
using Sirenix.OdinInspector;
#endif

namespace PBBox.UI
{

    public sealed partial class UIViews : SingleClass<UIViews>
    {
        [System.Flags]
        public enum ViewFlags
        {
            None = 0,
            /// <summary>
            /// hide后会destroy掉，destroy的时间取决于OnViewHide返回的float(sec)
            /// </summary>
            DestroyOnHided = 1 << 0,

            #region Stack 相关
            /// <summary>
            /// 入栈管理
            /// </summary>
            Stacked = 1 << 1,
            /// <summary>
            /// 入栈时关闭上一个入栈的view
            /// </summary>
            HidePreviousAtStack = 1 << 2,
            /// <summary>
            /// 入栈时关闭之前所有入栈的view
            /// </summary>
            HideAllAtStack = 1 << 3,
            /// <summary>
            /// 当后续入栈的view中有Hide At Stack标志时仅进入暂停状态
            /// </summary>
            PauseIfHideAtStack = 1 << 4,
            /// <summary>
            /// 入栈时忽略之前的view的仅暂停标志
            /// </summary>
            Ingore_PauseIf_Flags = 1 << 5,
            #endregion
        }

        [System.Serializable]
        public class ViewConfigure
        {
            [Tooltip("若为空，则默认取ID为 Canvas 的canvas")]
            public string canvasID;
            [Tooltip("若为空或者container不存在，则默认取canvas默认组")]
            public string containerName;
#if USE_ODIN
            [OnValueChanged("_OnFlagChanged")]
#endif
            [SerializeField]
            ViewFlags m_UiFlags;
            public ViewFlags uiFlags => m_UiFlags;
            public bool? isPausing { get; internal set; } = null;
            public bool? isShowing { get; internal set; } = null;

            public ViewConfigure(string canvasID = null, string containerName = null, ViewFlags flags = ViewFlags.None)
            {
                this.canvasID = canvasID;
                this.containerName = containerName;
                m_UiFlags = flags;
            }

#if USE_ODIN
            void _OnFlagChanged()
            {
                if (m_UiFlags.HasFlag(ViewFlags.HidePreviousAtStack) || m_UiFlags.HasFlag(ViewFlags.HideAllAtStack))
                {
                    m_UiFlags = m_UiFlags | ViewFlags.Stacked;
                }
            }
#endif
        }

    }

}