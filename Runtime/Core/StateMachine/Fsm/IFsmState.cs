using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.StateMachine
{
    public abstract class FsmState<TOwner> where TOwner : class
    {
        protected internal virtual void OnInit(IFsm<TOwner> fsm)
        {

        }

        protected internal virtual void OnEnter(IFsm<TOwner> fsm, string fromStateName)
        {

        }

        protected internal virtual void OnUpdate(IFsm<TOwner> fsm, float deltaTime)
        {

        }

        protected internal virtual void OnExit(IFsm<TOwner> fsm, string toStateName)
        {

        }

        protected internal virtual void OnDestroy(IFsm<TOwner> fsm)
        {

        }
    }

}