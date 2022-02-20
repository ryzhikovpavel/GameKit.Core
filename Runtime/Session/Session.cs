using System;

namespace GameKit
{
    public static class Session<T> where T: class
    {
        private static string _group = "general";
        private static string _key;
        private static T _value;

        public static T Get()
        {
#if UNITY_EDITOR
            if (typeof(T).IsSerializable == false)
                throw new Exception($"{typeof(T).Name} is not serializable");
#endif
            if (_value is null) Initialize();
            return _value;
        }

        public static void ChangeGroup(string group)
        {
            if (group == _group) return;
            if (_value is null == false)
            {
                Save();
                _group = group;
                _value = Service<ISessionProvider>.Instance.Load<T>(_group, _key);
            } 
            else 
                _group = group;
        }
        
        private static void Initialize()
        {
            _key = typeof(T).Name;
            _value = Service<ISessionProvider>.Instance.Load<T>(_group, _key);
            Service<ISessionProvider>.Instance.EventSave += Save;
        }

        private static void Save()
        {
            Service<ISessionProvider>.Instance.Save(_group, _key, _value);
        }
    }
}