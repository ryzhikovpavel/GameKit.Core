using System;
using System.Collections;
using System.Collections.Generic;
using GameKit.Implementation;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit
{
    public static class Pool
    {
        public static Pool<T> Build<T>(Func<T> instantiate, Action<T> reset = null)  where T: class =>
            new Pool<T>(new FuncPoolItemHandler<T>(instantiate, reset));
        public static Pool<T> Build<T>(T prefab, Transform root = null) where T : Component =>
            new Pool<T>(new PrefabPoolItemHandler<T>(prefab, root));
        public static Pool<T> Build<T>() where T: class => new Pool<T>(new PoolItemHandler<T>());
        public static Pool<T> Build<T>(IPoolItemHandler<T> poolItemHandler) where T: class => new Pool<T>(poolItemHandler);
    }
    
    public class Pool<T> : IPool<T>, IDisposable where T: class
    {
        private readonly List<PoolItem<T>> _items = new List<PoolItem<T>>();
        private readonly IPoolItemHandler<T> _poolItemHandler;

        public event Action<T> EventReset =delegate { };
        public int IssuedCount { get; private set; }
        public int PooledCount => _items.Count - IssuedCount;
        
        internal Pool(IPoolItemHandler<T> poolItemHandler)
        {
            this._poolItemHandler = poolItemHandler;
        }

        public void Initialize(int capacity)
        {
            _items.Capacity = capacity;
            for (int i = 0; i < capacity; i++)
            {
                Create();
            }
        }

        public T Pull()
        {
            T item = PullInternal();
            return item;
        }
        private T PullInternal()
        {
            foreach (var item in _items)
            {
                if (item.State == PoolItemState.Pooled)
                {
                    item.State = PoolItemState.Issued;
                    IssuedCount++;
                    return item.Instance;
                }
            }
            
            var newItem = Create();
            newItem.State = PoolItemState.Issued;
            IssuedCount++;
            return newItem.Instance;
        }

        public void PushInstance(T instance)
        {
            foreach (var item in _items)
            {
                if (item.Instance == instance)
                {
                    Push(item.Entity);
                    return;
                }
            }
            
            var newItem = new PoolItem<T>(instance, _items.Count, this);
            Reset(newItem);
            _items.Add(newItem);
        }

        public void Push(IPoolEntity entity)
        {
            var item = _items[entity.Id];
            item.State = PoolItemState.Pooled;
            IssuedCount--;
            
            GameObject gameObject;
            if (item.Instance is Component comp)
                gameObject = comp.gameObject;
            else
                gameObject = item.Instance as GameObject;
            
            if (gameObject != null)
                gameObject.SetActive(false);
            
            Reset(item);
        }

        public void ReleaseAll()
        {
            foreach (var item in _items)
            {
                if (item.State == PoolItemState.Issued)
                {
                    Push(item.Entity);
                }
            }
        }

        private PoolItem<T> Create()
        {
            T instance = _poolItemHandler.CreateInstance();

            PoolItem<T> item;
            for (var i = 0; i < _items.Count; i++)
            {
                var poolItem = _items[i];
                if (poolItem.State == PoolItemState.Destroyed)
                {
                    item = new PoolItem<T>(instance, i, this);
                    _items[i] = item;
                    Reset(item);
                    return item;
                }
            }

            item = new PoolItem<T>(instance, _items.Count, this);
            _items.Add(item);
            Reset(item);
            return item;
        }

        private void Reset(PoolItem<T> item)
        {
            item.Entity.Reset();
            _poolItemHandler.Reset(item.Instance);
            EventReset(item.Instance);
        }

        public void Dispose()
        {
            foreach (var item in _items)
            {
                if (item.Instance is IDisposable dis)
                    dis.Dispose();
                
                if (item.Instance is Component comp)
                    Object.Destroy(comp.gameObject);
                
                if (item.Instance is GameObject go)
                    Object.Destroy(go);
                
                item.OnDestroy();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new IssuedEnumerator<T>(_items.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}