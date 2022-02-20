using System;
using UnityEngine;

namespace GameKit.Implementation
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class SessionProviderAsPlayerPrefs : ISessionProvider
    {
        public event Action EventSave;

        public T Load<T>(string group, string name)
        {
            return JsonUtility.FromJson<T>(PlayerPrefs.GetString($"{group}_{name}", "{}"));
        }

        public void Save<T>(string group, string name, T data)
        {
            PlayerPrefs.SetString($"{group}_{name}", JsonUtility.ToJson(data));
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