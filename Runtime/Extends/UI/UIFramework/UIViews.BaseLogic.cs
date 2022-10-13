/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PBBox.UI
{
    /// <summary>
    /// UIView管理
    /// </summary>
    public sealed partial class UIViews : SingleClass<UIViews>
    {
        //主逻辑

        Dictionary<string, IUIView> m_ViewPrefabs;
        HoldingViewGroup m_HoldingViews;

        void InitLogic()
        {
            m_ViewPrefabs = new Dictionary<string, IUIView>();
            m_HoldingViews = new HoldingViewGroup();
        }

        /// <summary>
        /// 暂停view
        /// </summary>
        /// <param name="view"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T PauseView<T>(T view) where T : IUIView
        {
            if (view != null && (!view.configure.isPausing.HasValue || view.configure.isPausing.Value == false))
            {
                view.configure.isPausing = true;
                view.OnViewPause();
            }
            return view;
        }

        /// <summary>
        /// 恢复显示中的已暂停的view
        /// </summary>
        /// <param name="view"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T ResumeView<T>(T view) where T : IUIView
        {
            if (view != null && view.IsShowing() && (!view.configure.isPausing.HasValue || view.configure.isPausing.Value == true))
            {
                view.configure.isPausing = false;
                view.OnViewResume();
            }
            return view;
        }

        /// <summary>
        /// 创建一个view
        /// </summary>
        /// <param name="uiid">view的基础Id</param>
        /// <param name="uniqueID">view实例的唯一标识</param>
        /// <returns></returns>
        IUIView CreateView(string uiid, string uniqueID)
        {
            m_ViewPrefabs.TryGetValue(uiid, out var view);
            if (view == null)
            {
                view = LoadSync(uiid);
            }
            if (view != null)
            {
                //实例化view
                view = InstantiateView(view, uniqueID);
            }
            return view;
        }

        /// <summary>
        /// 获取实例化中的View,如果uniqueID==null则总是回去最新holding的View
        /// </summary>
        /// <param name="uiid">view的基础Id</param>
        /// <param name="uniqueID">view实例的唯一标识</param>
        /// <returns></returns>
        IUIView GetView(string uiid, string uniqueID)
        {
            var holdingView = m_HoldingViews.TryGetHoldingViewOrLast(uiid, uniqueID);
            var view = m_HoldingViews.GetInstantiatedView(holdingView);
            return view;
        }

        /// <summary>
        /// 显示View
        /// </summary>
        /// <param name="view"></param>
        /// <param name="pushOption">是否有入栈操作</param>
        /// <param name="pauseView">是否更改是否直接暂停view</param>
        void ShowView(IUIView view, bool pushOption, bool pauseView = false, bool hideStackBehind = false)
        {
            if (view == null)
                return;
            var holdingView = m_HoldingViews.TryGetHoldingView(view);
            if (!holdingView)
                return;
            if (view.IsHidingOrUninitialized())
            {
                if (view.configure.uiFlags.HasFlag(ViewFlags.Stacked) && pushOption)
                {
                    PushView(holdingView, hideStackBehind);
                }

                view.configure.isShowing = true;
                view.OnViewShow();

                if (!pauseView || !view.configure.isPausing.HasValue /*为保证新建的View有完整的生命周期*/)
                {
                    ResumeView(view);
                }
                else
                {
                    PauseView(view);
                }
            }
        }

        // void HideView(string uiid, string uniqueID, bool popOption, bool destroyOption = true)
        // {
        //     HideView(Get(uiid, uniqueID), m_HoldingViews.TryGetHoldingView(uiid, uniqueID), popOption, destroyOption);
        // }

        // void HideView(IUIView view, bool popOption, bool destroyOption = true) => HideView(view, view?.GetUIID(), view?.uniqueID, popOption, destroyOption);

        /// <summary>
        /// 隐藏View
        /// </summary>
        /// <param name="view"></param>
        /// <param name="popOption">出栈操作是否执行</param>
        /// <param name="pasueView">是否暂停View</param>
        /// <param name="destroyOption">销毁操作是否执行</param>
        /// <returns></returns>
        void HideView(IUIView view, HoldingView holdingView, bool popOption, bool destroyOption = true, bool hideStackBehind = false)
        {
            float hideDur = 0f;
            if (view != null && view.IsShowing())
            {
                PauseView(view);

                view.configure.isShowing = false;
                hideDur = view.OnViewHide();
            }
            if (holdingView && popOption && holdingView.flags.HasFlag(ViewFlags.Stacked))
            {
                PopView(holdingView, hideStackBehind);
            }
            if (view != null)
            {
                if (destroyOption
                    && view.configure.uiFlags.HasFlag(ViewFlags.DestroyOnHided)
                    && !(view.configure.uiFlags.HasFlag(ViewFlags.DontDestroyOnStack) && IsViewInStack(view))
                    )
                {
                    RemoveHoldingView(view);
                    if (view.GetGameObject() != null)
                        GameObject.Destroy(view.GetGameObject(), hideDur);
                }
            }
            else if(holdingView)
            {
                m_HoldingViews.TryToRemoveHoldingView(holdingView.uiid, holdingView.uniqueID, view);
            }
        }

        //尝试销毁HoldingView
        void RemoveHoldingView(IUIView view)
        {
            if (view==null||string.IsNullOrEmpty(view.uniqueID))
                return;
            //如果holdingView可以被移除
            m_HoldingViews.TryToRemoveHoldingView(view.GetUIID(), view.uniqueID, view);

            GetController(view.GetUIID(), view.uniqueID)?.OnViewDestroy(view);
            TryReleaseCtrl(view.GetUIID(), view.uniqueID, view.controller);
            view.uniqueID = null;//销毁view
        }

        IUIView InstantiateView(IUIView viewPrefab, string uniqueID)
        {
            string canvasID = viewPrefab.configure.canvasID;
            string containerName = viewPrefab.configure.containerName;
            GameObject parentObj = GetViewContainer(canvasID, containerName);
            if (parentObj == null)
            {
                DebugUtils.LogError($"[{Instance.GetType().Name}]不存在作为Parent的ID为[{(string.IsNullOrEmpty(canvasID) ? DEFAULT_CANVAS_ID : canvasID)}]的Canvas");
                return null;
            }
            IUIView view = GameObject.Instantiate(viewPrefab.GetGameObject(), parentObj.transform, false).GetComponent<IUIView>();
            if (string.IsNullOrEmpty(uniqueID))
            {
                uniqueID = PBMath.GenGUID();
            }
            view.uniqueID = uniqueID;
            view.GetGameObject()?.SetActive(false);

            m_HoldingViews.SetHoldingView(view);

            view.GetGameObject()?.ObservedDestroy(_ =>
            {
                if (UIViews.HasInstance)
                {
                    //如果UI依旧在激活中，则跑完生命周期
                    UIViews.Instance.HideView(view, m_HoldingViews.TryGetHoldingView(view), false, false);
                    //end
                    UIViews.Instance.RemoveHoldingView(view);
                }
            });
            TryBindCtrlToView(view);
            return view;
        }

    }

}