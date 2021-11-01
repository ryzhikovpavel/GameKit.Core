using System;
using GameKit.Pooling.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.Pooling
{
    internal class PoolItemHandler<T> : IPoolItemHandler<T>
    {
        public T CreateInstance() => Activator.CreateInstance<T>();
        public void Reset(T item) { }
    }

    internal class FuncPoolItemHandler<T> : IPoolItemHandler<T>
    {
        private readonly Func<T> _create;
        private readonly Action<T> _reset;

        public FuncPoolItemHandler(Func<T> createInstance, Action<T> reset)
        {
            _create = createInstance;
            _reset = reset;
        }
        
        public T CreateInstance() => _create();
        public void Reset(T item) { }
    }
}