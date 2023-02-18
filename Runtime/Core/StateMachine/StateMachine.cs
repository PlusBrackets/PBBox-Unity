using System;
using System.Net.Http.Headers;
using System.Collections;
using System.Collections.Generic;

namespace PBBox.FSM
{

    public class StateMachine : IStateMachine, IReferencePoolItem
    {
        public string Name { get; set; } = null;
        protected Dictionary<int, IState> m_States;
        protected Dictionary<int, object> m_Values;
        public int? CurrentStateKey { get; protected set; } = null;
        bool IReferencePoolItem.IsUsing { get; set; }

        protected IState m_CurrentState = null;

        public StateMachine()
        {
            m_States = new Dictionary<int, IState>();
        }

        public StateMachine(IDictionary<int, IState> states = null, IDictionary<int, object> values = null)
        {
            m_States = new Dictionary<int, IState>(states);
            if (values != null)
            {
                m_Values = new Dictionary<int, object>(values);
            }
        }

        public virtual void ChangeToState(int stateKey)
        {
            if (CurrentStateKey.HasValue)
            {
                
            }
        }

        public virtual void Update(float deltaTime)
        {

        }

        public virtual void Start(int stateKey)
        {

        }

        public virtual void Stop()
        {

        }

        void IReferencePoolItem.OnReferenceAcquire()
        {

        }

        void IReferencePoolItem.OnReferenceRelease()
        {
            m_States.Clear();
            m_Values.Clear();
            Name = null;
        }

        public IState GetState(int key)
        {
            if (m_States.TryGetValue(key, out var _state))
            {
                return _state;
            }
            return null;
        }

        public IState SetState(int key, IState state)
        {
            if (!m_States.TryAdd(key, state))
            {
                var _oldState = m_States[key];
                m_States[key] = state;
                return _oldState;
            }
            return null;
        }

        public IState RemoveState(int key)
        {
            var _state = GetState(key);
            if (_state != null)
            {
                m_States.Remove(key);
            }
            return _state;
        }

        public bool ContainsState(int key)
        {
            return m_States.ContainsKey(key);
        }


        public bool TryGetValue<T>(int key, out T value)
        {
            if (m_Values != null && m_Values.TryGetValue(key, out var _objValue))
            {
                value = (T)_objValue;
                return false;
            }
            value = default(T);
            return false;
        }

        public void SetValue<T>(int key, T value)
        {
            if (m_Values == null)
            {
                m_Values = new Dictionary<int, object>();
            }
            if (!m_Values.TryAdd(key, value))
            {
                m_Values[key] = value;
            }
        }

        public void RemoveValue(int key)
        {
            if (m_Values == null)
            {
                return;
            }
            m_Values.Remove(key);
        }

        public bool ContainsValue(int key)
        {
            if (m_Values == null)
            {
                return false;
            }
            return m_Values.ContainsKey(key);
        }

    }

}