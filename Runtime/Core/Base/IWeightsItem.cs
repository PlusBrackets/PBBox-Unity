/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;

namespace PBBox
{
    public interface IWeightsItem
    {
        float weights{get;set;}
    }

    public interface IWeightsItem<Content> : IWeightsItem
    {
        Content content { get; }
    }
    
    public static class WeightsItemExtensions
    {
        #region IWeightsItem

        /// <summary>
        /// 根据权重随机获取一个Item
        /// </summary>
        /// <param name="target"></param>
        /// <param name="totalWeights"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetWeightsItem<T>(this IEnumerable<T> target, float totalWeights = -1, System.Func<T, bool> canGet = null) where T : IWeightsItem
        {
            var items = GetWeightsItems(target, 1, true, totalWeights, canGet);
            if (items.Count > 0)
                return items[0];
            return default(T);
        }

        /// <summary>
        /// 根据权重随机获取多个Item
        /// </summary>
        /// <param name="target"></param>
        /// <param name="totalWeights"></param>
        /// <param name="count"></param>
        /// <param name="canRepeat"></param>
        /// <param name="canGet"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetWeightsItems<T>(this IEnumerable<T> target, int count, bool canRepeat = true, float totalWeights = -1, System.Func<T, bool> canGet = null) where T : IWeightsItem
        {
            List<T> items = new List<T>();
            var _target = target;
            if (!canRepeat || canGet != null)
            {
                _target = new List<T>(target);
            }
            if (totalWeights < 0)
            {
                totalWeights = target.GetTotalWeight();
            }
            while (count > 0)
            {
                count--;
                float baseWeights = 0;
                float rand = RandomUtils.Range(0f, totalWeights);
                int _index = 0;
                bool _removeItem = false;
                foreach (T item in _target)
                {
                    float w = item.weights;
                    if (w + baseWeights >= rand)
                    {
                        if (canGet == null || canGet.Invoke(item))
                        {
                            items.Add(item);
                            break;
                        }
                        else
                        {
                            _removeItem = true;
                            count++;
                            break;
                        }
                    }
                    _index++;
                    baseWeights += w;
                }
                if ((_removeItem || !canRepeat) && _index < (_target as List<T>).Count)
                {
                    (_target as List<T>).RemoveAt(_index);
                    totalWeights = _target.GetTotalWeight();
                }
            }
            return items;
        }


        /// <summary>
        /// 计算总权重
        /// </summary>
        /// <param name="target"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static float GetTotalWeight<T>(this IEnumerable<T> target) where T : IWeightsItem
        {
            float total = 0;
            foreach (T item in target)
            {
                total += item.weights;
            }
            return total;
        }

        /// <summary>
        /// 根据列表中的权重获取index
        /// </summary>
        /// <param name="target"></param>
        /// <param name="count"></param>
        /// <param name="canRepeat"></param>
        /// <param name="totalWeights"></param>
        /// <returns></returns>
        public static List<int> GetWeightsIndexs(this IList<float> target, int count = 1, bool canRepeat = true, float totalWeights = -1)
        {
            List<int> indexs = new List<int>();
            var _target = target;
            if (!canRepeat)
            {
                _target = new List<float>(target);
            }
            if (totalWeights < 0)
            {
                totalWeights = 0;
                foreach(float f in _target){
                    totalWeights += f;
                }
            }
            while (count > 0 && totalWeights > 0f)
            {
                count--;
                float baseWeights = 0;
                float rand = RandomUtils.Range(0f, totalWeights);
                int _index = 0;
                foreach (float item in _target)
                {
                    float w = item;
                    if (w + baseWeights >= rand)
                    {
                        indexs.Add(_index);
                        break;
                    }
                    _index++;
                    baseWeights += w;
                }
                if (!canRepeat && _index < _target.Count)
                {
                    _target[_index] = 0;
                    totalWeights = 0;
                    foreach (float f in _target)
                    {
                        totalWeights += f;
                    }
                }
            }
            return indexs;
        }

        /// <summary>
        /// 根据列表中的权重获取index
        /// </summary>
        /// <param name="target"></param>
        /// <param name="totalWeights"></param>
        /// <returns></returns>
        public static int GetWeightsIndex(this IList<float> target, float totalWeights = -1)
        {
            var indexs = GetWeightsIndexs(target, 1, true, totalWeights);
            if (indexs.Count > 0)
            {
                return indexs[0];
            }
            return -1;
        }

        #endregion
    }
}