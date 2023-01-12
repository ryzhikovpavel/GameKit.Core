using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace GameKit
{
    [PublicAPI]
    public abstract class State: IDisposable
    {
        private readonly List<IEnumerator> _routines = new List<IEnumerator>();
        private CancellationTokenSource _cancellation;
        protected CancellationToken Token => _cancellation.Token;
        
        public StateContext Context { get; internal set; }
        public abstract void Enter();
        public abstract Task Exit();
        public abstract void Dispose();


        protected void StartCoroutine(IEnumerator routine)
        {
            _routines.Add(routine);
            Loop.StartCoroutine(routine);
        }

        internal void Initialize()
        {
            _cancellation = new CancellationTokenSource();
        }

        internal void Release()
        {
            foreach (IEnumerator routine in _routines)
            {
                Loop.StopCoroutine(routine);
            }
            
            if (_cancellation.IsCancellationRequested == false) _cancellation.Cancel();
            _cancellation.Dispose();
            _cancellation = null;
            
            Dispose();
        }
    }
}