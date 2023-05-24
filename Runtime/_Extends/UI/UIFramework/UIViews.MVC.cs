/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.21
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

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

        Dictionary<int, IUIViewController> m_ViewCtrls;

        /// <summary>
        /// 绑定ViewCtrl
        /// </summary>
        void InitMVC()
        {
            m_ViewCtrlBindMap = new Dictionary<string, BindTypeInfo>();
            m_ViewCtrls = new Dictionary<int, IUIViewController>();

#if UNITY_EDITOR || GAME_TEST
            System.Text.StringBuilder logs = new System.Text.StringBuilder();
            logs.AppendLine("");
            var testCost = new System.Diagnostics.Stopwatch();
            testCost.Start();
#endif
            var types = ReflectionUtils.GetAllChildClassWithAttribute<IUIViewController, BindUIViewAttribute>(assemblyNames: PBBoxConfigs.UI_REFLECT_ASSEMBLIES);

            // if (types == null || types.Count() == 0)
            //     return;
            int bindCount = 0;
            foreach (Type t in types)
            {
                var attributes = t.GetCustomAttributes<BindUIViewAttribute>(false);
                foreach (var a in attributes)
                {
                    if (m_ViewCtrlBindMap.TryAdd(a.uiid, new BindTypeInfo() { type = t, attribute = a }))
                    {
#if UNITY_EDITOR || GAME_TEST
                        logs.AppendLine($"绑定 UIID[{a.uiid}] <-> ViewCtrl[{t.FullName}]");
#endif
                        bindCount++;
                    }
                    else
                    {
                        Log.Error($"{t.FullName} 无法绑定UIID: {a.uiid}，请检查是否有重复的绑定", "UIViews", Log.PBBoxLoggerName);
                    }
                }
            }
#if UNITY_EDITOR || GAME_TEST
            testCost.Stop();
            Log.Debug($"UIViewCtrl绑定结束,已绑定:{bindCount},耗时{testCost.Elapsed.TotalMilliseconds}ms"
                + logs.ToString(), "UIViews", Log.PBBoxLoggerName);
#else
            Log.Debug($"UIViewCtrl绑定结束,已绑定:{bindCount}", "UIViews", Log.PBBoxLoggerName);
#endif
        }
        #region controllers
             /// <summary>
        /// 获取View的控制器
        /// </summary>
        /// <param name="uiid"></param>
        /// <returns></returns>
        public static IUIViewController GetController(string uiid, string uniqueID)
        {
            Instance.m_ViewCtrls.TryGetValue(HoldingView.GetHashCode(uiid, uniqueID), out var ctrl);
            return ctrl;
        }

        /// <summary>
        /// 获取View的控制器
        /// </summary>
        /// <param name="uiid"></param>
        /// <returns></returns>
        public static T GetController<T>(string uiid, string uniqueID) where T : class, IUIViewController
        {
            return GetController(uiid, uniqueID) as T;
        }

        /// <summary>
        /// 手动销毁ViewCtrl，
        /// </summary>
        /// <param name="uiid"></param>
        /// <returns></returns>
        public static bool ReleaseController(string uiid, string uniqueID)
        {
            var self = Instance;
            if (self.m_ViewCtrls.TryGetValue(HoldingView.GetHashCode(uiid, uniqueID), out var ctrl))
            {
                if (self.m_HoldingViews.HasInstantiatedView(uiid, uniqueID))
                {
                    Log.Debug($"UIID:[{uiid}] ViewCtrl占用中，请先将对应的UIView销毁", "UIViews", Log.PBBoxLoggerName);
                    return false;
                }
                else
                {
                    self.TryReleaseCtrl(uiid, uniqueID, ctrl, true);
                    return true;
                }
            }
            return false;
        }
        
        void TryReleaseCtrl(string uiid, string uniqueID, IUIViewController ctrl, bool forceRelease = false)
        {
            if (ctrl == null)
            {
                if (!m_ViewCtrls.TryGetValue(HoldingView.GetHashCode(uiid, uniqueID), out ctrl))
                    return;
            }
            if (forceRelease)// || m_ViewCtrlBindMap[uiid].attribute.dontAutoDispose) //暂不加入
            {
                ctrl.Release();
                m_ViewCtrls.Remove(HoldingView.GetHashCode(uiid, uniqueID));
            }
        }

        void TryBindCtrlToView(IUIView view)
        {
            if (m_ViewCtrlBindMap.TryGetValue(view.GetUIID(), out BindTypeInfo info))
            {
                if (!m_ViewCtrls.TryGetValue(HoldingView.GetHashCode(view), out var vc))
                {
                    vc = Activator.CreateInstance(info.type) as IUIViewController;
                    m_ViewCtrls.Add(HoldingView.GetHashCode(view), vc);
                }
                view.controller = vc;
                vc.OnViewCreate(view);
            }
        }
        #endregion

        #region Models

        public static T GetModel<T>(bool autoCreate = true) where T : class, IUIViewModel, new()
        {
            return DataModels.Get<T>(autoCreate);
        }

        public static T GetModel<T>(string modelName, bool autoCreate = true) where T : class, IUIViewModel, new()
        {
            return DataModels.Get<T>(modelName, autoCreate);
        }

        public static void ReleaseModel<T>() where T : IUIViewModel
        {
            DataModels.Release<T>();
        }

        public static void ReleaseModel(string modelName)
        {
            DataModels.Release(modelName);
        }
        #endregion

    }

}