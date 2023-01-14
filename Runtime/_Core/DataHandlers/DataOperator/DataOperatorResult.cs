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
    // /// <summary>
    // /// 操作数据返回接口
    // /// </summary>
    public interface IOperatorResult
    {
        string RequestKey { get; set; }
        DataOperatorManager.Code Status { get; set; }
        string Message { get; set; }
        bool IsVaild();
    }

    /// <summary>
    /// 操作数据返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOperatorResult<T> : IOperatorResult
    {
        T Data { get; set; }
    }

    // /// <summary>
    // /// 数据返回
    // /// </summary>
    // public struct OperatorResult : IOperatorResult
    // {
    //     public string RequestKey { get; set; }
    //     public DataOperatorManager.Code Status { get; set; }
    //     public string Message { get; set; }

    //     public bool IsVaild()
    //     {
    //         return !string.IsNullOrEmpty(RequestKey);
    //     }

    //     public OperatorResult(string requestKey, DataOperatorManager.Code status, string message = null)
    //     {
    //         this.RequestKey = requestKey;
    //         this.Status = status;
    //         this.Message = message;
    //     }
    // }

    /// <summary>
    /// 数据返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct OperatorResult<T> : IOperatorResult<T>
    {
        public string RequestKey { get; set; }
        public DataOperatorManager.Code Status { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        public bool IsVaild()
        {
            return !string.IsNullOrEmpty(RequestKey);
        }

        public OperatorResult(string requestKey, T data, DataOperatorManager.Code status, string message = null)
        {
            this.RequestKey = requestKey;
            this.Status = status;
            this.Data = data;
            this.Message = message;
        }

        public override string ToString()
        {
            return $"Result from operate [{RequestKey}], Code:{Status}, Message:{Message},\n Data: {Data}";
        }

    }

}