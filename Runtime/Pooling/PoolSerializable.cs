using System;
using System.Collections;
using System.Collections.Generic;
using GameKit.Implementation;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit
{
    [Serializable]
    public class PoolSerializable<T>: IPool<T> where T: Component
    {
        private class PoolItemHandler: IPoolItemHandler<T> 
        {
            T IPoolItemHandler<T>.CreateInstance()
            {
                return _owner.CreateInstance();
            }

            void IPoolItemHandler<T>.Reset(T item)
            {
                _owner.ResetInstance(item);
            }

            private PoolSerializable<T> _owner;
            
            public PoolItemHandler(PoolSerializable<T> owner)
            {
                _owner = owner;
            }
        }
        
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
        private Transform _parent;
        
        public event Action<T> EventReset = delegate {};
        public Transform Root => _parent ? _parent : root;
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
        
        // ReSharper disable once ParameterHidesMember
        public void Initialize(Transform root)
        {
            this._parent = root;
        }
        
        // ReSharper disable once ParameterHidesMember
        public void Initialize(int capacity, Transform root)
        {
            this._parent = root;
            GetPool().Initialize(capacity);
        }

        private Pool<T> GetPool()
        {
            if (_pool == null)
            {
                _parent = root;
                _pool = Pool.Build(new PoolItemHandler(this));
                if (_parent == null)
                {
                    var go = new GameObject($"pool_root_for_{typeof(T).Name}");
                    _parent = go.transform;
                }
            }
            return _pool;
        }

        private void OnDestroy()
        {
            Object.Destroy(_parent);
        }

        private T CreateInstance()
        {
            GameObject go = Object.Instantiate(prefab.gameObject, _parent) as GameObject;
            return go.GetComponent<T>();
        }

        private void ResetInstance(T item)
        {
            var t = item.transform;
            
            if (_parent is null == false) t.SetParent(_parent);
            t.localPosition = Vector3.zero;
            if (entityScale != PrefabOption.Ignore)
                t.localScale = entityScale == PrefabOption.Reset ? Vector3.one : prefab.transform.localScale;
            if (entityRotate != PrefabOption.Ignore)
                t.localRotation = entityRotate == PrefabOption.Reset ? Quaternion.identity : prefab.transform.localRotation;
            item.Event().Clear();
            EventReset(item);
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return GetPool().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}