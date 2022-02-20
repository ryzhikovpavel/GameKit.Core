using System;

namespace GameKit
{
    public interface ISessionProvider
    {
        event Action EventSave;
        T Load<T>(string group, string name);
        void Save<T>(string group, string name, T data);
    }
}