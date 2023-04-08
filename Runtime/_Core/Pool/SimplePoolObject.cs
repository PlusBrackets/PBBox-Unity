/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
using UnityEngine.Events;

namespace PBBox
{
    /// <summary>
    /// 继承IPoolOpbject，结构体等值类型可避免装箱
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [AddComponentMenu("")]
    public abstract class SimplePoolObject<T> : SimplePoolObject, IPoolObject<T>
    {
        void IPoolObject<T>.OnSpawned(T data)
        {
            this.OnSpawned(data);
        }

        protected virtual void OnSpawned(T data) { }
    }

    [AddComponentMenu("PBBox/Pool/Simple Pool Object")]
    public class SimplePoolObject : MonoBehaviour, IPoolObject
    {
        public class PoolObjectEvent : UnityEvent<SimplePoolObject> { }

        SimplePool IPoolObject.Pool { get; set; }
        [SerializeField]
        private float m_LifeTime = -1f;
        public float lifeTime
        {
            get
            {
                return m_LifeTime;
            }
            set
            {
                m_LifeTime = value;
                if (lifeTimer.state == GameTimer.State.Started)
                {
                    if (value < 0f) lifeTimer.Stop();
                    lifeTimer.duration = value;
                }
                else
                {
                    if (m_LifeTime > Mathf.Epsilon) lifeTimer.Start(m_LifeTime);
                }
            }
        }
        public float recycleDelay = 0f;
        public bool delayInGameTime = true;
        [Tooltip("在disable时立刻回收")]
        public bool recycleInDisable = false;
        [SerializeField]
        protected GameDualityTimer lifeTimer = new GameDualityTimer();
        [SerializeField]
        protected PoolObjectEvent m_OnSpawnedEvent;
        public PoolObjectEvent onSpawnedEvent
        {
            get
            {
                if (m_OnSpawnedEvent == null)
                {
                    m_OnSpawnedEvent = new PoolObjectEvent();
                }
                return m_OnSpawnedEvent;
            }
        }
        [SerializeField]
        protected PoolObjectEvent m_OnDespawnedEvent;
        public PoolObjectEvent onDespawnedEvent
        {
            get
            {
                if (m_OnDespawnedEvent == null)
                {
                    m_OnDespawnedEvent = new PoolObjectEvent();
                }
                return m_OnDespawnedEvent;
            }
        }

        public event System.Action<SimplePoolObject> onLifeEndEvent;

        ActionQueue m_RecycleDelayAQ;
        bool m_IsInRecycle = false;

        void IPoolObject.OnSpawned(object data)
        {
            m_IsInRecycle = false;
            if (lifeTime >= 0f)
            {
                lifeTimer.Start(lifeTime);
            }

            // this.pool = pool;
            OnSpawned(data);
            m_OnSpawnedEvent?.Invoke(this);
        }

        void IPoolObject.OnDespawned()
        {
            if (!m_IsInRecycle)
            {
                OnLifeEnd();
                onLifeEndEvent?.Invoke(this);
            }
            m_IsInRecycle = true;
            m_RecycleDelayAQ?.Cancel();
            lifeTimer.Stop();
            OnDespawned();
            m_OnDespawnedEvent?.Invoke(this);
        }

        protected virtual void LateUpdate()
        {
            if (lifeTimer.IsOver())
            {
                EndLife();
                lifeTimer.Stop();
            }
        }

        protected virtual void OnDisable()
        {
            if (recycleInDisable && (this as IPoolObject).Pool != null)
            {
                this.RecycleSelf();
            }
        }

        protected virtual void OnSpawned(object data) { }

        protected virtual void OnDespawned() { }

        public virtual void EndLife()
        {
            if (m_IsInRecycle)
                return;
            if (recycleDelay > 0)
            {
                m_IsInRecycle = true;
                if (m_RecycleDelayAQ == null)
                {
                    m_RecycleDelayAQ = new ActionQueue(this);
                }
                OnLifeEnd();
                onLifeEndEvent?.Invoke(this);
                m_RecycleDelayAQ.DelayGameTime(recycleDelay, !delayInGameTime).Do(this.RecycleSelf).Execute();
            }
            else
                this.RecycleSelf();
        }

        protected virtual void OnLifeEnd(){

        }
    }
}