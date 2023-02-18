using System.Collections;
using System.Collections.Generic;

namespace PBBox.FSM
{
    public interface IStateMachine
    {
        string Name { get; }
        int? CurrentStateKey { get; }
        
        IState GetState(int key);
        IState SetState(int key, IState state);
        IState RemoveState(int key);
        bool ContainsState(int key);
        
        bool TryGetValue<T>(int key, out T value);
        void SetValue<T>(int key, T value);
        void RemoveValue(int key);
        bool ContainsValue(int key);

        void ChangeToState(int key);
        void Update(float deltaTime);
        void Start(int key);
        void Stop();
    }
}
