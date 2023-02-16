using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Unity
{
    [MonoSingletonCreator(HideFlags = HideFlags.HideAndDontSave), MonoSingletonDestroyer]
    internal class UnityLifecycleReference : MonoSingleton<UnityLifecycleReference>
    {
        internal event Action onUpdate;
        internal event Action onFixedUpdate;
        internal event Action onLateUpdate;

        private void Update()
        {
            onUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            onFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            onLateUpdate?.Invoke();
        }
    }
}