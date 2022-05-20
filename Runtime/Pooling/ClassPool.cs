using System;
using UnityEngine;

namespace GameKit
{
    public class ClassPool<T> where T : class, new()
    {
        private static T[] _instances;
        private static int _count;

        public static void Initialize(int capacity)
        {
            _instances = new T[capacity];
            for (int i = 0; i < capacity; i++)
            {
                _instances[i] = new T();
            }
            _count = capacity;
        }

        public static T Pull()
        {
            if (_count == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning("ClassPool is empty, new instance created");
#endif
                return new T();
            }
            return _instances[--_count];
        }

        public static void Push(T instance)
        {
            if (_count == _instances.Length)
            {
#if UNITY_EDITOR
                Debug.LogWarning("ClassPool is full, pool resized");
#endif                
                Array.Resize(ref _instances, _count + 1);
            }
            _instances[_count++] = instance;
        }
    }
}