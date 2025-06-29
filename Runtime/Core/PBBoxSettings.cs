using System.Collections;
using System.Collections.Generic;

namespace PBBox
{
    /// <summary>
    /// PBPox的一些设置
    /// </summary>    
    public static partial class PBBoxSettings
    {
        #region 通用设置
        internal const string ASSEMBLY_NAME_PBBOX = "PBBox";
        internal const string ASSEMBLY_UNITY_PROJECT = "Assembly-CSharp";
        
        /// <summary>
        /// 通用的反射绑定程序集名称,为空则反射全部程序集(耗时)
        /// </summary>
        public static readonly HashSet<string> CommonInitReflectAssemblies = new HashSet<string>()
        {
            ASSEMBLY_NAME_PBBOX,
            ASSEMBLY_UNITY_PROJECT,
        };
        #endregion


        #region Singleton模块
        /// <summary>
        /// 额外的单例的反射绑定程序集名称
        /// </summary>
        public static readonly HashSet<string> InitReflectAssemblies_Singleton = new HashSet<string>()
        {
        };
        #endregion


        #region AssetLoader模块
        /// <summary>
        /// 额外的AssetLoader的反射绑定程序集名称
        /// </summary>
        public static readonly HashSet<string> InitReflectAssemblies_AssetLoader = new HashSet<string>()
        {
        };
        /// <summary>
        /// AssetLoader默认的加载器类型ID
        /// </summary>
        public static int AssetLoaderDefaultId = (int)AssetLoaderTypes.Resources;
        #endregion


        #region View模块
        /// <summary>
        /// 额外的View的反射绑定程序集名称
        /// </summary>
        public static readonly HashSet<string> InitReflectAssemblies_View = new HashSet<string>
        {
        };
        #endregion
        //旧

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
        internal static bool enableDataOperaAutoReflectInit = true;
        /// <summary>
        /// 允许反射绑定指令的程序集名称,为空则反射全部程序集(耗时)
        /// </summary>
        internal static readonly HashSet<string> DataOperaReflectAssemblies = new HashSet<string>
        {
            ASSEMBLY_UNITY_PROJECT
        };

    }
}