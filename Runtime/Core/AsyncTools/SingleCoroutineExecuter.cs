/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.15
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox
{
    /// <summary>
    /// 单线协程执行器，若执行新协程会关闭上一个协程
    /// </summary>
    public struct SingleCoroutineExecuter
    {
        MonoBehaviour handler;
        Coroutine coroutine;
        public bool isStarted => coroutine != null;

        public void Start(IEnumerator routine, MonoBehaviour handler)
        {
            Stop();
            this.handler = handler;
            if (handler.enabled && handler.gameObject.activeInHierarchy)
            {
                coroutine = handler.StartCoroutine(WaitRoutineEnd(routine));
            }
        }

        public void Stop()
        {
            if (coroutine == null)
                return;
            else
            {
                handler?.StopCoroutine(coroutine);
                coroutine = null;
                handler = null;
            }
        }

        IEnumerator WaitRoutineEnd(IEnumerator routine)
        {
            yield return routine;
            coroutine = null;
            handler = null;
        }
    }
}