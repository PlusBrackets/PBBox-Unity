using System.Collections;
using System.Collections.Generic;

namespace PBBox.StateMachine
{

    public interface IState<TOwner>
    {
        void OnInit(IMachine<TOwner> machine);
        void OnDestroy(IMachine<TOwner> machine);
        void OnEnter(IMachine<TOwner> machine, int? prefStateId);
        void OnExit(IMachine<TOwner> machine, int? nextStateId);
        void OnUpdate(IMachine<TOwner> machine, float deltaTime);

    }
}