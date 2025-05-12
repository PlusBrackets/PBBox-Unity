/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PBBox
{

    /// <summary>
    /// 对象池组管理,使用original.name标识
    /// </summary>
    public sealed class PoolsManager : SingleClass<PoolsManager>
    {
        public const string Default_GroupName = "Common";

        #region static

        public static GameObject Spawn(GameObject original, SimplePool.SpawnParam? param = null, object data = null)//params object[] datas)
        {
            return Spawn(Default_GroupName, original, param, data);
        }

        public static GameObject Spawn(string origianlName, SimplePool.SpawnParam? param = null, object data = null)//params object[] datas)
        {
            return Spawn(Default_GroupName, origianlName, param, data);
        }

        public static GameObject Spawn(string groupName, GameObject original, SimplePool.SpawnParam? param = null, object data = null)// params object[] datas)
        {
            var _pool = GetPool(groupName, original);
            return _pool.Spawn(param, data);
        }

        public static GameObject Spawn(string groupName, string originalName, SimplePool.SpawnParam? param = null, object data = null)//params object[] datas)
        {
            var _pool = GetPool(groupName, originalName);
            return _pool?.Spawn(param, data);

        }

        public static SimplePool GetPool(GameObject original, Transform poolParent = null)
        {
            return Instance._GetPool(Default_GroupName, original, poolParent);
        }

        public static SimplePool GetPool(string groupName, GameObject original, Transform poolParent = null)
        {
            return Instance._GetPool(groupName, original, poolParent);
        }

        public static SimplePool GetPool(string groupName, string original)
        {
            return Instance._GetPool(groupName, original);
        }

        #endregion

        public struct Group
        {
            public string groupName;
            public Dictionary<string, SimplePool> pools;
            public Transform defaultGroupParent;
        }

        Dictionary<string, Group> groups;

        protected override void Init()
        {
            base.Init();
            groups = new Dictionary<string, Group>();
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private Group _GetGroup(string groupName)
        {
            if (String.IsNullOrEmpty(groupName))
            {
                groupName = Default_GroupName;
                // DebugUtils.Internal.LogWarning("groupName为空，使用默认groupName");
            }
            Group group;
            if (!groups.TryGetValue(groupName, out group))
            {
                group = new Group
                {
                    groupName = groupName,
                    pools = new Dictionary<string, SimplePool>()
                };
                groups.Add(groupName, group);
            }
            return group;
        }

        private SimplePool _GetPool(string groupName, GameObject original, Transform poolParent)
        {
            Group group = _GetGroup(groupName);
            groupName = group.groupName;
            SimplePool pool = null;
            bool got = group.pools.TryGetValue(original.name, out pool);
            if (pool == null || pool.isDestroyed)
            {
                if (got)
                {
                    group.pools.Remove(original.name);
                }
                if (group.defaultGroupParent == null)
                {
                    if (!poolParent)
                    {
                        group.defaultGroupParent = new GameObject($"[PBPools - {groupName}]").transform;
                    }
                    groups[groupName] = group;
                }
                var parent = poolParent ? poolParent : group.defaultGroupParent;
                pool = new SimplePool(original, parent);
                group.pools.Add(original.name, pool);
            }
            return pool;
        }

        private SimplePool _GetPool(string groupName, string originalName)
        {
            Group group = _GetGroup(groupName);
            SimplePool pool = null;
            group.pools.TryGetValue(originalName, out pool);
            if(pool!= null&&pool.isDestroyed)
            {
                group.pools.Remove(originalName);
                pool = null;
            }
            return pool;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            groups = null;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            var groupKeys = groups.Keys.ToArray();
            foreach (string groupKey in groupKeys)
            {
                var group = groups[groupKey];
                var poolKeys = group.pools.Keys.ToArray();
                foreach (string poolKey in poolKeys)
                {
                    var pool = group.pools[poolKey];
                    if (pool.isDestroyed || pool.objectParent == null)
                    {
                        pool.DestoryPool();
                        group.pools.Remove(poolKey);
                    }
                }
                if (group.pools.Count == 0)
                {
                    groups.Remove(groupKey);
                }
            }
        }

    }
}