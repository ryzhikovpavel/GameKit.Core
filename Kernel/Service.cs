#pragma warning disable 649
using System;
using System.Runtime.CompilerServices;
using GameKit.Core;
using UnityEngine;

namespace GameKit
{
    public static class Service
    {
        public enum State
        {
            Unknown,
            Registered,
            Instantiated
        }
        
        public static T Get<T>() => Service<T>.Get();
        public static void BindInstance<T>(T instance) => Service<T>.Bind(instance);
        public static void BindResolve<T>(Func<T> resolve) => Service<T>.Bind(resolve);
        public static void BindImplementation<TService, TImplementation>() where TImplementation : TService => Service<TService>.Bind<TImplementation>();
        public static void Instantiate<T>() => Service<T>.Instantiate();

        public static State GetState<T>()
        {
            if (Service<T>.IsInstantiated()) return State.Instantiated;
            if (Service<T>.IsRegistered()) return State.Registered;
            return State.Unknown;
        }
    }
}

namespace GameKit.Core
{
    internal static class Service<T>
    {
        private static Func<T> _resolve;
        private static T _instance;
        
        /// <summary>
        /// Gets global instance of T type.
        /// </summary>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get()
        {
            if (_instance == null) Instantiate();
            return _instance;
        }

        public static void Bind(T instance)
        {
            if (IsInstantiated())
                throw new Exception($"Binding not possible. The {typeof(T).Name} service is already initialized.");

            _instance = instance;
        }

        public static void Bind(Func<T> resolve)
        {
            if (IsInstantiated())
                throw new Exception($"Binding not possible. The {typeof(T).Name} service is already initialized.");

            _resolve = resolve;
        }

        public static void Bind<TDerived>() where TDerived: T
        {
            // ReSharper disable once ConvertClosureToMethodGroup
            Bind(()=>Service<TDerived>.Get());
        }

        public static bool IsRegistered() => _instance != null || _resolve != null;
        public static bool IsInstantiated() => _instance != null;

        public static void Instantiate()
        {
            if (_instance != null)
                throw new Exception($"The {typeof(T).Name} service is already initialized.");

            if (_resolve != null)
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