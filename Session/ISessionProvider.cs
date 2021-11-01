﻿using System;

namespace GameKit.Core.Session
{
    public interface ISessionProvider
    {
        event Action EventSave;
        T Load<T>(string name);
        void Save<T>(string name, T data);
    }
}