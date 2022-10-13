namespace PBBox
{
    /// <summary>
    /// PBPox的一些设置
    /// </summary>    
    internal static partial class PBBoxConfigs
    {
        #region CMD_SYS
        /// <summary>
        /// 是否在游戏加载时自动创建PBCommandSystem
        /// </summary>
        public static readonly bool CMD_SYS_AUTO_CREATE = true;
        /// <summary>
        /// PBCommandSystem 允许反射绑定指令的程序集名称,为空则反射全部程序集(耗时)
        /// </summary>
        public static readonly string[] CMD_SYS_REFLECT_ASSEMBLIES = new string[] {
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
        public static readonly string[] DATA_OPERA_REFLECT_ASSEMBLIES = new string[]{
            "Assembly-CSharp"
            };
        #endregion

        #region UI
        /// <summary>
        /// 允许反射绑定指令的程序集名称,为空则反射全部程序集(耗时)
        /// </summary>
        public static readonly string[] UI_REFLECT_ASSEMBLIES = new string[]{
            "Assembly-CSharp"
            };
        #endregion
    }
}