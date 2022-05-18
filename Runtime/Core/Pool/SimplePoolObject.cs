/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
using UnityEngine.Events;

namespace PBBox
{
    public class SimplePoolObject : MonoBehaviour, IPoolObject
    {
        public class PoolObjectEvent : UnityEvent<SimplePoolObject> { }

        SimplePool IPoolObject.pool { get; set; }
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
                    lifeTimer.duration = value;
                else
                {
                    if (m_LifeTime > 0f)
                    {
                        lifeTimer.Start(m_LifeTime);
                    }
                }
            }
        }
        public float recycleDelay = 0f;
        public bool delayInGameTime = true;
        [Tooltip("在disable时立刻回收")]
        public bool recycleInDisable = false;
        protected GameTimer lifeTimer = new GameTimer();
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

        private ActionQueue m_RecycleDelayAQ;

        void IPoolObject.OnSpawned(object[] datas)
        {
            if (lifeTime >= 0f)
            {
                lifeTimer.Start(lifeTime);
            }

            // this.pool = pool;
            OnSpawned(datas);
            m_OnSpawnedEvent?.Invoke(this);
        }

        void IPoolObject.OnDespawned()
        {
            lifeTimer.Stop();
            OnDespawned();
            m_OnDespawnedEvent?.Invoke(this);
            m_RecycleDelayAQ?.Cancel();
        }

        protected virtual void LateUpdate()
        {
            if (lifeTimer.IsOver())
            {
                OnLifeTimeEnd();
                lifeTimer.Stop();
            }
        }

        protected virtual void OnDisable()
        {
            if (recycleInDisable && (this as IPoolObject).pool != null)
            {
                this.RecycleSelf();
            }
        }

        protected virtual void OnSpawned(object[] datas)
        {

        }

        protected virtual void OnDespawned()
        {
        }

        protected virtual void OnLifeTimeEnd()
        {
            if (recycleDelay > 0)
            {
                if(m_RecycleDelayAQ==null){
                    m_RecycleDelayAQ = new ActionQueue(this);
                }
                m_RecycleDelayAQ.DelayGameTime(recycleDelay,!delayInGameTime).Do(this.RecycleSelf).Execute();
            }
            else
                this.RecycleSelf();
        }
    }
}