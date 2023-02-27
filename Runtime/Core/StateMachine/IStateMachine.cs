using System.Collections;
using System.Collections.Generic;

namespace PBBox.FSM
{
    public interface IStateMachine
    {
        string Name { get; }
        object Owner { get; }
        int? CurrentState { get; }
        int? EntryState { get; }

        IState GetState(int key);
        IState SetState(int key, IState state);
        IState RemoveState(int key);
        bool ContainsState(int key);

        bool TryGetValue<T>(int key, out T value);
        void SetValue<T>(int key, T value);
        void RemoveValue(int key);
        bool ContainsValue(int key);
        
        IEnumerator<KeyValuePair<int, IState>> GetAllStates();
        IEnumerator<KeyValuePair<int, object>> GetAllValues();

        void ChangeToState(int key);
        void Update(float deltaTime);
        void Start(int? key = null);
        void Stop();
    }
}
