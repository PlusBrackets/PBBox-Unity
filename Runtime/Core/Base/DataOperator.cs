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
    #region interface
    public interface IDataOperator : IDisposable
    {
        string key { get; }
        bool isRegistered { get; set; }
    }

    public interface IDataOperator<T> : IDataOperator
    {
        bool isAsyncRequest { get;}
        void Request(Action<DataOperatorResult<T>> callBack, params object[] parameters);
    }
    #endregion

    /// <summary>
    /// 数据返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DataOperatorResult<T>
    {
        public string requestKey { get; private set; }
        public int status { get; private set; }
        public T data { get; private set; }
        public string message { get; private set;}

        public DataOperatorResult(string requestKey, T result, int status, string message = null)
        {
            this.requestKey = requestKey;
            this.status = status;
            this.data = result;
            this.message = message;
        }

    }

    /// <summary>
    /// 数据操作基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DataOperator<T> : IDataOperator<T>
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        /// <value></value>
        public virtual string key { get; protected set; } = null;
        public virtual bool autoRegisterOnCreate { get; } = true;
        public virtual bool isAsyncRequest { get; } = false;
        bool IDataOperator.isRegistered { get; set; } = false;

        public bool isRegistered
        {
            get
            {
                return ((IDataOperator)this).isRegistered;
            }
        }

        public bool isDisposed { get; private set; } = false;

        public DataOperator(string key)
        {
            this.key = key;
            if (autoRegisterOnCreate)
            {
                Register();
            }
        }

        public void Request(Action<DataOperatorResult<T>> callBack, params object[] parameters)
        {
            PBEvents.Emit<string>(DataOperators.Events.OnRequestStart, key);
            OnRequest((data, status, msg) =>
            {
                var result = CreateResult(data, status, msg);
                PBEvents.Emit<DataOperatorResult<T>>(DataOperators.Events.OnRequestEnd, result);
                callBack?.Invoke(result);
            }, parameters);
        }

        protected abstract void OnRequest(Action<T, int, string> callBack, params object[] parameters);
        protected virtual void OnDispose() { }

        /// <summary>
        /// 加入DataOperatorHandler中以便全局访问到，不加入也不影响直接使用
        /// </summary>
        /// <returns></returns>
        public bool Register()
        {
            return DataOperators.Register(this);
        }

        /// <summary>
        /// 从DataOperatorHandler中移除
        /// </summary>
        /// <param name="dispose"></param>
        /// <returns></returns>
        public bool Unregister(bool dispose = true)
        {
            return DataOperators.Unregister(this, dispose);
        }

        public void Dispose()
        {
            if (isDisposed) return;
            if (isRegistered)
            {
                Unregister(false);
            }
            OnDispose();
            isDisposed = true;
        }

        protected DataOperatorResult<T> CreateResult(T data, int status = DataOperators.CODE_SUCCESS, string message = null){
            return new DataOperatorResult<T>(key, data, status, message);
        }

    }

    /// <summary>
    /// 数据处理器
    /// </summary>
    public static class DataOperators
    {
        public enum Events
        {
            OnRequestStart,
            OnRequestEnd
        }

        public const int CODE_ERR = 404;
        public const int CODE_SUCCESS = 200;
        static Lazy<Dictionary<string, IDataOperator>> m_Operas = new Lazy<Dictionary<string, IDataOperator>>();

        private static Dictionary<String, IDataOperator> operas => m_Operas.Value;

        /// <summary>
        /// 获取已注册的Operator
        /// </summary>
        /// <typeparam name="TOpera"></typeparam>
        public static TOpera GetOperator<TOpera>(string key) where TOpera : class, IDataOperator
        {
            IDataOperator op = null;
            if (operas.TryGetValue(key, out op))
            {
                return op as TOpera;
            }
            return null;
        }

        public static bool Register(IDataOperator dataOperator)
        {
            if (operas.TryAdd(dataOperator.key, dataOperator))
            {
                dataOperator.isRegistered = true;
                return true;
            }
            else
            {
                DebugUtils.LogError($"DataOperator[key:{dataOperator.key}] has been registered!");
                return false;
            }
        }

        public static bool Unregister(IDataOperator dataOperator, bool dispose = true)
        {
            IDataOperator opera = GetOperator<IDataOperator>(dataOperator.key);
            if (opera == dataOperator)
            {
                operas.Remove(dataOperator.key);
                dataOperator.isRegistered = false;
                if (dispose == true)
                {
                    dataOperator.Dispose();
                }
                return true;
            }
            return false;
        }

        public static void ClearAllOperator(bool disposeAll = true)
        {
            string[] keys = operas.Keys.ToArray();
            foreach (string k in keys)
            {
                var opera = operas[k];
                opera.isRegistered = false;
                if (disposeAll)
                {
                    opera.Dispose();
                }
            }
            operas.Clear();
        }

        public static DataOperatorResult<T> Request<T>(string key, params object[] parameters)
        {
            IDataOperator<T> opera = GetOperator<IDataOperator<T>>(key);
            if (opera != null)
            {
                return opera.RequestSync(parameters);
            }
            return null;
        }

        public static void Request<T>(string key, Action<DataOperatorResult<T>> callBack, params object[] parameters)
        {
            IDataOperator<T> opera = GetOperator<IDataOperator<T>>(key);
            if (opera != null)
            {
                opera.Request(callBack, parameters);
            }
        }

        public static async Task<DataOperatorResult<T>> RequestAsync<T>(string key, params object[] parameters)
        {
            IDataOperator<T> opera = GetOperator<IDataOperator<T>>(key);
            if (opera != null)
            {
                return await opera.RequestAsync(parameters);
            }
            return null;
        }

        #region extends
        /// <summary>
        /// 异步请求数据
        /// </summary>
        /// <param name="opera"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<DataOperatorResult<T>> RequestAsync<T>(this IDataOperator<T> opera, params object[] parameters)
        {
            DataOperatorResult<T> result = null;
            opera.Request((r) =>
            {
                result = r;
            }, parameters);
            while (result == null)
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
        public static DataOperatorResult<T> RequestSync<T>(this IDataOperator<T> opera, params object[] parameters)
        {
            if (opera.isAsyncRequest)
            {
                return AsyncUtils.RunSync<DataOperatorResult<T>>(()=>opera.RequestAsync(parameters));
            }
            else
            {
                return opera.RequestAsync(parameters).GetAwaiter().GetResult();
            }
        }

        #endregion
    }
}