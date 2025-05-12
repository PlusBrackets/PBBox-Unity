/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;

namespace PBBox
{
    /// <summary>
    /// 数据模型类
    /// </summary>
    public sealed class DataModels : SingleClass<DataModels>
    {
        public enum Events
        {
            OnModelCreate,
            OnModelUpdate,
            OnModelUpdateFinish,
            OnModelRelease
        }

        public static PBEvents.Register<Events, IDataModel> OnEvents => PBEvents.Register<Events, IDataModel>.DEFAULT;

        private readonly Dictionary<string, IDataModel> m_Models = new Dictionary<string, IDataModel>();

        #region static func
        public static T Get<T>(bool autoCreate = true) where T : IDataModel, new() => Instance.GetModel<T>(autoCreate);

        public static T Get<T>(string modelName, bool autoCreate = true) where T : IDataModel, new() => Instance.GetModel<T>(modelName, autoCreate);

        public static IDataModel Get(string modelName) => Instance.GetModel(modelName);

        public static bool TryGet<T>(out T model) where T : IDataModel, new() => Instance.TryGetModel<T>(out model);
        public static bool TryGet<T>(string modelName, out T model) where T : IDataModel, new() => Instance.TryGetModel<T>(modelName, out model);
        public static bool TryGet(string modelName, out IDataModel model) => Instance.TryGetModel(modelName, out model);

        public static void Update<T>() where T : IDataModel
        {
            if (Instance.TryGetModel<T>(out var model))
            {
                Instance.UpdateModel(model);
            }
        }

        public static void Update(string modelName)
        {
            if (Instance.TryGetModel(modelName, out var model))
            {
                Instance.UpdateModel(model);
            }
        }

        public static void Release<T>() where T : IDataModel
        {
            if (Instance.TryGetModel<T>(out var model))
            {
                Instance.ReleaseModel(model);
            }
        }
        public static void Release(string modelName)
        {
            if (Instance.TryGetModel(modelName, out var model))
            {
                Instance.ReleaseModel(model);
            }
        }

        #endregion

        public void UpdateModel(IDataModel model, Action<IDataModel> onUpdateFinish = null)
        {
            if (!model.IsVaild)
                return;
            model.OnUpdate(_m =>
            {
                onUpdateFinish?.Invoke(_m);
                PBEvents.Emit(Events.OnModelUpdateFinish, model);
            });
            model.OnUpdateEvent?.Invoke(model);
            PBEvents.Emit(Events.OnModelUpdate, model);
        }

        public void ReleaseModel(IDataModel model)
        {
            if (!model.IsVaild)
                return;
            if (!string.IsNullOrEmpty(model.ModelName))
            {
                m_Models.Remove(model.ModelName);
            }
            model.IsVaild = false;
            model.OnRelease();
            model.OnReleaseEvent?.Invoke(model);
            PBEvents.Emit(Events.OnModelRelease, model);
        }

        public T GetModel<T>(bool autoCreate = true) where T : IDataModel, new()
        {
            return GetModel<T>(typeof(T).FullName, autoCreate);
        }

        public T GetModel<T>(string modelName, bool autoCreate = true) where T : IDataModel, new()
        {
            if (!m_Models.TryGetValue(modelName, out var model))
            {
                if (autoCreate)
                {
                    model = new T();
                    model.ModelName = modelName;
                    model.IsVaild = true;
                    m_Models[modelName] = model;
                    model.OnCreate();
                    PBEvents.Emit(Events.OnModelCreate, model);
                    return (T)model;
                }
                return default(T);
            }
            return (T)model;
        }

        public IDataModel GetModel(string modelName)
        {
            m_Models.TryGetValue(modelName, out var model);
            return model;
        }

        public bool TryGetModel(string modelName, out IDataModel model)
        {
            return m_Models.TryGetValue(modelName, out model);
        }

        public bool TryGetModel<T>(string modelName, out T model) where T : IDataModel
        {
            if (m_Models.TryGetValue(modelName, out var _model))
            {
                if (_model is T __model)
                {
                    model = __model;
                    return true;
                }
            }
            model = default(T);
            return false;
        }

        public bool TryGetModel<T>(out T model) where T : IDataModel
        {
            string name = typeof(T).FullName;
            return TryGetModel<T>(name, out model);
        }
    }

    public static class DataModelExtensions
    {
        public static void Update(this IDataModel model, Action<IDataModel> onUpdateFinish = null)
        {
            DataModels.Instance.UpdateModel(model, onUpdateFinish);
        }

        public static void Release(this IDataModel model)
        {
            DataModels.Instance.ReleaseModel(model);
        }
    }

    public interface IDataModel
    {
        bool IsVaild { get; set; }
        string ModelName { get; set; }
        Action<IDataModel> OnReleaseEvent { get; }
        Action<IDataModel> OnUpdateEvent { get; }
        void OnCreate();
        void OnUpdate(Action<IDataModel> onUpdateFinish);
        void OnRelease();
    }

    /// <summary>
    /// 数据模型基类
    /// </summary>
    public abstract class BaseDataModel : IDataModel
    {
        bool IDataModel.IsVaild { get; set; }
        string IDataModel.ModelName { get; set; }
        public Action<IDataModel> OnReleaseEvent => _onReleaseEvent;
        public Action<IDataModel> OnUpdateEvent => _onUpdateEvent;
        void IDataModel.OnCreate() => this.OnCreate();
        void IDataModel.OnUpdate(Action<IDataModel> callBack) => this.OnUpdate(callBack);
        void IDataModel.OnRelease() => this.OnRelease();

        public bool IsVaild => ((IDataModel)this).IsVaild;
        public string ModelName => ((IDataModel)this).ModelName;
        private event Action<IDataModel> _onUpdateEvent, _onReleaseEvent;

        protected virtual void OnCreate() { }

        protected virtual void OnUpdate(Action<IDataModel> onUpdateFinish)
        {
            onUpdateFinish?.Invoke(this);
        }
        protected virtual void OnRelease() { }

        protected T GetModel<T>(bool autoCreate = true) where T : IDataModel, new()
        {
            return DataModels.Get<T>(autoCreate);
        }
        protected T GetModel<T>(string modelName, bool autoCreate = true) where T : IDataModel, new()
        {
            return DataModels.Get<T>(modelName, autoCreate);
        }

        protected IDataModel GetModel(string modelName)
        {
            return DataModels.Get(modelName);
        }


    }
}