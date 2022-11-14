using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PBBox.StateMachine
{
    public interface IFsm<TOwner> where TOwner : class
    {
        Dictionary<string, FsmState<TOwner>> States { get; }
        TOwner Owner { get; }
    }

    public sealed class Fsm<TOwner> : IFsm<TOwner>, IAttachDatas where TOwner : class
    {
        public Dictionary<string, FsmState<TOwner>> States { get; }
        public TOwner Owner { get; private set; }
        public string CurrentStateName { get; private set; } = null;
        private Lazy<Dictionary<string, object>> _AttachDatas;
        public Dictionary<string, object> AttachDatas => _AttachDatas.Value;
        public FsmState<TOwner> this[string stateName]
        {
            get => GetState(stateName);
            set => SetState(stateName, value);
        }

        public void Start(string stateName)
        {

        }

        public void Stop()
        {

        }

        public void ChangeToState(string stateName)
        {
            if (!States.TryGetValue(stateName, out var nextState))
            {
                DebugUtils.LogError("[Fsm] Invaild State!");
                return;
            }
            if (!string.IsNullOrEmpty(CurrentStateName))
            {
                States[CurrentStateName].OnExit(this, stateName);
            }
            var _currentStateName = CurrentStateName;
            CurrentStateName = stateName;
            nextState.OnEnter(this, _currentStateName);
        }

        public bool HasState(string stateName)
        {
            return States.ContainsKey(stateName);
        }

        public void SetState(string stateName, FsmState<TOwner> state)
        {
            bool isUpdating = string.Equals(stateName, CurrentStateName);
            if (States.TryGetValue(stateName, out var prevState))
            {
                if (isUpdating)
                {
                    prevState.OnExit(this, stateName);
                }
                prevState.OnDestroy(this);
            }
            States[stateName] = state;
            state.OnInit(this);
            if (isUpdating)
            {
                state.OnEnter(this, stateName);
            }
        }

        public FsmState<TOwner> GetState(string stateName)
        {
            FsmState<TOwner> state = null;
            States.TryGetValue(stateName, out state);
            return state;
        }
    }
}