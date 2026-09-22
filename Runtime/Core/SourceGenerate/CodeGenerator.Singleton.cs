#if UNITY_5_3_OR_NEWER && UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PBBox.SourceGenerate
{
    internal static partial class CodeGenerator
    {
        [RegisterGenerator]
        private static CodeItem[] _GenPBBoxRegister_Singleton()
        {
            StringBuilder sb = new StringBuilder();
            BeginGenericContent(sb, "PBBoxRegister");
            //Contents
            sb.AppendLine(@"
        static partial void RegisterSingletons()
        {
#if PB_TEST_LOG || UNITY_EDITOR
            var test = new System.Diagnostics.Stopwatch();
            test.Start();
#endif
            // register start
            ");
            var registerCodes = GetSingletonRegisterCode(out var debugLogs);
            if (registerCodes != null) {
                foreach (var str in registerCodes)
                {
                    sb.AppendLine($"            {str}");
                }
            }
            sb.AppendLine(@"
            // register end
            
            ISingleton.MarkInitialized();
#if PB_TEST_LOG || UNITY_EDITOR
            test.Stop();
            System.Text.StringBuilder logs = new System.Text.StringBuilder();
            logs.AppendLine($""单例直接绑定完成，耗时:{test.Elapsed.TotalMilliseconds}ms"");
            ");
            if (debugLogs != null)
            {
                foreach(var str in debugLogs)
                {
                    sb.AppendLine($"            {str}");
                }
            }

            sb.AppendLine($"            Log.Debug(logs.ToString(), nameof(ISingleton), \"{Log.PBBoxLoggerName} \");");
            sb.AppendLine(@"
#endif

        }
            ");
            EndGenericContent(sb);
            return new CodeItem[]
            {
                new CodeItem("PBBoxRegister.Singleton.g.cs", sb.ToString())
            };
        }

        private static List<string> GetSingletonRegisterCode(out List<string> debugLogs)
        {
            //获得所有继承了ISingleton的具体类
            var types = typeof(ISingleton).GetAllChildClass(moreDeep: true, containAbstract: false);

            //获取其中继承了ISingleton<T>且T不等于该type或者相等且T为可实例化的类型，并返回type和T的集合列表
            var tempMap = ISingleton.GetRegisterMapInTypes(types);
            StringBuilder logs = new StringBuilder();
            logs.AppendLine("生成单例注册代码");
            var codes = new List<string>();
            debugLogs = new List<string>();
            string assemblyName = PBBoxSettings.ASSEMBLY_UNITY_PROJECT;

            foreach (var (interfaceType, (instanceType, priority)) in tempMap)
            {
                if (interfaceType == instanceType)
                {
                    logs.AppendLine($"--单例类型 {interfaceType.FullName} 与实例类型相同，跳过");
                    continue;
                }
                bool interfaceTypeAccessible = interfaceType.IsTypeAccessibleFromAssembly(assemblyName);
                bool instanceTypeAccessible = instanceType.IsTypeAccessibleFromAssembly(assemblyName);
                string interfaceAccessPath = interfaceType.GetAccessPath();
                string instanceAccessPath = instanceType.GetAccessPath();
                if (interfaceTypeAccessible && instanceTypeAccessible)
                {
                    //两者都可直接访问
                    codes.Add($"ISingleton<{interfaceAccessPath}>.SetInstanceType<{instanceAccessPath}>();");
                }
                else if (interfaceTypeAccessible && !instanceTypeAccessible)
                {
                    //实例类型不可访问
                    codes.Add($"ISingleton<{interfaceAccessPath}>.SetInstanceType(Type.GetType(\"{instanceType.FullName}, {instanceType.Assembly.GetName().Name}\"));");
                }
                else
                {
                    //接口类型不可访问
                    codes.Add($"ISingleton.SetInstanceType(Type.GetType(\"{interfaceType.FullName}, {interfaceType.Assembly.GetName().Name}\"), Type.GetType(\"{instanceType.FullName}, {instanceType.Assembly.GetName().Name}\"));");
                }
                debugLogs.Add($"logs.AppendLine($\"{interfaceType.FullName}-- > {instanceType.FullName}，优先级：{priority}\");");
                logs.AppendLine($"{interfaceType.FullName}-- > {instanceType.FullName}，优先级：{priority}, 接口可访问:{interfaceTypeAccessible}, 实例可访问:{instanceTypeAccessible}");
            }

            Log.Debug(logs.ToString(), nameof(CodeGenerator), Log.PBBoxLoggerName);
            //ISingleton<SingleModulueTest.ITestModuleA>.SetInstanceType(Type.GetType(""SingleModulueTest + TestModuleA, Assembly - CSharp""));
            //ISingleton<SingleModulueTest.TestSingleton3>.SetInstanceType<SingleModulueTest.TestSingleton4>();
            //ISingleton<PBBox.IAssetManager>.SetInstanceType(Type.GetType(""PBBox.AssetManager2, PBBox""));
            //ISingleton<PBBox.IEventManager>.SetInstanceType<PBBox.EventPool>();
            //ISingleton.SetInstanceType(Type.GetType(""PBBox.UnityLifecycleReference, PBBox""), Type.GetType(""PBBox.UnityLifecycleReference, PBBox""));
            //ISingleton<PBBox.View.IViewManager>.SetInstanceType(Type.GetType(""PBBox.ViewManager, PBBox""));


            return codes;
        }
        
        //[RegisterGenerator]
        //private static CodeItem[] _GenSingletonTest()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    BeginGenericContent(sb, "PBBoxSingletonTest");
        //    for(int i = 0; i < 100; i++)
        //    {
        //        sb.AppendLine($"        public interface ITestA{i} : ISingleton<ITestA{i}> {{ }}");
        //        sb.AppendLine($"        private class TestA{i} : ITestA{i} {{ }}");
        //        sb.AppendLine("");
        //        sb.AppendLine($"        private interface ITestB{i} : ISingleton<ITestB{i}> {{ }}");
        //        sb.AppendLine($"        private class TestB{i} : ITestB{i} {{ }}");
        //        sb.AppendLine("");
        //    }
        //    
        //    EndGenericContent(sb);
        //    return new CodeItem[]
        //    {
        //        new CodeItem("PBBoxSingletonTest.g.cs", sb.ToString())
        //    };
        //}
    }
}
#endif