using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox
{

    public static partial class PBExtensions
    {
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetOrAddComponent<T>();
        }

        public static T ParseOrGetComponent<T>(this Component component)
        {
            if (component is T _t)
            {
                return _t;
            }
            else
            {
                return component.GetComponent<T>();
            }
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
        /// 遍历Transform
        /// </summary>
        /// <param name="target"></param>
        /// <param name="action"></param>
        public static void ForeachChildren(this Transform target, System.Action<Transform> action)
        {
            foreach (Transform _t in target)
            {
                action(_t);
                _t.ForeachChildren(action);
            }
        }

        public static Transform SearchChildren(this Transform target, System.Predicate<Transform> predicate)
        {
            foreach (Transform _t in target)
            {
                if (predicate(_t))
                {
                    return _t;
                }
            }
            foreach (Transform _t in target)
            {
                Transform _result = _t.SearchChildren(predicate);
                if (_result != null)
                {
                    return _result;
                }
            }
            return null;
        }
    }
}