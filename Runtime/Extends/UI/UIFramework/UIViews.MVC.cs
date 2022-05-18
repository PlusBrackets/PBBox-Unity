/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.31
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace PBBox.UI
{

    public sealed partial class UIViews : SingleClass<UIViews>
    {
        //处理UIView - UIViewCtrl - UIViewModel之间的联系

        struct BindTypeInfo
        {
            public Type type;
            public BindUIViewAttribute attribute;
        }

        Dictionary<string, BindTypeInfo> m_ViewCtrlBindMap;

        Dictionary<string, IUIViewController> m_ViewCtrls;
        Dictionary<string, IUIViewModel> m_ViewModels;

        /// <summary>
        /// 绑定ViewCtrl
        /// </summary>
        void InitMVC()
        {
            m_ViewCtrlBindMap = new Dictionary<string, BindTypeInfo>();
            m_ViewCtrls = new Dictionary<string, IUIViewController>();
            m_ViewModels = new Dictionary<string, IUIViewModel>();

            var types = ReflectionUtils.GetAllChildClass<IUIViewController>();
            if (types == null || types.Length == 0)
                return;
            int bindCount = 0;
            foreach (Type t in types)
            {
                if (t.IsDefined(typeof(BindUIViewAttribute), true))
                {
                    var attributes = t.GetCustomAttributes<BindUIViewAttribute>();
                    foreach (var a in attributes)
                    {
                        if (m_ViewCtrlBindMap.TryAdd(a.uiid, new BindTypeInfo() { type = t, attribute = a }))
                        {
                            DebugUtils.Internal.Info<UIViews>($"绑定 UIID[{a.uiid}] <-> ViewCtrl[{t.FullName}]");
                            bindCount++;
                        }
                        else
                        {
                            this.LogInfoError($"{t.FullName} 无法绑定UIID: {a.uiid}，请检查是否有重复的绑定");
                        }
                    }
                }
            }
            DebugUtils.Internal.Info<UIViews>($"UIViewCtrl绑定结束,已绑定:{bindCount}");
        }

        /// <summary>
        /// 获取View的控制器
        /// </summary>
        /// <param name="uiid"></param>
        /// <returns></returns>
        public static IUIViewController GetController(string uiid)
        {
            Instance.m_ViewCtrls.TryGetValue(uiid, out var ctrl);
            return ctrl;
        }

        /// <summary>
        /// 获取View的控制器
        /// </summary>
        /// <param name="uiid"></param>
        /// <returns></returns>
        public static T GetController<T>(string uiid) where T : class, IUIViewController
        {
            return GetController(uiid) as T;
        }

        /// <summary>
        /// 手动销毁ViewCtrl，
        /// </summary>
        /// <param name="uiid"></param>
        /// <returns></returns>
        public static bool DisposeController(string uiid)
        {
            var self = Instance;
            if (self.m_ViewCtrls.TryGetValue(uiid, out var ctrl))
            {
                if (self.m_InstantiatedViews.ContainsKey(uiid))
                {
                    self.LogInfo($"UIID:[{uiid}] ViewCtrl占用中，请先将对应的UIView销毁");
                    return false;
                }
                else
                {
                    self.TryDisposeCtrl(uiid, ctrl, true);
                    return true;
                }
            }
            return false;
        }

        void TryDisposeCtrl(string uiid, IUIViewController ctrl, bool forceDispose = false)
        {
            if (ctrl == null)
            {
                if (!m_ViewCtrls.TryGetValue(uiid, out ctrl))
                    return;
            }
            if (forceDispose || m_ViewCtrlBindMap[uiid].attribute.dontAutoDispose)
            {
                ctrl.Dispose();
                m_ViewCtrls.Remove(uiid);
            }
        }

        void TryBindCtrlToView(IUIView view)
        {
            string uiid = view.GetUIID();
            if (m_ViewCtrlBindMap.TryGetValue(uiid, out BindTypeInfo info))
            {
                if (!m_ViewCtrls.TryGetValue(uiid, out var vc))
                {
                    vc = Activator.CreateInstance(info.type) as IUIViewController;
                    m_ViewCtrls.Add(uiid, vc);
                }
                view.controller = vc;
                vc.OnViewCreate(view);
            }
        }

        public static T GetModel<T>(bool autoCreate = true) where T : class, IUIViewModel, new()
        {
            string uid = typeof(T).FullName;
            return GetModel<T>(uid, autoCreate);
        }

        public static T GetModel<T>(string uid, bool autoCreate = true) where T : class, IUIViewModel, new()
        {
            var self = Instance;
            if (!self.m_ViewModels.TryGetValue(uid, out var model))
            {
                if (autoCreate)
                {
                    model = new T();
                    self.m_ViewModels.Add(uid, model);
                }
            }
            return model as T;
        }

        public static void DisposeModel<T>(string uid = null) where T : IUIViewModel
        {
            var self = Instance;
            if (string.IsNullOrEmpty(uid))
            {
                uid = typeof(T).FullName;
            }
            if (self.m_ViewModels.Remove(uid, out var model))
            {
                model.Dispose();
                model.onDispose?.Invoke(model);
            }
        }

    }

}