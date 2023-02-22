/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.02.19
 *@author: PlusBrackets
 --------------------------------------------------------*/
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

        public StateMachine()
        {
            m_States = new Dictionary<int, IState>();
            CurrentStateKey = null;
        }

        public static StateMachine Create(IDictionary<int, IState> states = null, IDictionary<int, object> values = null)
        {
            var _machine = ReferencePool.Acquire<StateMachine>();
            if (states != null)
            {
                foreach (var _stateKvp in states)
                {
                    _machine.m_States.TryAdd(_stateKvp.Key, _stateKvp.Value);
                }
            }
            if (values != null)
            {
                if (_machine.m_Values == null)
                {
                    _machine.m_Values = new Dictionary<int, object>();
                }
                else
                {
                    foreach (var _valueKvp in values)
                    {
                        _machine.m_Values.TryAdd(_valueKvp.Key, _valueKvp.Value);
                    }
                }
            }
            return _machine;
        }

        public static void Release(StateMachine machine, bool tryReleaseState)
        {
            machine.Stop();
            foreach (var _stateKvp in machine.m_States)
            {
                if (_stateKvp.Value is IReferencePoolItem _state)
                {
                    ReferencePool.Release(_state);
                }
            }
            ReferencePool.Release(machine);
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
            IState _nextState = GetState(stateKey);
            if (_nextState == null)
            {
                Log.Error(
                    $"Can not change! State[{stateKey}] is not found.",
                    "FSM",
                    Log.PBBoxLoggerName);
                return;
            }
            IState _previousState = null;
            if (CurrentStateKey.HasValue)
            {
                _previousState = GetState(CurrentStateKey.Value);
            }
            var _prevoursKey = CurrentStateKey;
            _previousState?.Exit(this, stateKey);
            CurrentStateKey = stateKey;
            _nextState.Enter(this, _prevoursKey);
        }

        public virtual void Update(float deltaTime)
        {
            if (!CurrentStateKey.HasValue)
            {
                return;
            }
            IState _currentState = GetState(CurrentStateKey.Value);
            if (_currentState == null)
            {
                return;
            }
            _currentState.Update(this, deltaTime);
        }

        public virtual void Start(int stateKey)
        {
            if (CurrentStateKey.HasValue)
            {
                Log.Warning(
                    $"This Machine[{(!string.IsNullOrEmpty(Name) ? Name : this.GetType())}] has areadly stated",
                    "FSM",
                    Log.PBBoxLoggerName);
                return;
            }
            IState _state = GetState(stateKey);
            if (_state == null)
            {
                Log.Error(
                    $"Can not start! State[{stateKey}] is not found.",
                    "FSM",
                    Log.PBBoxLoggerName);
                return;
            }
            CurrentStateKey = stateKey;
            _state.Enter(this, null);
        }

        public virtual void Stop()
        {
            if (!CurrentStateKey.HasValue)
            {
                return;
            }
            IState _state = GetState(CurrentStateKey.Value);
            _state.Exit(this, null);
            CurrentStateKey = null;
        }

        void IReferencePoolItem.OnReferenceAcquire() { }

        void IReferencePoolItem.OnReferenceRelease()
        {
            if(CurrentStateKey.HasValue){
                Stop();
            }
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
                if (CurrentStateKey.HasValue && CurrentStateKey.Value == key)
                {
                    _oldState.Exit(this, key);
                    m_States[key] = state;
                    state.Enter(this, key);
                }
                else
                {
                    m_States[key] = state;
                }
                return _oldState;
            }
            return null;
        }

        public IState RemoveState(int key)
        {
            if (CurrentStateKey.HasValue && CurrentStateKey.Value == key)
            {
                throw new Log.FetalErrorException($"This state[{key}] is runing, can not remove!", "FSM", Log.PBBoxLoggerName);
            }
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

        
        public Dictionary<int, IState>.Enumerator GetAllStates() => m_States.GetEnumerator();
        IEnumerator<KeyValuePair<int, IState>> IStateMachine.GetAllStates() => GetAllStates();

        public Dictionary<int, object>.Enumerator GetAllValues()
        {
            if (m_Values == null)
            {
                return default(Dictionary<int, object>.Enumerator);
            }
            return m_Values.GetEnumerator();
        }
        IEnumerator<KeyValuePair<int, object>> IStateMachine.GetAllValues() => GetAllValues();

    }

}