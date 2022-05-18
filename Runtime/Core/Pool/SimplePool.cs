/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;
using UnityEngine;

namespace PBBox
{
    public interface IPoolObject
    {
        SimplePool pool { get; set; }
        void OnSpawned(object[] datas);
        void OnDespawned();
    }
    
    internal static class IPoolObjectExtensions
    {
        internal static void _Spawn(this IPoolObject target, SimplePool pool, object[] datas)
        {
            target.pool = pool;
            target.OnSpawned(datas);
        }

        internal static void _Despawn(this IPoolObject target)
        {
            target.pool = null;
            target.OnDespawned();
        }

        public static void RecycleSelf(this IPoolObject target)
        {
            var c = target as Component;
            if (!c)
                return;
            var obj = c.gameObject;
            if (target.pool != null && !target.pool.isDestroyed)
                target.pool.Recycle(obj);
            else
            {
                DebugUtils.Internal.Log($"[{obj?.name} 没有对应的pool ]");
                GameObject.Destroy(obj);
            }
        }
    }

    //TODO 等待优化,让其可支持普通类，Monobehaviour，GameObject
    public sealed class SimplePool
    {

        public struct SpawnParam
        {
            public Transform parent;
            public Vector3? position;
            public Quaternion? rotation;
            public Vector3? scale;
            public Vector3? scaleMultiply;
            public bool? worldPosStay;
        }

        private struct PoolObject
        {
            public bool isSpawned;
            public GameObject obj;
            public IPoolObject[] componentCaches;
        }

        private GameObject m_Original;
        public GameObject original => m_Original;
        private List<PoolObject> m_PoolObjectList = null;
        private List<int> m_NotExistIndex = null;
        private Transform m_Parent;
        public Transform objectParent
        {
            get
            {
                // if (!m_Parent && autoCreatePoolParent)
                // {
                //     m_Parent = new GameObject($"[SimplePool][{m_Original.name}]").transform;
                // }
                return m_Parent;
            }
            set => m_Parent = value;
        }

        private Vector3 m_ObjDefaultPos, m_ObjDefaultScale;
        private Quaternion m_ObjDefaultRot;
        /// <summary>
        /// 当obj unactive时标志回收，否则需要使用Recycle回收
        /// </summary>
        public bool recycleUnactive = false;
        private int m_PoolSize = 10;
        /// <summary>
        /// 对象池大小
        /// </summary>
        public int poolSize
        {
            get
            {
                return m_PoolSize;
            }
            set
            {
                m_PoolSize = Mathf.Max(value, 1);
            }
        }
        /// <summary>
        /// 当池中的object上限时，是否可以创建新的object，否则先进先出
        /// </summary>
        public bool canExpand = true;
        public bool isDestroyed { get; private set; } = false;
        // /// <summary>
        // /// 若为true，则在objectParent为空时自动创建parent
        // /// </summary>
        // public bool autoCreatePoolParent = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original">对象原型</param>
        /// <param name="parent">生成时默认的父组件</param>
        /// <param name="preInit">预创建个数</param>
        /// <param name="poolSize">对象池大小</param>
        /// <param name="canExpand">是否可以扩展对象池</param>
        /// <param name="recycleUnactive">是否允许回收非active的对象</param>
        public SimplePool(GameObject original, Transform parent = null, int preInit = 1, int poolSize = 10, bool canExpand = true, bool recycleUnactive = false)
        {
            m_Parent = parent;
            m_PoolObjectList = new List<PoolObject>();
            m_Original = original;
            this.poolSize = poolSize;
            this.canExpand = canExpand;
            this.recycleUnactive = recycleUnactive;
            m_ObjDefaultPos = original.transform.position;
            m_ObjDefaultRot = original.transform.rotation;
            m_ObjDefaultScale = original.transform.localScale;
            for (int i = 0; i < preInit; i++)
            {
                CreateObj();
            }
        }

        private int CreateObj()
        {
            if (m_Original != null)
            {
                var obj = GameObject.Instantiate(m_Original, objectParent);
                PoolObject po = new PoolObject
                {
                    obj = obj,
                    isSpawned = false,
                    componentCaches = obj.GetComponents<IPoolObject>()
                };
                po.obj.SetActive(false);
                m_PoolObjectList.Add(po);
                return m_PoolObjectList.Count - 1;
            }
            return -1;
        }

        private int GetASpawnableIndex()
        {
            int index = -1;
            //获取可用object的index
            for (int i = 0; i < m_PoolObjectList.Count; i++)
            {
                var po = m_PoolObjectList[i];
                if (!po.obj)
                {
                    if (m_NotExistIndex == null)
                        m_NotExistIndex = new List<int>();
                    m_NotExistIndex.Add(i);
                    continue;
                }
                if (CheckDespawned(po))
                {
                    index = i;
                    break;
                }
            }
            //清除不存在的object
            if (m_NotExistIndex != null)
            {
                foreach (var idx in m_NotExistIndex)
                {
                    m_PoolObjectList.RemoveAt(idx);
                }
                m_NotExistIndex?.Clear();
            }

            if (index < 0)
            {
                if (m_PoolObjectList.Count < poolSize || canExpand)
                {
                    index = CreateObj();
                }
                else
                {
                    index = 0;
                }

            }
            return index;
        }

        private bool CheckDespawned(PoolObject po)
        {
            return !po.isSpawned || (recycleUnactive && !po.obj.activeInHierarchy);
        }

        public T Spawn<T>(params object[] datas)
        {
            return Spawn<T>(null, datas);
        }

        public GameObject Spawn(params object[] datas)
        {
            return Spawn(null, datas);
        }

        public T Spawn<T>(SpawnParam? spawnParam, params object[] datas)
        {
            var obj = Spawn(spawnParam, datas);
            return obj ? obj.GetComponent<T>() : default(T);
        }

        public GameObject Spawn(SpawnParam? spawnParam, params object[] datas)
        {
            if (isDestroyed)
            {
                DebugUtils.LogError("对象池已被销毁");
                return null;
            }
            int index = GetASpawnableIndex();
            var po = m_PoolObjectList[index];
            if (po.isSpawned)
            {
                Recycle(index);
            }

            m_PoolObjectList.RemoveAt(index);
            po.isSpawned = true;
            m_PoolObjectList.Add(po);
            var obj = po.obj;
            obj.transform.position = m_ObjDefaultPos;
            obj.transform.rotation = m_ObjDefaultRot;
            obj.transform.localScale = m_ObjDefaultScale;
            if (spawnParam.HasValue)
            {
                var sp = spawnParam.Value;
                if (sp.scale.HasValue) obj.transform.localScale = sp.scale.Value;
                if (sp.scaleMultiply.HasValue) obj.transform.localScale = Vector3.Scale(obj.transform.localScale, sp.scaleMultiply.Value);
                if (sp.parent)  obj.transform.SetParent(sp.parent, sp.worldPosStay.HasValue ? sp.worldPosStay.Value : false);
                if (sp.position.HasValue) obj.transform.position = sp.position.Value;
                if (sp.rotation.HasValue) obj.transform.rotation = sp.rotation.Value;
            }
            obj.SetActive(true);
            // obj.SendMessage("OnSpawned", datas, SendMessageOptions.DontRequireReceiver);
            foreach (var component in po.componentCaches)
            {
                component._Spawn(this, datas);
            }
            return obj;
        }

        public void Recycle(GameObject obj)
        {
            int index = m_PoolObjectList.FindIndex(po => po.obj == obj);
            if (index >= 0)
            {
                Recycle(index);
            }
        }

        private void Recycle(int index)
        {
            var po = m_PoolObjectList[index];
            if (po.obj.transform.parent != objectParent && po.obj.transform.parent.gameObject.activeInHierarchy)
            {
                po.obj.transform.parent = objectParent;
            }
            po.isSpawned = false;
            m_PoolObjectList[index] = po;
            foreach (var component in po.componentCaches)
            {
                component._Despawn();
            }
            po.obj.SetActive(false);
        }

        /// <summary>
        /// 清空pool对象
        /// </summary>
        public void Clear()
        {
            Remain(m_PoolObjectList.Count);
        }

        /// <summary>
        /// 清理pool对象，最多留下poolsize的数量
        /// </summary>
        public void Clean()
        {
            Remain(m_PoolObjectList.Count - poolSize);
        }

        /// <summary>
        /// 留下count的数量的对象，其余的destory
        /// </summary>
        /// <param name="count"></param>
        public void Remain(int count)
        {
            count = Mathf.Clamp(count, 0, m_PoolObjectList.Count);
            if (count == 0)
                return;
            int startIndex = m_PoolObjectList.Count - count;
            for (int i = startIndex; i < m_PoolObjectList.Count; i++)
            {
                var po = m_PoolObjectList[i];
                GameObject.Destroy(po.obj);
            }
            m_PoolObjectList.RemoveRange(startIndex, count);
        }

        public void DestoryPool()
        {
            m_Original = null;
            m_PoolObjectList.Clear();
            isDestroyed = true;
        }

    }
}