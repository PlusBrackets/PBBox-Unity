// using System.Collections;
// using System.Collections.Generic;

// namespace PBBox.FSM
// {

//     public abstract class StateBase : IState
//     {
//         void IState.Enter(IStateMachine stateMachine, int? previousState)
//         {
//             OnEnter(stateMachine, previousState);
//         }

//         void IState.Exit(IStateMachine stateMachine, int? nextState)
//         {
//             OnExit(stateMachine, nextState);
//         }

//         void IState.Update(IStateMachine stateMachine, float deltaTime)
//         {
//             OnUpdate(stateMachine, deltaTime);
//         }

//         protected virtual void OnEnter(IStateMachine stateMachine, int? previousState) { }
//         protected virtual void OnExit(IStateMachine stateMachine, int? nextState) { }
//         protected virtual void OnUpdate(IStateMachine stateMachine, float deltaTime) { }
//     }

//     //复合状态
    
// }