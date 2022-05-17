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

        /// <summary>
        /// 当前存在的View
        /// </summary>
        Dictionary<string, IUIView> m_InstantiatedViews;
        /// <summary>
        /// view预制体
        /// </summary>
        Dictionary<string, IUIView> m_ViewPrefabs;

        UIViews()
        {
            m_InstantiatedViews = new Dictionary<string, IUIView>();
            m_ViewPrefabs = new Dictionary<string, IUIView>();
            InitViewStacked();
            InitViewCanvas();
            InitViewLoader();
            InitMVC();
        }


        #region Public Statics Func

        /// <summary>
        /// 显示View
        /// </summary>
        /// <param name="uiid"></param>
        /// <returns></returns>
        public static IUIView Show(string uiid)
        {
            IUIView view = GetOrCreate(uiid);
            return Show(view);
        }

        /// <summary>
        /// 显示View
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T Show<T>(string uiid) where T : class, IUIView
        {
            return Show(uiid) as T;
        }

        /// <summary>
        /// 显示View
        /// </summary>
        /// <param name="view"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Show<T>(T view) where T : IUIView
        {
            Instance.ShowView(view, true);
            return view;
        }

        /// <summary>
        /// 关闭View
        /// </summary>
        /// <param name="uiid"></param>
        /// <returns></returns>
        public static IUIView Hide(string uiid)
        {
            IUIView view = Get(uiid);
            return Hide(view);
        }

        /// <summary>
        /// 关闭View
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T Hide<T>(string uiid) where T : class, IUIView
        {
            return Hide(uiid) as T;
        }

        /// <summary>
        /// 关闭View
        /// </summary>
        /// <param name="view"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Hide<T>(T view) where T : IUIView
        {
            Instance.HideView(view, true);
            return view;
        }

        /// <summary>
        /// 暂停view
        /// </summary>
        /// <param name="uiid"></param>
        /// <returns></returns>
        public static IUIView Pause(string uiid)
        {
            IUIView view = Get(uiid);
            return Pause(view);
        }

        /// <summary>
        /// 暂停view
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T Pause<T>(string uiid) where T : class, IUIView
        {
            return Pause(uiid) as T;
        }

        /// <summary>
        /// 恢复显示中的已暂停的view
        /// </summary>
        /// <param name="uiid"></param>
        /// <returns></returns>
        public static IUIView Resume(string uiid)
        {
            IUIView view = Get(uiid);
            return Resume(view);
        }

        /// <summary>
        /// 恢复显示中的已暂停的view
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T Resume<T>(string uiid) where T : class, IUIView
        {
            return Resume(uiid) as T;
        }


        /// <summary>
        /// 暂停view
        /// </summary>
        /// <param name="view"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Pause<T>(T view) where T : IUIView
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
        public static T Resume<T>(T view) where T : IUIView
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
        /// <param name="uiid"></param>
        /// <returns></returns>
        public static IUIView GetOrCreate(string uiid)
        {
            IUIView view = Get(uiid);
            if (view == null)
            {
                Instance.m_ViewPrefabs.TryGetValue(uiid, out view);
                if (view == null)
                {
                    view = Load(uiid);
                }
                if (view != null)
                {
                    view = Instance.InstantiateView(view);
                }
            }
            return view;
        }

        /// <summary>
        /// 创建一个view
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T GetOrCreate<T>(string uiid) where T : class, IUIView
        {
            return GetOrCreate(uiid) as T;
        }

        /// <summary>
        /// 获取实例化中的View
        /// </summary>
        /// <param name="uiid"></param>
        /// <returns></returns>
        public static IUIView Get(string uiid)
        {
            IUIView view = null;
            Instance.m_InstantiatedViews.TryGetValue(uiid, out view);
            return view;
        }

        /// <summary>
        /// 获取实例化中的View
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T Get<T>(string uiid) where T : class, IUIView
        {
            return Get(uiid) as T;
        }

        #endregion

        /// <summary>
        /// 显示View
        /// </summary>
        /// <param name="view"></param>
        /// <param name="pushOption">是否有入栈操作</param>
        /// <param name="pauseView">是否更改是否直接暂停view</param>
        void ShowView(IUIView view, bool pushOption, bool pauseView = false)
        {
            if (view == null)
                return;
            if (view.IsHidingOrUninitialized())
            {
                if (view.configure.uiFlags.HasFlag(ViewFlags.Stacked) && pushOption)
                {
                    PushView(view);
                }

                view.configure.isShowing = true;
                view.OnViewShow();

                if (!pauseView || !view.configure.isPausing.HasValue /*为保证新建的View有完整的生命周期*/)
                {
                    Resume(view);
                }
                else
                {
                    Pause(view);
                }
            }
        }

        /// <summary>
        /// 隐藏View
        /// </summary>
        /// <param name="view"></param>
        /// <param name="popOption">出栈操作是否执行</param>
        /// <param name="pasueView">是否暂停View</param>
        /// <param name="destroyOption">销毁操作是否执行</param>
        /// <returns></returns>
        float HideView(IUIView view, bool popOption, bool destroyOption = true)
        {
            if (view != null && view.IsShowing())
            {
                Pause(view);

                view.configure.isShowing = false;
                float hideDur = view.OnViewHide();

                if (view.configure.uiFlags.HasFlag(ViewFlags.Stacked) && popOption)
                {
                    PopView(view);
                }
                if (view.configure.uiFlags.HasFlag(ViewFlags.DestroyOnHided) && destroyOption)
                {
                    RemoveInstantiatedView(view);
                    if (view.GetGameObject() != null)
                        GameObject.Destroy(view.GetGameObject(), hideDur);
                }
                return hideDur;
            }
            return 0f;
        }

        void RemoveInstantiatedView(IUIView view)
        {
            IUIView v = null;
            string uiid = view.GetUIID();
            if (m_InstantiatedViews.TryGetValue(uiid, out v))
            {
                if (v == view || v == null)
                {
                    m_InstantiatedViews.Remove(uiid);
                    TryDisposeCtrl(view.GetUIID(), view.controller);
                }
            }
        }

        IUIView InstantiateView(IUIView viewPrefab)
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
            view.GetGameObject()?.SetActive(false);
            Instance.m_InstantiatedViews.TryAdd(view.GetUIID(), view);
            view.GetGameObject()?.ObservedDestroy(_ =>
            {
                if (UIViews.HasInstance)
                {
                    //如果UI依旧在激活中，则跑完生命周期
                    UIViews.Instance.HideView(view,false,false);
                    //end
                    UIViews.Instance.RemoveInstantiatedView(view);
                }
            });
            TryBindCtrlToView(view);
            return view;
        }


    }

}