
/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.04.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PBBox
{
    /// <summary>
    /// 异步工具
    /// </summary>
    public static class AsyncUtils
    {
        private static readonly TaskFactory _taskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static TaskFactory GetFactory()
        {
            return _taskFactory;
        }

        public static T RunSync<T>(this Func<Task<T>> func)
        {
            return _taskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

        public static void RunSync(this Func<Task> func)
        {
            _taskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }
    }
}