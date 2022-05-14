using System;

namespace GameKit
{
    public interface ISessionGroup
    {
        T Load<T>(string name);
        void Save<T>(string name, ref T data);
        void Remove(string name);
    }
}