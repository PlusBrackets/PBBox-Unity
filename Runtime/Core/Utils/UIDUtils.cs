/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/

namespace PBBox
{
    /// <summary>
    /// 创建唯一标识符的工具
    /// </summary>
    public static class UIDUtils
    {
        /// <summary>
        /// 使用System.Guid生成一个唯一标识符
        /// </summary>
        /// <param name="specifier">
        /// N:
        /// 00000000000000000000000000000000
        /// 
        /// D:
        /// 00000000-0000-0000-0000-000000000000
        /// 
        /// B:
        /// {00000000-0000-0000-0000-000000000000}
        /// 
        /// P:
        /// (00000000-0000-0000-0000-000000000000)
        /// 
        /// X:
        /// {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}
        /// </param>
        /// <returns></returns>
        public static string GenUUID(string specifier = "N")
        {
            return System.Guid.NewGuid().ToString(specifier);
        }

        static uint SIMPLE_UID_BASE = 0;

        /// <summary>
        /// 生成一个仅限本次运行的UID，基于uint的形式，从0开始，累积到最大值时会重置为0
        /// </summary>
        /// <returns></returns>
        public static string GenSimpleUID()
        {
            string uid = SIMPLE_UID_BASE.ToString();
            if (SIMPLE_UID_BASE == uint.MaxValue)
            {
                SIMPLE_UID_BASE = 0;
            }
            else
            {
                SIMPLE_UID_BASE++;
            }
            return uid;
        }
    }
}