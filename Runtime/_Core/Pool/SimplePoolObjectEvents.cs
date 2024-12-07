using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PBBox
{
    public class SimplePoolObjectEvents : SimplePoolObjectDecorator
    {
        [SerializeField]
        private UnityEvent OnSpawnedEvent = new UnityEvent();
        [SerializeField]
        private UnityEvent OnDespawnedEvent = new UnityEvent();
        [SerializeField]
        private UnityEvent OnEndLifeEvent = new UnityEvent();

        protected override void OnSpawned(IPoolObject target)
        {
            OnSpawnedEvent.Invoke();
        }

        protected override void OnDespawned(IPoolObject target)
        {
            OnDespawnedEvent.Invoke();
        }

        protected override void OnEndLife(SimplePoolObject target)
        {
            OnEndLifeEvent.Invoke();
        }
    }
}