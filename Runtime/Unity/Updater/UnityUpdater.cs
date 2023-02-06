using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Unity
{
    public struct FixedUpdateParam : IUpdateParameter
    {
        public float DeltaTime { get; set; }
    }

    public struct UpdateParam : IUpdateParameter
    {
        public float DeltaTime { get; set; }
    }

    [MonoSingletonCreator, MonoSingletonDestroyer]
    public class UnityUpdater : MonoSingleton<UnityUpdater>, ISingletonLifecycle
    {

        public Updater<UpdateParam> Updater { get; private set; }
        public Updater<FixedUpdateParam> FixedUpdater { get; private set; }

        public static void Attach(IUpdatable<UpdateParam> updatable)
        {
            Instance.Updater.Attach(updatable);
        }

        public static void Attach(IUpdatable<FixedUpdateParam> updatable)
        {
            Instance.FixedUpdater.Attach(updatable);
        }

        public static void Unattach(IUpdatable<UpdateParam> updatable, bool immediately = false)
        {
            Instance.Updater.Unattach(updatable, immediately);
        }
        
        public static void Unattach(IUpdatable<FixedUpdateParam> updatable, bool immediately = false)
        {
            Instance.FixedUpdater.Unattach(updatable, immediately);
        }

        public void OnCreateAsSingleton()
        {
            Updater = new Updater<UpdateParam>();
            FixedUpdater = new Updater<FixedUpdateParam>();
        }

        public void OnDestroyAsSingleton()
        {
            Updater.Clear();
            FixedUpdater.Clear();
        }

        private void Update()
        {
            UnityEngine.Profiling.Profiler.BeginSample("Update Test");
            UpdateParam _param = new UpdateParam{DeltaTime = Time.deltaTime};
            Updater.Update(ref _param);
            UnityEngine.Profiling.Profiler.EndSample();
        }

        private void FixedUpdate()
        {
            UnityEngine.Profiling.Profiler.BeginSample("Fixed Update Test");
            FixedUpdateParam _param = new FixedUpdateParam{DeltaTime = Time.fixedDeltaTime};
            FixedUpdater.Update(ref _param);
            UnityEngine.Profiling.Profiler.EndSample();
        }
    }
}