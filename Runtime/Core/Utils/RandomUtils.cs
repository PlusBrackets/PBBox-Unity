/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox
{
    /// <summary>
    /// 使用System.Random，可用在线程中
    /// </summary>
    public static class RandomUtils
    {
        public static System.Random sysRandom = new System.Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// [min,max)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Range(float min, float max)
        {
            double r = sysRandom.NextDouble();
            return (float)((max - min) * r + min);
        }

        /// <summary>
        /// [min,max)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Range(int min, int max)
        {
            return sysRandom.Next(min, max);
        }

        /// <summary>
        /// 在单位圆里随机
        /// </summary>
        public static Vector2 insideUnitCircle
        {
            get
            {
                double r = sysRandom.NextDouble();
                float r2 = Mathf.Sqrt((float)r);
                return onsideUnitCircle * r2;
            }
        }

        /// <summary>
        /// 在单位圆上随机
        /// </summary>
        public static Vector2 onsideUnitCircle
        {
            get
            {
                double t = sysRandom.NextDouble() * 2 * Mathf.PI;
                return new Vector2(Mathf.Sin((float)t), Mathf.Cos((float)t));
            }
        }

        /// <summary>
        /// 抽奖
        /// </summary>
        /// <param name="rate">中奖几率 [0,1]</param>
        /// <returns></returns>
        public static bool Lottery(float rate)
        {
            return Lottery(rate, 1f);
        }

        public static bool Lottery(float rate, float range)
        {

            if (rate >= range)
                return true;
            if (rate <= 0)
                return false;
            return Range(0f, range) < rate;
        }

        /// <summary>
        /// 投硬币
        /// </summary>
        /// <returns></returns>
        public static bool CastCoin()
        {
            return Range(0, 2) == 1;
        }

        #region Array

        /// <summary>
        /// 随机选取数组中的一位
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomArrayItem<T>(IList<T> array)
        {
            int rIndex = Range(0, array.Count);
            return array[rIndex];
        }

        /// <summary>
        /// 随机取数组中的count位
        /// </summary>
        /// <param name="array"></param>
        /// <param name="count"></param>
        /// <param name="canRepeat"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] RandomArrayItems<T>(IList<T> array, int count, bool canRepeat = true)
        {
            T[] items = new T[Mathf.Clamp(count, 0, array.Count)];
            if (canRepeat)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = RandomArrayItem(array);
                }
            }
            else{
                List<T> _temps = new List<T>(array);
                for(int i = 0;i<items.Length;i++){
                    int rIndex = Range(0,_temps.Count);
                    T temp = _temps[rIndex];
                    _temps.RemoveAt(rIndex);
                    items[i] = temp;
                }
            }
            return items;
        }

        /// <summary>
        /// 打乱array的顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        public static void DisturbArray<T>(T[] array, int startIndex, int count)
        {
            for (int i = 0; i < count - 1; i++)
            {
                int index = startIndex + Range(0, count - 1 - i);
                int exIndex = startIndex + count - 1 - i;
                T temp = array[index];
                array[index] = array[exIndex];
                array[exIndex] = temp;
            }
        }

        /// <summary>
        /// 打乱list的顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        public static void DisturbArray<T>(IList<T> array, int startIndex, int count)
        {
            for (int i = 0; i < count - 1; i++)
            {
                int index = startIndex + Range(0, count - i);
                int exIndex = startIndex + count - 1 - i;
                T temp = array[index];
                array[index] = array[exIndex];
                array[exIndex] = temp;
            }
        }

        /// <summary>
        /// 打乱list的顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void DisturbArray<T>(IList<T> array)
        {
            DisturbArray(array, 0, array.Count);
        }

        /// <summary>
        /// 打乱array的顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void DisturbArray<T>(T[] array)
        {
            DisturbArray(array, 0, array.Length);
        }


        #endregion
    }
}