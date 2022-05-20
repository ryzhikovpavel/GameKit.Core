using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameKit
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }

    internal class DefaultTimeProvider: ITimeProvider
    {
        public DateTime Now => DateTime.Now;

        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Register()
        {
            if (Service<ITimeProvider>.IsRegistered == false)
                Service<ITimeProvider>.Bind(Getter);
        }

        private static ITimeProvider Getter() => new DefaultTimeProvider();
    }
}