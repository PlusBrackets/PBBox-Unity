using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox
{

    public static partial class PBExtensions
    {
        /// <summary>
        /// [x,y]
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>        
        public static int RandomRange(this Vector2Int target)
        {
            return RandomUtils.Range(target.x, target.y + 1);
        }

        /// <summary>
        /// [x,y]
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float RandomRange(this Vector2 target)
        {
            return RandomUtils.Range(target.x, target.y);
        }

        /// <summary>
        /// rect里的随机点
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector2 RandomInsidePoint(this Rect target)
        {
            return new Vector2(
                RandomUtils.Range(target.xMin, target.xMax),
                RandomUtils.Range(target.yMin, target.yMax)
            );
        }

        /// <summary>
        /// 获得总时间 lastKeyTime - firstKeyTime
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float GetDuration(this AnimationCurve target)
        {
            if (target.length >= 2)
            {
                return target[target.length - 1].time - target[0].time;
            }
            return 0;
        }

        /// <summary>
        /// 按比例进度evaluate
        /// </summary>
        /// <param name="target"></param>
        /// <param name="progress"></param>
        /// <param name="totalProgress">与该curve宽度对应的宽度</param>
        /// <returns></returns>
        public static float Evaluate(this AnimationCurve target, float progress, float totalProgress)
        {
            float w = target.GetDuration();
            if (w > 0 && totalProgress > 0)
            {
                progress = progress / totalProgress * w;
            }
            return target.Evaluate(progress);
        }
    }
}