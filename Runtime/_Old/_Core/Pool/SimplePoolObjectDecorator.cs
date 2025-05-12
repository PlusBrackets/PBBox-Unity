using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox
{
    public abstract class SimplePoolObjectDecorator : MonoBehaviour
    {
        [SerializeField]
        private SimplePoolObject m_Target;
        public SimplePoolObject Target => m_Target;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (m_Target == null && !UnityEditor.EditorApplication.isPlaying)
            {
                m_Target = GetComponent<SimplePoolObject>();
            }
        }
#endif

        protected virtual void Awake()
        {
            if (m_Target == null)
            {
                m_Target = GetComponent<SimplePoolObject>();
            }
            m_Target.OnSpawnedEvent += OnSpawned;
            m_Target.OnDespawnedEvent += OnDespawned;
            m_Target.OnLifeEndEvent += OnEndLife;
        }

        protected virtual void OnDestroy()
        {
            if (m_Target == null)
            {
                return;
            }
            m_Target.OnSpawnedEvent -= OnSpawned;
            m_Target.OnDespawnedEvent -= OnDespawned;
            m_Target.OnLifeEndEvent -= OnEndLife;
        }

        protected virtual void OnSpawned(IPoolObject target) { }
        protected virtual void OnDespawned(IPoolObject target) { }
        protected virtual void OnEndLife(SimplePoolObject target) { }

    }
}