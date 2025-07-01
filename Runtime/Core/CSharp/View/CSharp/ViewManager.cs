/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.19
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PBBox.View
{
    using ControllerFactoryHandler = Func<string, IViewFactory, IController>;

    [SingletonPriority(-1)]
    internal class ViewManager : IViewManager, ISingletonLifecycle
    {
        private Dictionary<string, IController> m_Controllers = new Dictionary<string, IController>();
        private Dictionary<string, ControllerFactory> m_ControllerFactories = new Dictionary<string, ControllerFactory>();

        private static readonly Lazy<Dictionary<Type, ControllerFactoryHandler>> s_ControllerCreators = new Lazy<Dictionary<Type, ControllerFactoryHandler>>();

        public static ControllerFactoryHandler CreateExpressionFactory(Type controllerType)
        {
            if (!s_ControllerCreators.Value.TryGetValue(controllerType, out var factory))
            {
                var ctor = controllerType.GetConstructor(new[] { typeof(string), typeof(IViewFactory) });
                if (ctor == null)
                {
                    ctor = controllerType.GetConstructor(new[] { typeof(string) });
                }
                if (ctor == null)
                {
                    Log.Error($"Controller type '{controllerType.Name}' does not have a valid constructor. " +
                              "It must have a constructor with parameters (string id, IViewFactory viewFactory) or (string id).",
                              nameof(IViewManager), Log.PBBoxLoggerName);
                    return null;
                }
                var idParam = Expression.Parameter(typeof(string), "id");
                var viewFactoryParam = Expression.Parameter(typeof(IViewFactory), "viewFactory");
                var newExpression = Expression.New(ctor, idParam, viewFactoryParam);
                var lambda = Expression.Lambda<ControllerFactoryHandler>(newExpression, idParam, viewFactoryParam);
                factory = lambda.Compile();
                s_ControllerCreators.Value[controllerType] = factory;
            }
            return factory;
        }

        public void OnCreateAsSingleton()
        {
            //收集所有的控制器工厂
            RegisterAttributeControllers();
        }

        public void OnDestroyAsSingleton()
        {

        }

        private void RegisterAttributeControllers()
        {
            //用反射获取所有标记了BindViewAttribute的IController类型
            var assemblyNames = PBBoxSettings.CommonInitReflectAssemblies.Union(PBBoxSettings.InitReflectAssemblies_View);
            var controllerTypes = typeof(IController).GetAllChildClassWithAttribute<BindViewAttributeBase>(assemblyNames: assemblyNames);

#if PB_TEST_LOG
            System.Text.StringBuilder logs = new System.Text.StringBuilder();
            logs.AppendLine("");
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
#endif
            foreach (var type in controllerTypes)
            {
                var attributes = type.GetCustomAttributes<BindViewAttributeBase>(false);
                foreach (var attribute in attributes)
                {
                    if (string.IsNullOrEmpty(attribute.Id))
                    {
                        Log.Error($"Controller type '{type.Name}' has a BindViewAttribute with an empty ID.", nameof(IViewManager), Log.PBBoxLoggerName);
                        continue;
                    }
                    if (m_ControllerFactories.ContainsKey(attribute.Id))
                    {
                        Log.Error($"Controller factory with ID '{attribute.Id}' is already registered for type '{type.Name}'.", nameof(IViewManager), Log.PBBoxLoggerName);
                        continue;
                    }
                    var factory = new ControllerFactory(attribute.Id, type, attribute.GetViewFactory());
                    if (attribute.IsCreateExpressionFactory)
                    {
                        factory.SetFactory(CreateExpressionFactory(type));
                    }
                    m_ControllerFactories[attribute.Id] = factory;
#if PB_TEST_LOG
                    logs.AppendLine($"绑定 ID[{attribute.Id}] <-> Controller[{type.FullName}]");
#endif
                }
            }

#if PB_TEST_LOG
            sw.Stop();
            Log.Debug($"Controller工厂绑定结束,已绑定:{m_ControllerFactories.Count},耗时{sw.Elapsed.TotalMilliseconds}ms"
                + logs.ToString(), nameof(IViewManager), Log.PBBoxLoggerName);
#endif
        }

        public void RegisterControllerFactory(string id, Type controllerType, IViewFactory viewFactory, ControllerFactoryHandler factory = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                Log.Error("Controller ID is null or empty.", nameof(IViewManager), Log.PBBoxLoggerName);
                return;
            }
            if (m_ControllerFactories.ContainsKey(id))
            {
                Log.Error($"Controller factory with ID '{id}' is already registered.", nameof(IViewManager), Log.PBBoxLoggerName);
                return;
            }

            var controllerFactory = new ControllerFactory(id, controllerType, viewFactory);
            if (factory != null)
            {
                controllerFactory.SetFactory(factory);
            }
            m_ControllerFactories[id] = controllerFactory;
        }

        public void UnregisterControllerFactory(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Log.Error("Controller factory ID is null or empty.", nameof(IViewManager), Log.PBBoxLoggerName);
                return;
            }
            m_ControllerFactories.Remove(id);
        }

        public void RegisterController(IController controller)
        {
            if (controller == null || string.IsNullOrEmpty(controller.ID))
            {
                Log.Error("Controller or its ID is null or empty.", nameof(IViewManager), Log.PBBoxLoggerName);
                return;
            }
            if (!m_Controllers.TryAdd(controller.ID, controller))
            {
                Log.Error($"Controller with ID '{controller.ID}' is already registered.", nameof(IViewManager), Log.PBBoxLoggerName);
            }
        }

        public void UnregisterController(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Log.Error("Controller ID is null or empty.", nameof(IViewManager), Log.PBBoxLoggerName);
                return;
            }
            m_Controllers.Remove(id);
        }

        public IController Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Log.Error("Controller ID is null or empty.", nameof(IViewManager), Log.PBBoxLoggerName);
                return null;
            }
            if (m_Controllers.TryGetValue(id, out var controller))
            {
                return controller;
            }
            if (m_ControllerFactories.TryGetValue(id, out var factory))
            {
                controller = factory.Create();
                if (controller != null)
                {
                    RegisterController(controller);
                    return controller;
                }
            }
            Log.Error($"Controller with ID '{id}' not found.", nameof(IViewManager), Log.PBBoxLoggerName);
            return null;
        }


        private class ControllerFactory
        {
            private string m_Id;
            private Type m_ControllerType;
            private IViewFactory m_ViewFactory;
            private ControllerFactoryHandler m_ContrllerFactory;

            public ControllerFactory(string id, Type controllerType, IViewFactory viewFactory)
            {
                this.m_Id = id;
                this.m_ControllerType = controllerType;
                this.m_ViewFactory = viewFactory;
            }

            public void SetFactory(ControllerFactoryHandler factory)
            {
                this.m_ContrllerFactory = factory;
            }

            public IController Create()
            {
                if (m_ContrllerFactory == null)
                {
                    return Activator.CreateInstance(m_ControllerType, m_Id, m_ViewFactory) as IController;
                }
                else
                {
                    return m_ContrllerFactory(m_Id, m_ViewFactory);
                }
            }
        }
    }
}