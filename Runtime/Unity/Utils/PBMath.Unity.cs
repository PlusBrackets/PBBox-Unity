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

    }
}