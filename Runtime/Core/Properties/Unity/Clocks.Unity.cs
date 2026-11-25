#if UNITY_5_3_OR_NEWER
/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.25
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;

namespace PBBox.Properties
{
    public static partial class Clocks
    {
        private static Clock _gameUnscaleTime = new Clock();

        /// <summary>
        /// 一个基于游戏非缩放时间的时钟实例
        /// </summary>
        public static IClock GameUnscaleTime => _gameUnscaleTime;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitUnity()
        {
            PBEvents.OnFrameUpdate.Subscribe(OnFrameUpdate, -100);
            PBEvents.OnFixedUpdate.Subscribe(OnFixedUpdate, -100);
        }

        private static void OnFrameUpdate(in FrameUpdateEvent ev)
        {
            _gameTime.SetTime(ev.TotalTime);
            UpdateCachedClocks();
        }
        
        private static void OnFixedUpdate(in FrameUpdateEvent ev)
        {
            _gameUnscaleTime.SetTime(ev.TotalTime);
        }
    }
}

#endif