#pragma warning disable 649
using System;
using GameKit.Core.Session;

namespace GameKit
{
    public static class Session
    {
        public static T Get<T>() where T : class => Session<T>.Get();
    }
}

namespace GameKit.Core.Session
{
    internal static class Session<T> where T: class
    {
        private static T _value;
        
        public static T Get()
        {
#if UNITY_EDITOR
            if (_value == null && typeof(T).IsSerializable == false)
                throw new Exception($"{typeof(T).Name} is not serializable");
#endif
            if (_value == null) Initialize();
            return _value;
        }
        
        private static void Initialize()
        {
            _value = Service<ISessionProvider>.Get().Load<T>(typeof(T).Name);
            Service<ISessionProvider>.Get().EventSave += Save;
        }

        private static void Save()
        {
            Service<ISessionProvider>.Get().Save(typeof(T).Name, _value);
        }
    }
}