/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.02.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
using PBBox.Unity;
using UnityEngine;

namespace PBBox
{
    /// <summary>
    /// 逻辑更新器
    /// </summary>
    public static partial class LogicUpdater
    {
        public class Fixed : LogicUpdaterBase<Fixed> { }
        public class Late : LogicUpdaterBase<Late> { }

        private static Fixed s_FixedUpdater { get; set; } = new Fixed();
        private static Late s_LateUpdater { get; set; } = new Late();

        /// <summary>
        /// 静态初始化，配置core中的LogicUpdater的一些值
        /// </summary>
        static LogicUpdater()
        {
            s_SetDefaultUpdateEnable = SetDefaultUpdaterEnable;
        }

        #region Default Updater
        private static void SetDefaultUpdaterEnable(bool enable)
        {
            if (enable)
            {
                UnityLifecycleReference.Instance.onUpdate += CallDefaultUpdate;
            }
            else
            {
                UnityLifecycleReference.Instance.onUpdate -= CallDefaultUpdate;
            }
        }

        private static void CallDefaultUpdate()
        {
            float deltaTime = Time.deltaTime;
            UpdateDefault(deltaTime);
        }
        #endregion

        #region Fixed Updater
        private static void SetFixedUpdateEnable(bool enable)
        {
            if (enable)
            {
                Unity.UnityLifecycleReference.Instance.onFixedUpdate += CallFixedUpdate;
            }
            else
            {
                Unity.UnityLifecycleReference.Instance.onFixedUpdate -= CallFixedUpdate;
            }
        }

        private static void CallFixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            s_FixedUpdater.UpdateHandlers(deltaTime);
            if (s_FixedUpdater.Count == 0)
            {
                SetFixedUpdateEnable(false);
            }
        }

        public static void Attach(ILogicUpdateHandler<Fixed> handler)
        {
            bool _isNoHandler = s_FixedUpdater.Count == 0;
            s_FixedUpdater.Attach(handler);
            if (_isNoHandler && s_FixedUpdater.Count > 0)
            {
                SetFixedUpdateEnable(true);
            }
        }

        public static void Detach(ILogicUpdateHandler<Fixed> handler, bool immediately = false)
        {
            s_FixedUpdater.Detach(handler, immediately);
        }
        #endregion

        #region Late Updater
        private static void SetLateUpdateEnable(bool enable)
        {
            if (enable)
            {
                Unity.UnityLifecycleReference.Instance.onLateUpdate += CallLateUpdate;
            }
            else
            {
                Unity.UnityLifecycleReference.Instance.onLateUpdate -= CallLateUpdate;
            }
        }

        private static void CallLateUpdate()
        {
            float deltaTime = Time.deltaTime;
            s_LateUpdater.UpdateHandlers(deltaTime);
            if (s_LateUpdater.Count == 0)
            {
                SetLateUpdateEnable(false);
            }
        }

        public static void Attach(ILogicUpdateHandler<Late> handler)
        {
            bool _isNoHandler = s_LateUpdater.Count == 0;
            s_LateUpdater.Attach(handler);
            if (_isNoHandler && s_LateUpdater.Count > 0)
            {
                SetLateUpdateEnable(true);
            }
        }

        public static void Detach(ILogicUpdateHandler<Late> handler, bool immediately = false)
        {
            s_LateUpdater.Detach(handler, immediately);
        }
        #endregion
    }
}