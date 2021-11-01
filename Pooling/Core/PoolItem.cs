using System;
using UnityEngine;

namespace GameKit.Pooling.Core
{
    internal enum PoolItemState
    {
        Pooled,
        Issued,
        Destroyed,
    }
    
    internal class PoolItem<T>
    {
        public PoolItemState State;
        public T Instance { get; private set; }
        public IPoolEntity Entity { get; private set; }
        
        public PoolItem(T instance, int id, IPoolContainer owner)
        {
            Instance = instance;
            State = PoolItemState.Pooled;

            if (instance is Component comp)
            {
                GameObject gameObject = comp.gameObject;
                Entity = gameObject.GetComponent<IPoolEntity>() ?? gameObject.AddComponent<PoolObjectEntity>();
                EventDestroy.Subscribe(gameObject, OnDestroy);
            }

            if (instance is GameObject go)
            {
                Entity = go.GetComponent<IPoolEntity>() ?? go.AddComponent<PoolObjectEntity>();
                EventDestroy.Subscribe(go, OnDestroy);
            }

            if (instance is IPoolEntity e) 
                Entity = e;
                
            if (Entity == null)
                throw new ArgumentException(
                    $"Instance type ({typeof(T).Name}) must be {nameof(GameObject)} or inherited from {nameof(IPoolEntity)} or {nameof(Component)}");
            
            Entity.Id = id;
            Entity.Owner = owner;
        }

        public void OnDestroy()
        {
            Instance = default;
            Entity = default;
            State = PoolItemState.Destroyed;
        }
    }
}