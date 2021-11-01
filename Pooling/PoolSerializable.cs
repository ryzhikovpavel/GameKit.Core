using System;
using System.Collections;
using System.Collections.Generic;
using GameKit.Pooling.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit
{
    [Serializable]
    public class PoolSerializable<T>: IPool<T>, IPoolItemHandler<T> where T: Component
    {
        private enum PrefabOption
        {
            Ignore,
            Reset,
            Preset
        }
        
        [Header("Links")]
        [SerializeField] private Transform root;
        [SerializeField] private T prefab;

        [Header("Options")]
        [SerializeField] private PrefabOption entityScale = PrefabOption.Reset;
        [SerializeField] private PrefabOption entityRotate = PrefabOption.Reset;
        [SerializeField] private bool activateOnPull = true;
        private Pool<T> _pool;
        
        
        public event Action<T> EventReset = delegate {};
        public Transform Root => root;
        public T Prefab => prefab;

        public void Push(IPoolEntity entity)
        {
            GetPool().Push(entity);
        }

        public void ReleaseAll()
        {
            GetPool().ReleaseAll();
        }

        public T Pull()
        {
            var e = GetPool().Pull();
            e.gameObject.SetActive(activateOnPull);
            return e;
        }

        public void PushInstance(T instance)
        {
            GetPool().PushInstance(instance);
        }

        public void Initialize(int capacity)
        {
            GetPool().Initialize(capacity);
        }

        private Pool<T> GetPool()
        {
            if (_pool == null) _pool = new Pool<T>(this);
            return _pool;
        }
        
        T IPoolItemHandler<T>.CreateInstance()
        {
            GameObject go = Object.Instantiate(prefab.gameObject) as GameObject;
            return go.GetComponent<T>();
        }

        void IPoolItemHandler<T>.Reset(T item)
        {
            var t = item.transform;
            
            if (root != null) t.SetParent(root);
            t.localPosition = Vector3.zero;
            if (entityScale != PrefabOption.Ignore)
                t.localScale = entityScale == PrefabOption.Reset ? Vector3.one : prefab.transform.localScale;
            if (entityRotate != PrefabOption.Ignore)
                t.localRotation = entityRotate == PrefabOption.Reset ? Quaternion.identity : prefab.transform.localRotation;
            
            EventReset(item);
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return _pool.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}