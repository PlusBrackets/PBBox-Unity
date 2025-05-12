using System.Collections;
using System.Collections.Generic;

namespace PBBox
{
    /// <summary>
    /// PBPox的一些设置
    /// </summary>    
    internal static partial class PBBoxSettings
    {
        internal const string ASSEMBLY_NAME_PBBOX = "PBBox";
        internal const string ASSEMBLY_UNITY_PROJECT = "Assembly-CSharp";

        /// <summary>
        /// 单例初始化时需要反射的程序集名称,为空则反射全部程序集(耗时)
        /// </summary>
        internal static readonly HashSet<string> SingletonInitReflectAssemblies = new HashSet<string>()
        {
            ASSEMBLY_NAME_PBBOX,
            ASSEMBLY_UNITY_PROJECT,
        };

        /// <summary>
        /// PBCommandSystem 允许反射绑定指令的程序集名称,为空则反射全部程序集(耗时)
        /// </summary>
        internal static readonly HashSet<string> CommandSystemInitReflectAssemblies = new HashSet<string>
        {
            ASSEMBLY_NAME_PBBOX,
            ASSEMBLY_UNITY_PROJECT,
        };

        /// <summary>
        /// 允许反射绑定指令的程序集名称,为空则反射全部程序集(耗时)
        /// </summary>
        internal static readonly HashSet<string> UIInitReflectAssemblies = new HashSet<string>
        {
            ASSEMBLY_UNITY_PROJECT
        };

        /// <summary>
        /// 是否在DataOperatorManager创建时自动反射注册DataOperator
        /// </summary>
        internal static readonly bool enableDataOperaAutoReflectInit = true;
        /// <summary>
        /// 允许反射绑定指令的程序集名称,为空则反射全部程序集(耗时)
        /// </summary>
        internal static readonly HashSet<string> DataOperaReflectAssemblies = new HashSet<string>
        {
            ASSEMBLY_UNITY_PROJECT
        };

    }
}