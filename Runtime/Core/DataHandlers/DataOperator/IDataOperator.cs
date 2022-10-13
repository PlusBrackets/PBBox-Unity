/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PBBox
{
    public interface IDataOperator
    {
        string key { get; internal set; }
        bool isRegistered { get; set; }
    }

    public interface IDataOperator<TResultData> : IDataOperator
    {
        bool isAsyncRequest { get; }
        void Request<TParam>(Action<IOperatorResult<TResultData>> callBack, TParam parameters);
    }

    public static class DataOperatorExtensions
    {

        #region extension 
        /// <summary>
        /// 异步请求数据
        /// </summary>
        /// <param name="opera"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<IOperatorResult<T>> RequestAsync<T, TParam>(this IDataOperator<T> opera, TParam parameters)
        {
            IOperatorResult<T> result = default;
            opera.Request((r) =>
            {
                result = r;
            }, parameters);
            while (!result.IsVaild())
            {
                await Task.Delay(1);
            }
            return result;
        }

        /// <summary>
        /// 同步请求数据
        /// </summary>
        /// <param name="opera"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IOperatorResult<T> RequestSync<T, TParam>(this IDataOperator<T> opera, TParam parameters)
        {
            if (opera.isAsyncRequest)
            {
                return AsyncUtils.RunSync<IOperatorResult<T>>(() => opera.RequestAsync(parameters));
            }
            else
            {
                IOperatorResult<T> result = default;
                opera.Request((r) =>
                {
                    result = r;
                }, parameters);
                return result;
            }
        }
        #endregion
    }
}