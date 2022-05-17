/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PBBox
{
    /// <summary>
    /// 通用工具
    /// </summary>
    public static class CommonUtils
    {

        #region  value

        /// <summary>
        /// 互换
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void ExChange<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        //public static void ExChange<T>(T a, T b)
        //{
        //    T temp = a;
        //    a = b;
        //    b = temp;
        //}


        #endregion

        #region prase
        /// <summary>
        /// 转化为float
        /// </summary>
        /// <returns></returns>
        public static float PraseFloat(string str, float defaultValue)
        {
            float value;
            if (!float.TryParse(str, out value))
                value = defaultValue;
            return value;
        }

        /// <summary>
        /// 转化为int
        /// </summary>
        /// <returns></returns>
        public static int PraseInt(string str, int defaultValue)
        {
            int value;
            if (!int.TryParse(str, out value))
                value = defaultValue;
            return value;
        }

        /// <summary>
        /// vector2转化为vector3，并将Y配置为Z
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector3 PraseXYToXZ(Vector2 v2)
        {
            Vector3 v3;
            v3.x = v2.x;
            v3.y = 0;
            v3.z = v2.y;
            return v3;
        }

        public static Vector2 PraseXZToXY(Vector3 v3)
        {
            Vector2 v2;
            v2.x = v3.x;
            v2.y = v3.z;
            return v2;
        }

        /// <summary>
        /// 将value转化为times的倍数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="times"></param>
        public static Rect PraseValueToTimes(Rect rect, float timesX, float timesY, float timesW, float timesH)
        {
            Vector2 vp = PraseValueToTimes(rect.position, timesX, timesY);
            Vector2 vs = PraseValueToTimes(rect.size, timesW, timesH);

            rect.position = vp;
            rect.size = vs;

            return rect;
        }

        /// <summary>
        /// 将value转化为times的倍数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="times"></param>
        public static Vector3 PraseValueToTimes(Vector3 v3, float timesX, float timesY, float timesZ)
        {
            v3.x = PraseValueToTimes(v3.x, timesX);
            v3.y = PraseValueToTimes(v3.y, timesY);
            v3.z = PraseValueToTimes(v3.z, timesZ);
            return v3;
        }

        /// <summary>
        /// 将value转化为times的倍数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="times"></param>
        public static Vector2 PraseValueToTimes(Vector2 v2, float timesX, float timesY)
        {
            v2.x = PraseValueToTimes(v2.x, timesX);
            v2.y = PraseValueToTimes(v2.y, timesY);
            return v2;
        }

        /// <summary>
        /// 将value转化为times的倍数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="times"></param>
        public static float PraseValueToTimes(float value, float times)
        {
            if (times != 0)
                value = Mathf.RoundToInt(value / times) * times;
            return value;
        }

        /// <summary>
        /// 将value转化为times的倍数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static int PraseValueToTimes(int value, int times)
        {
            if (times != 0)
                value = value / times * times;
            return value;
        }

        /// <summary>
        /// 将角度转化为 0°-- 360°内
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static float PraseDegreeIn360(float degree)
        {
            while (degree < 0)
            {
                degree = degree % 360 + 360;
            }
            degree = degree % 360;
            return degree;
        }
        #endregion

        #region distance
        /// <summary>
        /// 计算路径点数组的总路程
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static float DistanceVector3List(Vector3[] path)
        {
            float pathLength = .0f;
            for (int i = 0; i < path.Length - 1; i++)
            {
                pathLength += Vector3.Distance(path[i], path[i + 1]);
            }
            return pathLength;
        }

        /// <summary>
        /// 计算路径点数组的总路程
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static float DistanceVector3List(List<Vector3> path)
        {
            float pathLength = .0f;
            for (int i = 0; i < path.Count - 1; i++)
            {
                pathLength += Vector3.Distance(path[i], path[i + 1]);
            }
            return pathLength;
        }

        /// <summary>
        /// 计算路径点数组的总路程
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static float DistanceVector2List(Vector2[] path)
        {
            float pathLength = .0f;
            for (int i = 0; i < path.Length - 1; i++)
            {
                pathLength += Vector2.Distance(path[i], path[i + 1]);
            }
            return pathLength;
        }

        /// <summary>
        /// 计算路径点数组的总路程
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static float DistanceVector2List(List<Vector2> path)
        {
            float pathLength = .0f;
            for (int i = 0; i < path.Count - 1; i++)
            {
                pathLength += Vector2.Distance(path[i], path[i + 1]);
            }
            return pathLength;
        }

        /// <summary>
        /// 计算nav到点的距离
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public static float DistanceNavPath(NavMeshAgent nav, Vector3 targetPos)
        {
            NavMeshPath path = new NavMeshPath();
            if (nav.CalculatePath(targetPos, path))
            {
                Vector3[] allPoint = new Vector3[path.corners.Length + 2];
                allPoint[0] = nav.transform.position;
                allPoint[allPoint.Length - 1] = targetPos;
                for (int i = 0; i < path.corners.Length; i++)
                {
                    allPoint[i + 1] = path.corners[i];
                }
                return DistanceVector3List(allPoint);
            }
            else
            {
                return float.MaxValue;
            }
        }
        #endregion

        #region save Insert/Add/etc.
        /// <summary>
        /// 添加进字典中
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="replaceSameKey">当存在相同key时，将新的替换进去,否则不进行操作</param>
        public static void AddToDictionary<TKey, TValue>(IDictionary<TKey, TValue> dic, TKey key, TValue value, bool replaceSameKey = true)
        {
            if (dic == null)
                return;
            if (dic.ContainsKey(key))
            {
                if (replaceSameKey)
                {
                    dic[key] = value;
                }
            }
            else
            {
                dic.Add(key, value);
            }
        }

        /// <summary>
        /// 遍历字典
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="call"></param>
        public static void TraversalDictionary<TKey, TValue>(Dictionary<TKey, TValue> dic, System.Action<TKey, TValue> call, bool dontUseForeach = false)
        {
            if (dic != null)
            {
                Dictionary<TKey, TValue>.KeyCollection keys = dic.Keys;
                if (dontUseForeach)
                {
                    TKey[] ks = keys.ToArray();
                    for(int i = 0;i<ks.Length;i++){
                        TValue value = dic[ks[i]];
                        call(ks[i], value);
                    }
                }
                else
                {
                    foreach (TKey key in keys)
                    {
                        TValue value = dic[key];
                        call(key, value);
                    }
                }
            }
        }

        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="call"></param>
        public static void TraversalArray<T>(T[] array, System.Action<T> call)
        {
            if (array != null)
            {
                foreach (T elem in array)
                {
                    call(elem);
                }
            }
        }

        /// <summary>
        /// Pop
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T PopList<T>(List<T> list)
        {
            int lastIndex = list.Count - 1;
            if (lastIndex < 0)
                return default(T);
            T pop = list[lastIndex];
            list.RemoveAt(lastIndex);
            return pop;
        }

        #endregion


        #region compare
        /// <summary>
        /// 比较有误差的值
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="deviation"></param>
        /// <returns></returns>
        public static int CompareDeviation(float A, float B, float deviation)
        {
            if (Mathf.Abs(B - A) <= deviation)
                return 0;
            else
                return A > B ? 1 : -1;
        }
        #endregion
    }



}
