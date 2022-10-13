/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace PBBox
{
    public static partial class PBExtensions
    {
        #region Unity

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetOrAddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }
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
        /// 获得 lastKeyTime - firstKeyTime
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float CuvreWidth(this AnimationCurve target)
        {
            if (target.length >= 2)
            {
                return target.keys[target.length - 1].time - target.keys[0].time;
            }
            return 0;
        }

        /// <summary>
        /// 按尺寸evaluate
        /// </summary>
        /// <param name="target"></param>
        /// <param name="time"></param>
        /// <param name="width">与该curve宽度对应的宽度</param>
        /// <returns></returns>
        public static float Evaluate(this AnimationCurve target, float time, float width)
        {
            float w = target.CuvreWidth();
            if (w > 0 && width > 0)
            {
                time = time / width * w;
            }
            return target.Evaluate(time);
        }

        #endregion
        #region String
        /// <summary>
        /// string是否包含给定的多个string中的其中一个
        /// </summary>
        /// <param name="target"></param>
        /// <param name="values"></param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns></returns>
        public static bool Contains(this string target, IEnumerable<string> values)
        {
            foreach (string str in values)
            {
                if (target.Contains(str)) return true;
            }
            return false;
        }

        /// <summary>
        /// string是否包含给定的多个string中的其中一个
        /// </summary>
        /// <param name="target"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool Contains(this string target, params string[] values)
        {
            foreach (string str in values)
            {
                if (target.Contains(str)) return true;
            }
            return false;
        }

        #endregion
        #region Number
        /// <summary>
        /// value是否在此范围 [min,max)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsInRange(this float value, float min, float max)
        {
            return value >= min && value < max;
        }

        
        #endregion
        #region Array
        public static T GetRandomItem<T>(this IList<T> target)
        {
            return RandomUtils.RandomArrayItem(target);
        }

        public static T[] GetRandomItems<T>(this IList<T> target, int count, bool canRepeat = true)
        {
            return RandomUtils.RandomArrayItems(target, count, canRepeat);
        }

        public static void Push<T>(this IList<T> target, T value)
        {
            target.Add(value);
        }

        public static T Pop<T>(this IList<T> target)
        {
            T value = target[target.Count - 1];
            target.RemoveAt(target.Count - 1);
            return value;
        }

        public static bool TryPop<T>(this IList<T> target, out T value)
        {
            value = default;
            if (target == null || target.Count == 0)
            {
                return false;
            }
            else
            {
                value = Pop(target);
                return true;
            }
        }

        public static T Peek<T>(this IList<T> target)
        {
            return target[target.Count - 1];
        }

        public static bool TryPeek<T>(this IList<T> target, out T value)
        {
            value = default;
            if (target == null || target.Count == 0)
            {
                return false;
            }
            else
            {
                value = Peek(target);
                return true;
            }
        }

        public static bool TryGet<T>(this IList<T> target, int index, out T value, T defaultValue = default(T))
        {
            if(index<target.Count && index>=0){
                value = target[index];
                return true;
            }
            else
            {
                value = defaultValue;
                return false;
            }
        }

        /// <summary>
        /// 存在value中的任意一个值则返回true
        /// </summary>
        /// /// <param name="source"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> value)
        {
            foreach (var v in value)
            {
                if (System.Linq.Enumerable.Contains(source, v))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 存在value中的所有值则返回true
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> value)
        {
            foreach (var v in value)
            {
                if (!System.Linq.Enumerable.Contains(source, v))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}