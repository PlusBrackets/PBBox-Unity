/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.19
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox
{
    public static partial class PBExtensions
    {

        /// <summary>
        /// 从字符串中获取稳定HashCode
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public static int GetStableHashCode(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;

            unchecked
            {
                int hash = 23;
                foreach (char c in str)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }
    }
}