/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.04.26
 *@author: PlusBrackets
 --------------------------------------------------------*/

namespace PBBox
{
    public static partial class PBMath
    {
        public static float Remap(this float value, float srcMin, float srcMax, float dstMin, float dstMax)
        {
            float v = (value - srcMin) / (srcMax - srcMin) * (dstMax - dstMin) + dstMin;
            return v;
        }

        /// <summary>
        /// 是否在循环范围内，如一天24小时，6点是否在20点到8点之间
        /// </summary>
        /// <param name="value"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="length">循环长度，如一天24小时</param>
        /// <returns></returns>
        public static bool IsInCycleRange(float value, float left, float right, float length)
        {
            if (left > right)
            {
                right += length;
                if (value < left)
                {
                    value += length;
                }
            }
            if (value > left && value <= right)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 相加不溢出，溢出时返回int.MaxValue或int.MinValue
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int AddSafe(this int a, int b)
        {
            if (b > 0 && a > int.MaxValue - b)
            {
                return int.MaxValue;
            }
            else if (b < 0 && a < int.MinValue - b)
            {
                return int.MinValue;
            }
            else
            {
                return a + b;
            }
        }

        /// <summary>
        /// 相减不溢出，溢出时返回int.MaxValue或int.MinValue
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int SubtractSafe(this int a, int b)
        {
            return AddSafe(a, -b);
        }

        /// <summary>
        /// 相加不溢出，溢出时返回long.MaxValue或long.MinValue
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long AddSafe(this long a, long b)
        {
            if (b > 0 && a > long.MaxValue - b)
            {
                return long.MaxValue;
            }
            else if (b < 0 && a < long.MinValue - b)
            {
                return long.MinValue;
            }
            else
            {
                return a + b;
            }
        }

        /// <summary>
        /// 相减不溢出，溢出时返回long.MaxValue或long.MinValue
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long SubtractSafe(this long a, long b)
        {
            return AddSafe(a, -b);
        }

        /// <summary>
        /// 转换为int，溢出时返回int.MaxValue或int.MinValue
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ClampToInt(this long value)
        {
            return (int)System.Math.Clamp(value, int.MinValue, int.MaxValue);
        }
    }
}