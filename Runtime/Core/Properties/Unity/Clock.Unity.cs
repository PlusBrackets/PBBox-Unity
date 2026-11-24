#if UNITY_5_3_OR_NEWER
/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.25
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;

namespace PBBox.Properties
{
    internal class UnityGameTimeClock : IClock
    {
        public double Now => Time.timeAsDouble;
    }

    internal class UnityGameUnscaleTimeClock : IClock
    {
        public double Now => Time.unscaledTimeAsDouble;
    }

    public partial interface IClock
    {
        /// <summary>
        /// Unity Time.timeAsDouble的Clock
        /// </summary>
        public readonly static IClock GameTime = new UnityGameTimeClock();
        /// <summary>
        /// Unity Time.unscaledTimeAsDouble的Clock
        /// </summary>
        public readonly static IClock GameUnscaleTime = new UnityGameUnscaleTimeClock();
    }
}

#endif