using System.Threading;
using System.Threading.Tasks;
using System;
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
    public static partial class PBMath
    {
        public static readonly System.DateTime DATE_START_TIME = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        public static readonly int SUID_SUFFIX_BIT = 16;
        public static readonly int SUID_SUFFIX_MAX_NUM = (int)Math.Pow(2, SUID_SUFFIX_BIT);
        private static long _SUID_LAST_TIMESTAMP = -1;
        private static int _SUID_LAST_GEN_TIMES = 0;

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
        public static string GenGUID(string specifier = "N")
        {
            return System.Guid.NewGuid().ToString(specifier);
        }

        static uint SIMPLE_UID_BASE = 0;

        /// <summary>
        /// 生成一个仅限本次运行的ID，基于uint的形式，从0开始，累积到最大值时会重置为0
        /// </summary>
        /// <returns></returns>
        public static uint GenUintID()
        {
            uint uid = SIMPLE_UID_BASE;
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

        /// <summary>
        /// 生成一个18位的唯一ID，每毫秒支持最多创建65535个id，超出会sleep 1毫秒
        /// </summary>
        /// <returns></returns>
        public static long GenSUID(){
            long suid;
            while(!GenSUID(out suid)){
                Thread.Sleep(1);
            }
            return suid;
        }

        /// <summary>
        /// 生成一个18位的唯一ID，每毫秒支持最多创建65535个id，超出返回false
        /// </summary>
        /// <param name="suid"></param>
        /// <returns></returns>
        public static bool GenSUID(out long suid){
            long timestamp = GenTimestamp();
            long suffixNum = 0;
            suid = -1;
            if(timestamp == _SUID_LAST_TIMESTAMP)
            {
                if (_SUID_LAST_GEN_TIMES >= SUID_SUFFIX_MAX_NUM)
                {
                    return false;
                }
                suffixNum = ++_SUID_LAST_GEN_TIMES;
            }
            else{
                _SUID_LAST_GEN_TIMES = 0;
                _SUID_LAST_TIMESTAMP = timestamp;
            }
            suid = timestamp << SUID_SUFFIX_BIT;
            suid += suffixNum;
            return true;
        }

        public static long SUIDToTimestamp(long suid){
            return suid >> SUID_SUFFIX_BIT;
        }

        /// <summary>
        /// 生成时间戳(ms,UTC)
        /// </summary>
        /// <returns></returns>
        public static long GenTimestamp()
        {
            return (long)(System.DateTime.UtcNow - DATE_START_TIME).TotalMilliseconds;
        }

        /// <summary>
        /// 生成时间戳(s,UTC)
        /// </summary>
        /// <returns></returns>
        public static long GenTimestampShort()
        {
            return (long)(System.DateTime.UtcNow - DATE_START_TIME).TotalSeconds;
        }

        /// <summary>
        /// 根据所给时间生成时间戳(ms,UTC)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GenTimestamp(System.DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - DATE_START_TIME).TotalMilliseconds;
        }

        /// <summary>
        /// 根据所给时间生成时间戳(s,UTC)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GenTimestampShort(System.DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - DATE_START_TIME).TotalSeconds;
        }

        /// <summary>
        /// timestamp (ms) 转DateTime (UTC时间)
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime TimestampToDateTime(long timestamp){
            return DATE_START_TIME.AddMilliseconds(timestamp);
        }

        /// <summary>
        /// timestamp (s) 转DateTime (UTC时间)
        /// </summary>
        /// <param name="shortTimestamp"></param>
        /// <returns></returns>
        public static DateTime TimestampShortToDateTime(long shortTimestamp){
            return DATE_START_TIME.AddSeconds(shortTimestamp);
        }
    }
}