/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;

namespace PBBox
{
    [System.Obsolete]
    /// <summary>
    /// 数据
    /// </summary>
    public abstract class DataModel<T> : DataModel where T : DataModel<T>, new()
    {

        /// <summary>
        /// 获取一个DataModel
        /// </summary>
        /// <typeparam name="T">需要返回的类型</typeparam>
        /// <param name="isAutoCreate">如果DataModel列表中没有，是否自动注册一个新的</param>
        /// <returns></returns>
        public static T Get(bool isAutoCreate = true)
        {
            return Get<T>();
        }

        /// <summary>
        /// 新建一个DataModel
        /// </summary>
        /// <returns></returns>
        public static bool CreateModel()
        {
            return CreateModel<T>();
        }

        /// <summary>
        /// 销毁一个DataModel
        /// </summary>
        /// <returns></returns>    
        public static bool DisposeModel()
        {
            return DisposeModel<T>();
        }
    }

    /// <summary>
    /// 视图数据逻辑，储存与View关联的数据，负责处理业务逻辑（与数据源通信，处理数据等）
    /// </summary>
    public abstract class DataModel : IDisposable
    {
        #region Static Func

        protected static readonly Dictionary<Type, DataModel> m_Models = new Dictionary<Type, DataModel>();

        // /// <summary>
        // /// 当有DataModel注册时
        // /// </summary>
        // public static Action<DataModel> onModelCreate;

        // /// <summary>
        // /// 当有DataModel注销时
        // /// </summary>
        // public static Action<DataModel> onModelDispose;

        /// <summary>
        /// 返回一个DataModel
        /// </summary>
        /// <typeparam name="T">需要返回的类型</typeparam>
        /// <param name="isAutoCreate">如果DataModel列表中没有，是否自动注册一个新的</param>
        /// <returns></returns>
        public static T Get<T>(bool isAutoCreate = true) where T : DataModel, new()
        {
            if (m_Models.ContainsKey(typeof(T)))
            {
                return m_Models[typeof(T)] as T;
            }
            else if (isAutoCreate)
            {
                CreateModel<T>();
                return m_Models[typeof(T)] as T;
            }
            return null;
        }

        /// <summary>
        /// 注册一个ViewModel
        /// </summary>
        /// <typeparam name="T">要注册的类型</typeparam>
        /// <returns>成功与否</returns>
        public static bool CreateModel<T>() where T : DataModel, new()
        {
            if (m_Models.ContainsKey(typeof(T)))
            {
                DebugUtils.Internal.LogWarning(string.Format("View Model[{0}] has been created", typeof(T).ToString()));
                return false;
            }
            else
            {
                T _viewModel = new T();
                m_Models.Add(typeof(T), _viewModel);
                _viewModel.OnCreate();

                // if (onModelCreate != null)
                //     onModelCreate.Invoke(_viewModel);
                return true;
            }
        }

        /// <summary>
        /// 注销ViewModel
        /// </summary>
        /// <typeparam name="T">要注销的类型</typeparam>
        /// <returns>成功与否</returns>
        public static bool DisposeModel<T>() where T : DataModel, new()
        {
            DataModel _model;
            if (m_Models.TryGetValue(typeof(T), out _model))
            {
                m_Models.Remove(typeof(T));
                // _model = null;
                ((IDisposable)_model).Dispose();
                // if (onModelDispose != null)
                //     onModelDispose.Invoke(_model);
                return true;
            }
            else
            {
                DebugUtils.Internal.LogWarning(string.Format("View Model[{0}] is not exist", typeof(T).ToString()));
                return false;
            }
        }

        #endregion

        public bool isDisposed { get; private set; } = false;

        protected DataModel()
        {

        }

        /// <summary>
        /// 当该DataModel创建时
        /// </summary>
        protected abstract void OnCreate();

        /// <summary>
        /// 当该DataModel销毁时
        /// </summary>
        protected virtual void OnDispose() { }

        void IDisposable.Dispose()
        {
            isDisposed = true;
            OnDispose();
        }
    }

}