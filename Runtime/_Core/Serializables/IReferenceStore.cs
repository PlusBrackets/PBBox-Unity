/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.11.17
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;
using UnityEngine;
using PBBox.Collections;

namespace PBBox
{
    public interface IReferenceStore
    {
        SDictionary<string, Object> GetReferenceMap();
        SDictionary<string, Object[]> GetReferenceArrayMap();
        Dictionary<string, Component> GetComponentStorage();
    }

    public static partial class PBExtensions
    {
        /// <summary>
        /// 获取一个Reference
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetReference<T>(this IReferenceStore target, string key) where T : Object
        {
            var map = target.GetReferenceMap();
            if (map == null)
            {
                return null;
            }
            if (map.TryGetValue(key, out var r))
            {
                return r as T;
            }
            else{
                Log.Error(map.Count);
                foreach(var kvp in map){
                    Log.Error(kvp);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取一个Reference列表
        /// </summary>
        /// <param name="target"></param>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        // public static T[] GetReferenceArray<T>(this IReferenceStore target, string key) where T : Object
        // {
        //     var map = target.GetReferenceArrayMap();
        //     if (map == null) return null;
        //     if (map.TryGetValue(key, out var rs))
        //     {
        //         return rs as T[];
        //     }
        //     return null;
        // }

        /// <summary>
        /// 获取一个Reference作为GameObject
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static GameObject GetReference(this IReferenceStore target, string key)
        {
            return GetReference<GameObject>(target, key);
        }

        /// <summary>
        /// 获取一个Reference列表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Object[] GetReferenceArray(this IReferenceStore target, string key)
        {
            var map = target.GetReferenceArrayMap();
            if (map == null) return null;
            if (map.TryGetValue(key, out var rs))
            {
                return rs;
            }
            return null;
        }


        /// <summary>
        /// 从References中获取一个Component
        /// </summary>
        /// <param name="key"></param>
        /// <param name="store">是否存下来以备后续使用</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRefComponent<T>(this IReferenceStore target, string key, bool store = true) where T : Component
        {
            var m_StoredComponents = target.GetComponentStorage();
            string cKey = key + "_" + typeof(T).FullName;
            if (!m_StoredComponents.TryGetValue(cKey, out var component))
            {
                var obj = GetReference<Object>(target, key);
                if (obj == null)
                {
                    return null;
                }
                if (obj is T)
                {
                    component = obj as T;
                }
                else if (obj is Component)
                {
                    component = (obj as Component).GetComponent<T>();
                }
                else if (obj is GameObject)
                {
                    component = (obj as GameObject).GetComponent<T>();
                }
                if (component == null)
                    return null;
                if (store)
                {
                    StoreRefComponent(target, key, component as T);
                }
            }
            return component as T;
        }

        /// <summary>
        /// 储存一个自身Component以备后用
        /// </summary>
        /// <param name="key"></param>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        public static void StoreRefComponent<T>(this IReferenceStore target, string key, T component) where T : Component
        {
            var m_StoredComponents = target.GetComponentStorage();
            string cKey = key + "_" + typeof(T).FullName;
            m_StoredComponents[cKey] = component;
        }
    }
}
