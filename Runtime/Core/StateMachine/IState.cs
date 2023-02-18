using System.Collections;
using System.Collections.Generic;

namespace PBBox.FSM
{
    public interface IState
    {
        void Enter(IStateMachine stateMachine, int previousState);
        void Exit(IStateMachine stateMachine, int nextState);
        void Update(IStateMachine stateMachine, float deltaTime);
    }
}