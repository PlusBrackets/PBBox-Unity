using System.Collections;
using System.Collections.Generic;

namespace PBBox.StateMachine
{
    public interface IMachine
    {
        bool IsPaused { get; }
        bool IsRunning { get; }
        bool IsDestroyed { get; }
        
        void Start(int stateId);
        void Update(float deltaTime);
        void Stop();
        void Pause();
        void Resume();
        void ChangeToState(int stateId);
    }

    public interface IMachine<T> : IMachine
    {
        T Owner { get; }
        IState<T> CurrentState { get; }
        void AddState(int id, IState<T> state);
        void RemoveState(int id);
        IState<T> GetState(int id);
        bool HasState(int id);
    }
}