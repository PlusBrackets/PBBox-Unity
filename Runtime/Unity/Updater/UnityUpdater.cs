using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Unity
{
    public struct FixedUpdateParam : ILogicUpdateParameter
    {
        public TimeSpan DeltaTime { get; set; }
    }

    public struct UpdateParam : ILogicUpdateParameter
    {
        public TimeSpan DeltaTime { get; set; }
    }

    [MonoSingletonCreator, MonoSingletonDestroyer]
    public class UnityUpdater : MonoSingleton<UnityUpdater>, ISingletonLifecycle, ILogicUpdater<FixedUpdateParam>, ILogicUpdater<UpdateParam>
    {
        private Collections.SortedMutiLinkedList<ILogicUpdatable<UpdateParam>> m_Updatables;
        private Collections.SortedMutiLinkedList<ILogicUpdatable<FixedUpdateParam>> m_FixedUpdatables;

        private int updateTimes = 0;

        public void OnCreateAsSingleton()
        {
            m_Updatables = new Collections.SortedMutiLinkedList<ILogicUpdatable<UpdateParam>>();
            m_FixedUpdatables = new Collections.SortedMutiLinkedList<ILogicUpdatable<FixedUpdateParam>>();
        }

        public void OnDestroyAsSingleton()
        {
            m_Updatables.Clear();
            m_FixedUpdatables.Clear();
        }

        public void Add(ILogicUpdatable<FixedUpdateParam> updatable)
        {
            m_FixedUpdatables.Add(updatable.UpdateOrder, updatable);
            updatable.IsVaild = true;
            // var _group = m_FixedUpdatables.GetGroup(updatable.UpdateOrder);
            // if (!_group.IsVaild)
            // {
            //     var _set = new HashSet<ILogicUpdatable<FixedUpdateParam>>();
            //     _set.Add(updatable);
            //     m_FixedUpdatables.Add(updatable.UpdateOrder, _set);
            // }
            // else
            // {
            //     _group.Start.Value.Add(updatable);
            // }
        }

        public void Add(ILogicUpdatable<UpdateParam> updatable)
        {
            m_Updatables.Add(updatable.UpdateOrder, updatable);
            updatable.IsVaild = true;
            // var _group = m_Updatables.GetGroup(updatable.UpdateOrder);
            // if (!_group.IsVaild)
            // {
            //     var _set = new HashSet<ILogicUpdatable<UpdateParam>>();
            //     _set.Add(updatable);
            //     m_Updatables.Add(updatable.UpdateOrder, _set);
            // }
            // else
            // {
            //     _group.Start.Value.Add(updatable);
            // }
        }

        public void Remove(ILogicUpdatable<FixedUpdateParam> updatable)
        {
            updatable.IsVaild = false;
            // m_FixedUpdatables.Remove(updatable.UpdateOrder, updatable);
        }

        public void Remove(ILogicUpdatable<UpdateParam> updatable)
        {
            updatable.IsVaild = false;
            // m_Updatables.Remove(updatable.UpdateOrder, updatable);
        }

        public void RemoveImmediate(ILogicUpdatable<UpdateParam> updatable)
        {
            updatable.IsVaild = false;
            m_Updatables.Remove(updatable.UpdateOrder, updatable);
        }

        private void Update()
        {
            UnityEngine.Profiling.Profiler.BeginSample("Update Test");
            if (m_Updatables.Count == 0) return;
            UpdateParam _param = new UpdateParam { DeltaTime = TimeSpan.FromSeconds(Time.deltaTime) };
            ILogicUpdatable<UpdateParam> _item;
            for (var _node = m_Updatables.First; _node != null; _node = _node.Next)
            {
                _item = _node.Value.Item;
                if (_item == null || !_item.IsVaild)
                {
                    m_Updatables.Remove(_node);
                    continue;
                }
                if (_item.IsEnable)
                {
                    _item.OnUpdate(_param);
                }
            }
            // foreach (var _updatableSet in m_Updatables)
            // {
            //     foreach (var _updatable in _updatableSet)
            //     {
            //         if (_updatable.IsEnable)
            //         {
            //             _updatable.OnUpdate(_param);
            //             // updateTimes++;
            //         }
            //     }
            // }
            
            UnityEngine.Profiling.Profiler.EndSample();
            // Log.Debug("times:" + updateTimes);
            // updateTimes = 0;
        }

        private void FixedUpdate()
        {
            UnityEngine.Profiling.Profiler.BeginSample("Fixed Update Test");
            if (m_FixedUpdatables.Count == 0) return;
            FixedUpdateParam _param = new FixedUpdateParam { DeltaTime = TimeSpan.FromSeconds(Time.fixedDeltaTime) };
            foreach (var _updatable in m_FixedUpdatables)
            {
                if (_updatable.IsEnable)
                {
                    _updatable.OnUpdate(_param);
                    updateTimes++;
                }
            }

            UnityEngine.Profiling.Profiler.EndSample();
        }
    }
}