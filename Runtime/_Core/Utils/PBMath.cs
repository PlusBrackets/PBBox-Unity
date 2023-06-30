/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.04.26
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;

namespace PBBox
{
    public static partial class PBMath
    {
        #region Lines
        /// <summary>
        /// 求两直线的交点
        /// </summary>
        /// <param name="startPos1"></param>
        /// <param name="endPos1"></param>
        /// <param name="startPos2"></param>
        /// <param name="endPos2"></param>
        /// <param name="intersectionPos">相交位置</param>
        /// <param name="isSegments">是否为线段，若true，则只判断线段内的焦点</param>
        /// <returns></returns>
        public static bool LineIntersection(Vector2 startPos1, Vector2 endPos1, Vector2 startPos2, Vector2 endPos2, out Vector2 intersectionPos, bool isSegments = false)
        {
            intersectionPos = Vector2.zero;

            //判断是否平行

            var d = (endPos1.x - startPos1.x) * (endPos2.y - startPos2.y) - (endPos1.y - startPos1.y) * (endPos2.x - startPos2.x);

            if (d == 0.0f)
            {
                return false;
            }

            var u = ((startPos2.x - startPos1.x) * (endPos2.y - startPos2.y) - (startPos2.y - startPos1.y) * (endPos2.x - startPos2.x)) / d;
            var v = ((startPos2.x - startPos1.x) * (endPos1.y - startPos1.y) - (startPos2.y - startPos1.y) * (endPos1.x - startPos1.x)) / d;

            if (isSegments && (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f))
            {
                return false;
            }

            intersectionPos.x = startPos1.x + u * (endPos1.x - startPos1.x);
            intersectionPos.y = startPos1.y + u * (endPos1.y - startPos1.y);

            return true;
        }

        #endregion

        public static float Remap(this float value, float srcMin, float srcMax, float dstMin, float dstMax)
        {
            float v = (value - srcMin) / (srcMax - srcMin) * (dstMax - dstMin) + dstMin;
            return v;
        }

        public static float RemapClamp(this float value, float srcMin, float srcMax, float dstMin, float dstMax)
        {
            float v = (value - srcMin) / (srcMax - srcMin) * (dstMax - dstMin) + dstMin;
            return Mathf.Clamp(v, dstMin, dstMax);
        }

        /// <summary>
        /// 浮点数比较是否相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="deviation"></param>
        /// <returns></returns>
        public static bool IsAlmostEqual(this float a, float b, float deviation = 0.00001f)//float.Epsilon)
        {
            return Mathf.Abs(a - b) <= deviation;
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
        public static int AddSafe(int a, int b)
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
        public static int SubtractSafe(int a, int b)
        {
            return AddSafe(a, -b);
        }

        /// <summary>
        /// 相加不溢出，溢出时返回long.MaxValue或long.MinValue
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long AddSafe(long a, long b)
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
        public static long SubtractSafe(long a, long b)
        {
            return AddSafe(a, -b);
        }
    }
}