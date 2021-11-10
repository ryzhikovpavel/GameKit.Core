using System;
using UnityEngine;

namespace GameKit.Core.Session
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class SessionProviderAsPlayerPrefs : ISessionProvider
    {
        public event Action EventSave;

        public T Load<T>(string name)
        {
            return JsonUtility.FromJson<T>(PlayerPrefs.GetString(name, "{}"));
        }

        public void Save<T>(string name, T data)
        {
            PlayerPrefs.SetString(name, JsonUtility.ToJson(data));
        }
        

        public SessionProviderAsPlayerPrefs()
        {
            Loop.EventSuspend += LoopOnEventSuspend;
        }

        private void LoopOnEventSuspend()
        {
            EventSave?.Invoke();
            PlayerPrefs.Save();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Registration()
        {
            if (Service<ISessionProvider>.IsRegistered == false)
                Service<ISessionProvider>.Bind<SessionProviderAsPlayerPrefs>();
        }
    }
}