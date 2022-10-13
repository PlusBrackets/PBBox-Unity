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

    /// <summary>
    /// 数据操作基类
    /// </summary>
    /// <typeparam name="TResultData"></typeparam>
    public abstract class BaseDataOperator<TResultData> : IDataOperator<TResultData>
    {
        string IDataOperator.key { get; set; } = null;
        /// <summary>
        /// 唯一标识
        /// </summary>
        /// <value></value>
        public string key => ((IDataOperator)this).key;
        /// <summary>
        /// 如果不会立即返回，请将isAsyncRequest设置为true
        /// </summary>
        /// <value></value>
        public virtual bool isAsyncRequest { get; } = false;
        bool IDataOperator.isRegistered { get; set; } = false;

        public bool isRegistered
        {
            get
            {
                return ((IDataOperator)this).isRegistered;
            }
        }

        public void Request<TParam>(Action<IOperatorResult<TResultData>> callBack, TParam parameters)
        {
            PBEvents.Emit<string>(DataOperatorManager.Events.OnRequestStart, key);
            OnRequest((result) =>
            {
                PBEvents.Emit<IOperatorResult<TResultData>>(DataOperatorManager.Events.OnRequestEnd, result);
                callBack?.Invoke(result);
            }, parameters);
        }

        /// <summary>
        /// 完成后需要invoke Callback，参数为 返回数据，CODE，msg
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="parameters"></param>
        protected abstract void OnRequest<TParam>(Action<IOperatorResult<TResultData>> callBack, TParam parameters);

        protected OperatorResult<TResultData> CreateGenericResult(TResultData data, DataOperatorManager.Code status = DataOperatorManager.Code.Success, string message = null)
        {
            return new OperatorResult<TResultData>(key, data, status, message);
        }

        protected TResult CreateResult<TResult>(TResult data, DataOperatorManager.Code status = DataOperatorManager.Code.Success, string message = null) where TResult : IOperatorResult<TResult>, new()
        {
            return new TResult { RequestKey = key, Data = data, Status = status, Message = message };
        }

    }

    public abstract class BaseDataOperator<TResultData, TParam> : BaseDataOperator<TResultData>
    {
        protected abstract bool CanParamNull { get; }

        protected override void OnRequest<_TParam>(Action<IOperatorResult<TResultData>> callBack, _TParam parameters)
        {
            TParam _param;
            if (parameters is TParam __param)
            {
                _param = __param;
                OnRequest(callBack, _param);
            }
            else if (CanParamNull && parameters == null)
            {
                _param = default;
                OnRequest(callBack, _param);
            }
            else
            {
                DebugUtils.LogError($"Operate Err: [{this.key}] 参数类型错误，传入类型[{typeof(_TParam)}] 不是指定的[{typeof(TParam)}]类型.");
                callBack.Invoke(CreateGenericResult(default(TResultData), DataOperatorManager.Code.Err_IncorrectParams, "参数类型错误"));
            }
        }

        protected abstract void OnRequest(Action<IOperatorResult<TResultData>> callBack, TParam parameters);
    }

    public abstract class BaseDataOperator : BaseDataOperator<object>
    {

    }
}