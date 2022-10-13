/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.06.09
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace PBBox
{

    /// <summary>
    /// 绑定操作key，如果需要自动反射注册DataOperator，则需要添加此特性，添加此特性的Operator类需要继承BaseDataOperator或者IDataOperator
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class DataOperatorKeyAttribute : Attribute
    {
        public string key = null;
        public DataOperatorKeyAttribute(string key)
        {
            this.key = key;
        }
    }

    /// <summary>
    /// 数据处理器
    /// </summary>
    public class DataOperatorManager : SingleClass<DataOperatorManager>
    {
        public enum Events
        {
            OnRequestStart,
            OnRequestEnd
        }

        public enum Code
        {
            Err_IncorrectParams = -1,
            Success = 200,
            Fail = 404,
        }

        Lazy<Dictionary<string, IDataOperator>> __operas = new Lazy<Dictionary<string, IDataOperator>>();
        private static Dictionary<String, IDataOperator> _operas => Instance.__operas.Value;

        protected override void Init(){
            base.Init();
            if (PBBoxConfigs.DATA_OPERA_REFLECT_AUTO)
            {
                LoadAllOperators();
            }
        }

        private void LoadAllOperators()
        {
#if UNITY_EDITOR || GAME_TEST
            System.Text.StringBuilder logs = new System.Text.StringBuilder();
            logs.AppendLine("");
            var testCost = new System.Diagnostics.Stopwatch();
            testCost.Start();
#endif
            int count = 0;
            var assemblies = PBBoxConfigs.DATA_OPERA_REFLECT_ASSEMBLIES;
            var types = typeof(IDataOperator).GetAllChildClassWithAttribute<DataOperatorKeyAttribute>(assemblyNames: assemblies);
            foreach (var t in types)
            {
                var attritubes = t.GetCustomAttributes<DataOperatorKeyAttribute>(false);
                foreach (var a in attritubes)
                {
                    IDataOperator op = Activator.CreateInstance(t) as IDataOperator;
                    bool success = Register(a.key, op);
                    if (success) count++;
#if UNITY_EDITOR || GAME_TEST
                    if (success) logs.AppendLine($"载入operator[{a.key}] => {t.FullName}");
#endif
                }
            }
#if UNITY_EDITOR || GAME_TEST
            testCost.Stop();
            DebugUtils.Internal.Info<DataOperatorManager>($"DataOperator完成加载,已加载:{count},耗时{testCost.Elapsed.TotalMilliseconds}ms"
                + logs.ToString());
#else
                            
             DebugUtils.Internal.Info<DataOperatorManager>($"DataOperator完成加载,已加载:{count}");
#endif
        }

        /// <summary>
        /// 获取已注册的Operator
        /// </summary>
        /// <typeparam name="TOpera"></typeparam>
        public static TOpera GetOperator<TOpera>(string key) where TOpera : class, IDataOperator
        {
            IDataOperator op = null;
            if (_operas.TryGetValue(key, out op))
            {
                return op as TOpera;
            }
            return null;
        }

        public static bool Register(string key, IDataOperator dataOperator)
        {
            if (_operas.TryAdd(key, dataOperator))
            {
                dataOperator.key = key;
                dataOperator.isRegistered = true;
                return true;
            }
            else
            {
                DebugUtils.LogError($"DataOperator[key:{key}] has been registered!");
                return false;
            }
        }

        public static bool Unregister(IDataOperator dataOperator)
        {
            IDataOperator opera = GetOperator<IDataOperator>(dataOperator.key);
            if (opera == dataOperator)
            {
                _operas.Remove(dataOperator.key);
                dataOperator.isRegistered = false;
                if (dataOperator is IDisposable _do)
                {
                    _do.Dispose();
                }
                return true;
            }
            return false;
        }

        public static void ClearAllOperator()
        {
            string[] keys = _operas.Keys.ToArray();
            foreach (string k in keys)
            {
                var opera = _operas[k];
                opera.isRegistered = false;
                if (opera is IDisposable _do)
                {
                    _do.Dispose();
                }
            }
            _operas.Clear();
        }

        public static IOperatorResult<TResultData> RequestSync<TResultData, TParam>(string key, TParam parameters)
        {
            IDataOperator<TResultData> opera = GetOperator<IDataOperator<TResultData>>(key);
            if (opera != null)
            {
                return opera.RequestSync(parameters);
            }
            return default;
        }
        public static IOperatorResult<TResultData> RequestSync<TResultData>(string key) => RequestSync<TResultData, object>(key, null);
        public static IOperatorResult<object> RequestSync(string key) => RequestSync<object, object>(key, null);

        public static void Request<TResultData, TParam>(string key, TParam parameters, Action<IOperatorResult<TResultData>> callBack)
        {
            IDataOperator<TResultData> opera = GetOperator<IDataOperator<TResultData>>(key);
            if (opera != null)
            {
                opera.Request(callBack, parameters);
            }
        }
        public static void Request<TResultData>(string key, Action<IOperatorResult<TResultData>> callBack) => Request<TResultData, object>(key, null, callBack);
        public static void Request(string key, Action<IOperatorResult<object>> callBack) => Request<object, object>(key, null, callBack);

        public static async Task<IOperatorResult<TResultData>> RequestAsync<TResultData, TParam>(string key, TParam parameters)
        {
            IDataOperator<TResultData> opera = GetOperator<IDataOperator<TResultData>>(key);
            if (opera != null)
            {
                return await opera.RequestAsync(parameters);
            }
            return default;
        }
        public static async Task<IOperatorResult<TResult>> RequestAsync<TResult>(string key) => await RequestAsync<TResult, object>(key, null);
        public static async Task<IOperatorResult<object>> RequestAsync(string key) => await RequestAsync<object, object>(key, null);

    }

}