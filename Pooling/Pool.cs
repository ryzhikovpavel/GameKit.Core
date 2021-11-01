using System;
using System.Collections;
using System.Collections.Generic;
using GameKit.Pooling;
using GameKit.Pooling.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit
{
    public class Pool<T> : IPool<T>, IDisposable where T: class
    {
        private readonly List<PoolItem<T>> _items = new List<PoolItem<T>>();
        private readonly IPoolItemHandler<T> _poolItemHandler;

        public int IssuedCount { get; private set; }
        public int PooledCount => _items.Count - IssuedCount;

        
        public Pool(Func<T> instantiate, Action<T> reset = null): this(new FuncPoolItemHandler<T>(instantiate, reset)) { }
        public Pool(): this(new PoolItemHandler<T>()) { }
        public Pool(IPoolItemHandler<T> poolItemHandler)
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
            var item = new PoolItem<T>(instance, _items.Count, this);
            item.Entity.Reset();
            _poolItemHandler.Reset(item.Instance);
            _items.Add(item);
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
            
            item.Entity.Reset();
            _poolItemHandler.Reset(item.Instance);
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
                    item.Entity.Reset();
                    _poolItemHandler.Reset(item.Instance);
                    return item;
                }
            }

            item = new PoolItem<T>(instance, _items.Count, this);
            _items.Add(item);
            item.Entity.Reset();
            _poolItemHandler.Reset(item.Instance);
            return item;
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