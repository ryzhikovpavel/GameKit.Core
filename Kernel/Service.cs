#pragma warning disable 649
using System;
using System.Runtime.CompilerServices;
using GameKit.Core;
using UnityEngine;

namespace GameKit
{
    public static class Service<T>
    {
        private static Func<T> _resolve;
        private static T _instance;
        
        public static T Instance
        {
            get
            {
                if (_instance is null) Instantiate();
                return _instance;
            }
        }

        public static void Bind(T instance)
        {
            if (IsInstantiated)
                throw new Exception($"Binding not possible. The {typeof(T).Name} service is already initialized.");

            _instance = instance;
        }

        public static void Bind(Func<T> resolve)
        {
            if (IsInstantiated)
                throw new Exception($"Binding not possible. The {typeof(T).Name} service is already initialized.");

            _resolve = resolve;
        }

        public static void Bind<TDerived>() where TDerived: T
        {
            // ReSharper disable once ConvertClosureToMethodGroup
            Bind(()=>Service<TDerived>.Instance);
        }

        public static bool IsRegistered => !(_instance is null && _resolve is null);
        public static bool IsInstantiated => !(_instance is null);

        public static void Instantiate()
        {
            if (_instance is null == false)
                throw new Exception($"The {typeof(T).Name} service is already initialized.");

            if (_resolve is null == false)
            {
                _instance = _resolve();
                return;
            }

            var type = typeof(T);
            if (type.IsClass && type.IsAbstract == false)
            {
                _instance = (T)Activator.CreateInstance(type);
                return;
            }

            throw new NotImplementedException($"Service {typeof(T).Name} not implemented");
        }
    }
}