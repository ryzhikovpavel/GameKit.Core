using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit
{
    public class DispatchedSignal<T> where T: Delegate
    {
        private readonly List<T> _callbacks = new List<T>(10);
        
        public void AddListener(T handler)
        {
            if (_callbacks.IndexOf(handler) >= 0)
                Debug.LogWarning($"Handler {handler.Method.Name} already exist");
            _callbacks.Add(handler);
        }
        
        public void RemoveListener(T handler)
        {
            int index = _callbacks.IndexOf(handler);
            if (index >= 0) _callbacks.RemoveAt(index);
        }

        public DispatchedSignal()
        {
            ClassPool<List<T>>.Initialize(3);
        }
         
        protected virtual void OnDispatched()
        {
            if (Logger.IsDebugAllowed) Logger.Debug($"Signal {GetType().Name} is dispatched");
        }
        
        protected List<T> BeginInvocation()
        {
            var list = ClassPool<List<T>>.Pull();
            list.AddRange(_callbacks);
            return list;
        }

        protected void EndInvocation(List<T> list)
        {
            list.Clear();
            ClassPool<List<T>>.Push(list);
        }

        protected virtual ILogger Logger => Logger<Signal>.Instance;
    }
}