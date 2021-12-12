using System;
using UnityEngine;

namespace GameKit
{
    public interface IStateContext
    {
        void TransitionTo(IGameState state);
    }
    
    public delegate void EventStateChangedDelegate(IGameState state);
    
    public class StateContext : IStateContext
    {
        private Action _sequenceComplete;
        private IGameState _currentState;

        public event EventStateChangedDelegate EventStateStarted = delegate { };
        public event EventStateChangedDelegate EventStateStopped = delegate { };
        
        public IGameState CurrentState => _currentState;
        
        public bool IsCurrent<T>()
        {
            return _currentState is T;
        }

        public void TransitionTo<T>() where T : IGameState, new()
        {
            TransitionTo(new T());
        }
        
        public void TransitionTo(IGameState state)
        {
            if (state == null) throw new NullReferenceException("State cannot be null");
            state.Context = this;
            TransitionProcess(_currentState, state);
        }
        
        private void TransitionProcess(IGameState from, IGameState to)
        {
            if (from is IGameStateUpdate updateState1)
                Loop.EventUpdate -= updateState1.OnUpdate;
            
            if (from is IGameStateStop stopState)
            {
                if (Debug.isDebugBuild) Debug.Log($"State {from.GetType().Name} OnExit");
                stopState.OnStop();
            }
            
            if (from != null) EventStateStopped(from);

            _currentState = to;

            if (to is IGameStateStart startState)
            {
                if (Debug.isDebugBuild) Debug.Log($"State {to.GetType().Name} OnEnter");
                startState.OnStart();
            }
            
            if (to is IGameStateUpdate updateState2)
                Loop.EventUpdate += updateState2.OnUpdate;

            EventStateStarted(to);
        }
    }
}