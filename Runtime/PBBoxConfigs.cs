using System.Collections;
using System.Collections.Generic;

namespace PBBox
{
    /// <summary>
    /// PBPox的一些设置
    /// </summary>    
    internal static class PBBoxConfigs
    {
        #region CMD_SYS
        /// <summary>
        /// PBCommandSystem 允许反射绑定指令的程序集名称,为空则反射全部程序集(耗时)
        /// </summary>
        public static readonly HashSet<string> CMD_SYS_REFLECT_ASSEMBLIES = new HashSet<string> {
            "Assembly-CSharp",
            "PBBox"
            };
        #endregion
        
        #region UI
        /// <summary>
        /// 允许反射绑定指令的程序集名称,为空则反射全部程序集(耗时)
        /// </summary>
        public static readonly HashSet<string> UI_REFLECT_ASSEMBLIES = new HashSet<string>{
            "Assembly-CSharp"
            };
        #endregion

        #region Data_Operator
        /// <summary>
        /// 是否在DataOperatorManager创建时自动反射注册DataOperator
        /// </summary>
        public static readonly bool DATA_OPERA_REFLECT_AUTO = true;
        /// <summary>
        /// 允许反射绑定指令的程序集名称,为空则反射全部程序集(耗时)
        /// </summary>
        public static readonly HashSet<string> DATA_OPERA_REFLECT_ASSEMBLIES = new HashSet<string>{
            "Assembly-CSharp"
            };
        #endregion

    }
}