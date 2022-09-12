using System;
using System.Collections;
using System.Threading;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace GameKit
{
    public enum RoutineStatus
    {
        Executing,
        Canceled,
        Faulted,
        CompletedSuccessfully
    }

    public readonly struct RoutineResult<T> : IEnumerator
    {
        public readonly T Result;
        public RoutineResult(T result)
        {
            this.Result = result;
        }

        bool IEnumerator.MoveNext() => false;
        void IEnumerator.Reset() { }
        object IEnumerator.Current => null;
    }
    
    
    [PublicAPI]
    public class TaskRoutine<T> : TaskRoutine
    {
        public T Result { get; protected internal set; }
        
        internal TaskRoutine() {}
        internal TaskRoutine(IEnumerator routine) : base(routine) { }

        protected override void Complete()
        {
            if (Routine.Current is null)
            {
                CancelWithError($"{Routine.GetType().Name} return NULL result");
                return;
            }

            if (Routine.Current is RoutineResult<T> routineResult)
            {
                Result = routineResult.Result;
                Complete();
                return;
            }

            if (Routine.Current is T result)
            {
                Result = result;
                Complete();
                return;
            }
            
            CancelWithError($"{Routine.GetType().Name} return UNEXPECTED result ({Routine.Current.GetType().Name}), but required {typeof(T).Name}");
        }
    }
    
    // ReSharper disable once ClassNeverInstantiated.Global
    [PublicAPI]
    public class TaskRoutine : IEnumerator
    {
        // ReSharper disable once InconsistentNaming
        private const string CANCELED = "canceled";
        
        protected IEnumerator Routine;
        private object _asyncState;
        private WaitHandle _asyncWaitHandle;
        private bool _completedSynchronously;
        private bool _isCompleted;

        public event Action<TaskRoutine> EventStarting;
        public event Action<TaskRoutine> EventCancelled;
        public event Action<TaskRoutine, string> EventFaulted;
        public event Action<TaskRoutine> EventCompleted;
        public event Action<TaskRoutine> EventDestroyed;

        // ReSharper disable once MemberCanBePrivate.Global
        public RoutineStatus status { get; protected set; }

        public bool IsExecuting => status == RoutineStatus.Executing;
        public bool IsCanceled => status == RoutineStatus.Canceled;
        public bool IsCompleted => status != RoutineStatus.Executing;
        public bool IsFaulted => status == RoutineStatus.Faulted;
        public bool IsCompletedSuccessfully => status == RoutineStatus.CompletedSuccessfully;
        
        public string Error { get; private set; }
        
        
        public void Cancel()
        {
            if (IsExecuting)
            {
                Loop.StopCoroutine(Routine);
                status = RoutineStatus.Canceled;
                Error = CANCELED;
                EventCancelled?.Invoke(this);
                Clear();
            }
        }

        public void CancelWithError(string error)
        {
            if (IsExecuting)
            {
                Loop.StopCoroutine(Routine);
                status = RoutineStatus.Faulted;
                Error = error;
                EventFaulted?.Invoke(this, error);
                Clear();
            }
        }

        protected TaskRoutine(){}

        internal TaskRoutine(IEnumerator routine)
        {
            Routine = routine;
            EventStarting = null;
            EventCancelled = null;
            EventCompleted = null;
            EventDestroyed = null;
        }

        internal IEnumerator Run()
        {
            status = RoutineStatus.Executing;
            EventStarting?.Invoke(this);
            yield return Routine;
            Complete();
            Clear();
        }

        protected virtual void Complete()
        {
            status = RoutineStatus.CompletedSuccessfully;
            EventCompleted?.Invoke(this);
        }

        private void Clear()
        {
            EventDestroyed?.Invoke(this);
            EventCancelled = null;
            EventCompleted = null;
            EventStarting = null;
            EventDestroyed = null;
            Routine = null;
        }

        bool IEnumerator.MoveNext() => IsExecuting;
        void IEnumerator.Reset() { }
        object IEnumerator.Current => null;

        
        public static TaskRoutine FromCompleted()
        {
            return new TaskRoutine()
            {
                status = RoutineStatus.CompletedSuccessfully
            };
        }

        public static TaskRoutine FromCanceled()
        {
            return new TaskRoutine
            {
                status = RoutineStatus.Canceled,
                Error = CANCELED,
            };
        }

        public static TaskRoutine FromError(string error)
        {
            return new TaskRoutine
            {
                status = RoutineStatus.Faulted,
                Error = error
            };
        }

        public static TaskRoutine<TResult> FromResult<TResult>(TResult result)
        {
            return new TaskRoutine<TResult>()
            {
                status = RoutineStatus.CompletedSuccessfully,
                Result = result
            };
        }

        public static TaskRoutine Run(IEnumerator routine)
        {
            var h = new TaskRoutine(routine);
            Loop.StartCoroutine(h.Run());
            return h;
        }

        public static TaskRoutine<T> Run<T>(IEnumerator routine)
        {
            var h = new TaskRoutine<T>(routine);
            Loop.StartCoroutine(h.Run());
            return h;
        }
    }
}