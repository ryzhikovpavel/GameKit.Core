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
    
    internal class PrefabPoolItemHandler<T> : IPoolItemHandler<T> where T: Component
    {
        private T _prefab;
        private Transform _root;

        public PrefabPoolItemHandler(T prefab, Transform root = null)
        {
            _prefab = prefab;
            _root = root;
        }
        
        public T CreateInstance()
        {
            GameObject go = Object.Instantiate(_prefab.gameObject, _root) as GameObject;
            return go.GetComponent<T>();
        }

        public void Reset(T item)
        {
            var t = item.transform;
            
            if (_root != null) t.SetParent(_root);
            t.localPosition = Vector3.zero;
            t.localScale = _prefab.transform.localScale;
            t.localRotation = _prefab.transform.localRotation;
        }
    }
}