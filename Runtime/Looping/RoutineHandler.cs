using System;
using System.Collections;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameKit
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RoutineHandler : CustomYieldInstruction
    {
        private IEnumerator _routine;

        public event Action<RoutineHandler> EventStarting;
        public event Action<RoutineHandler> EventCancelled;
        public event Action<RoutineHandler> EventCompleted;
        public event Action<RoutineHandler> EventReset;

        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsRunnable => !(_routine is null);

        public void Cancel()
        {
            if (IsRunnable)
            {
                Loop.StopCoroutine(_routine);
                EventCancelled?.Invoke(this);
                Clear();
            }
        }

        internal RoutineHandler(IEnumerator routine)
        {
            _routine = routine;
        }
        
        internal IEnumerator Run()
        {
            EventStarting?.Invoke(this);
            yield return _routine;
            EventCompleted?.Invoke(this);
            Clear();
        }

        private void Clear()
        {
            EventReset?.Invoke(this);
            EventCancelled = null;
            EventCompleted = null;
            EventStarting = null;
            EventReset = null;
            _routine = null;
        }

        public override bool keepWaiting => IsRunnable;
    }
}