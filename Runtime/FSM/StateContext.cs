using System;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameKit
{
    public delegate void EventStateChangedDelegate(State state);
    
    [PublicAPI]
    public class StateContext: IDisposable
    {
        private Action _sequenceComplete;
        private State _current;

        public event EventStateChangedDelegate EventStateInitialize;
        public event EventStateChangedDelegate EventStateStarted;
        public event EventStateChangedDelegate EventStateStopped;
        public event Action EventContextExit;
        public State Current => _current;
        
        public bool CurrentStateIs<T>()
        {
            return _current is T;
        }

        public void ChangeTo(State state)
        {
            if (state == null) throw new NullReferenceException("State cannot be null");
            state.Context = this;
            TransitionProcess(state);
        }

        public async void Exit()
        {
            if (_current is not null)
            {
                try
                {
                    State c = _current;
                    _current = null;
                    await c.Exit();
                    c.Release();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            
            EventContextExit?.Invoke();
        }
        
        private async void TransitionProcess(State to)
        {
            try
            {
                if (_current is not null)
                {
                    await _current.Exit(); 
                    EventStateStopped?.Invoke(_current);
                    _current.Release();
                }

                _current = to;
                _current.Initialize();
                EventStateInitialize?.Invoke(_current);
                _current.Enter();
                EventStateStarted?.Invoke(_current);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public StateContext()
        {
            Loop.EventDispose += Dispose;
        }
        
        public void Dispose()
        {
            if (Current is not null)
            {
                State c = _current;
                _current = null;
                c.Release();
            }
        }
    }
}