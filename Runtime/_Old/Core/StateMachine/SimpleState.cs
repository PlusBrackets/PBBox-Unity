/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.02.19
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox.FSM
{
    public sealed class SimpleState : IState, IReferencePoolItem
    {
        public Action<IStateMachine, int?> OnEnter { get; set; } = null;
        public Action<IStateMachine, int?> OnExit { get; set; } = null;
        public Action<IStateMachine, float> OnUpdate { get; set; } = null;

        public SimpleState(){}

        public static SimpleState Create(Action<IStateMachine, int?> onEnter = null, Action<IStateMachine, int?> onExit = null, Action<IStateMachine, float> onUpdate = null)
        {
            var _state = ReferencePool.Acquire<SimpleState>();
            _state.OnEnter = onEnter;
            _state.OnExit = onExit;
            _state.OnUpdate = onUpdate;
            return _state;
        }

        public static void Release(SimpleState state)
        {
            ReferencePool.Release(state);
        }

        bool IReferencePoolItem.IsUsing { get; set; }

        void IReferencePoolItem.OnReferenceAcquire() { }

        void IReferencePoolItem.OnReferenceRelease()
        {
            OnEnter = null;
            OnExit = null;
            OnUpdate = null;
        }

        void IState.Enter(IStateMachine stateMachine, int? previousState)
        {
            OnEnter?.Invoke(stateMachine, previousState);
        }

        void IState.Exit(IStateMachine stateMachine, int? nextState)
        {
            OnExit?.Invoke(stateMachine, nextState);
        }

        void IState.Update(IStateMachine stateMachine, float deltaTime)
        {
            OnUpdate?.Invoke(stateMachine, deltaTime);
        }
    }
}