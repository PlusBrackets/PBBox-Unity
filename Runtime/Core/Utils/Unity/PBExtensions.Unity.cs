#if UNITY_5_3_OR_NEWER
/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.15
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PBBox
{
    public static partial class PBExtensions
    {
        /// <summary>
        /// 先停止coroutine已有协程，再启动新的协程并赋值给coroutine
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="coroutine"></param>
        /// <param name="routine"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StartCoroutine(this MonoBehaviour handler, ref Coroutine coroutine, IEnumerator routine)
        {
            handler.StopCoroutine(ref coroutine);
            coroutine = handler.StartCoroutine(routine);
        }
        
        /// <summary>
        /// 停止coroutine协程并将其置为null
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="coroutine"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StopCoroutine(this MonoBehaviour handler, ref Coroutine coroutine)
        {
            if (coroutine == null)
            {
                return;
            }
            handler.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}
#endif