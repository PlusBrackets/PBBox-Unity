using System.Collections.Generic;

namespace PBBox.Attributes
{
    [System.Obsolete]
    /// <summary>
    /// PopupStr的设置,在此处设置默认的popup内容
    /// </summary>
    public static class PopupStrSetting
    {
        /// <summary>
        /// 对应字符串数组
        /// </summary>
        public enum StrType
        {
            //Example
            Example
        }

        internal readonly static Dictionary<StrType, string[]> StrDic
#if UNITY_EDITOR
        = new Dictionary<StrType, string[]>
        {
            //Example
            {StrType.Example,new string[]{
                "Example1",
                "E1/Example2",
                "E2/Example3",
                "E2/Example4"
            } }
        };
        #else
        = null;
#endif

    }
}