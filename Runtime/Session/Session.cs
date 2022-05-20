using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace GameKit
{
    internal class Session<T>: ISession<T> where T: struct
    {
        private static readonly Dictionary<string, Session<T>> Instances = new Dictionary<string, Session<T>>();
        internal static ISession<T> Resolve(string name, ISessionGroup group)
        {
            if (Instances.TryGetValue(name, out var session)) return session;
            session = new Session<T>(name, group);
            Instances.Add(name, session);
            return session;
        }

        internal static string DefaultName { get; } = typeof(T).Name;
        
        private readonly string _name;
        private readonly ISessionGroup _group;
        private T _value;
        private bool _loaded;

        public event SessionHandler<T> EventChanged;

        private Session(string name, ISessionGroup group)
        {
            _group = group;
            _name = name;
#if UNITY_EDITOR
            if (typeof(T).IsSerializable == false)
                throw new Exception($"{typeof(T).Name} is not serializable");
#endif
        }

        public bool IsEmpty()
        {
            return _group.Contains(_name);
        }

        public T Get()
        {
            if (_loaded == false) return Load();
            return _value;
        }

        private ref T Load()
        {
            _loaded = true;
            _value = _group.Load<T>(_name);
            return ref _value;
        }
        
        public void Save(T value)
        {
            _loaded = true;
            _value = value;
            _group.Save(_name, ref _value);
            EventChanged?.Invoke(this);
        }

        public void Clear()
        {
            _group.Remove(_name);
        }
    }

    public delegate void SessionHandler<T>(ISession<T> session) where T : struct;

    public interface ISession<T> where T : struct
    {
        event SessionHandler<T> EventChanged;
        T Get();
        void Save(T value);
        void Clear();
        bool IsEmpty();
    }
    
    public static class Session
    {
        public static ISession<T> Resolve<T>(ISessionGroup group) where T : struct => Resolve<T>(Session<T>.DefaultName, group);
        public static ISession<T> Resolve<T>(string name, ISessionGroup group) where T: struct => Session<T>.Resolve(name, group);

        public static readonly SessionGroups Groups = new SessionGroups();
    }
}