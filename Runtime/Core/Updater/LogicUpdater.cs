using System.Collections;
using System.Collections.Generic;
using System;

namespace PBBox
{
    /// <summary>
    /// 逻辑更新器
    /// </summary>
    public static partial class LogicUpdater
    {
        public class Default : LogicUpdaterBase<Default> { }
        
        private static Default s_DefaultUpdater { get; set; } = new Default();

        private static Action<bool> s_SetDefaultUpdateEnable;

        public static void Attach(ILogicUpdateHandler<Default> handler)
        {
            bool _isNoHandler = s_DefaultUpdater.Count == 0;
            s_DefaultUpdater.Attach(handler);
            if (_isNoHandler && s_DefaultUpdater.Count > 0)
            {
                s_SetDefaultUpdateEnable(true);
            }
        }

        public static void Unattach(ILogicUpdateHandler<Default> handler)
        {
            s_DefaultUpdater.Unattach(handler);
        }

        private static void UpdateDefault(float deltaTime)
        {
            s_DefaultUpdater.UpdateHandlers(deltaTime);
            if (s_DefaultUpdater.Count == 0)
            {
                s_SetDefaultUpdateEnable(false);
            }
        }
    }
}