using System;
using UnityEngine;

namespace GameKit
{
    internal sealed class UnityEventsBehaviour: MonoBehaviour
    {
        public readonly UnityEvents Listeners = new UnityEvents();

        private void OnEnable() => Listeners.FireEnabled(gameObject);
        private void OnDisable() => Listeners.FireDisabled(gameObject);
        private void OnDestroy()
        {
            if (Loop.IsQuitting == false) Listeners.FireDestroyed(gameObject);
        }
    }
    
    public class UnityEvents
    {
        public delegate void EventHandler(GameObject sender);
        
        public event EventHandler Enabled;
        public event EventHandler Disabled;
        public event EventHandler Destroyed;

        public void Clear()
        {
            Enabled = null;
            Disabled = null;
            Destroyed = null;
        }
        
        internal void FireEnabled(GameObject sender) => Enabled?.Invoke(sender);
        internal void FireDisabled(GameObject sender) => Disabled?.Invoke(sender);
        internal void FireDestroyed(GameObject sender) => Destroyed?.Invoke(sender);
    }

    public static class UnityEventsExtensions
    {
        public static UnityEvents Event(this GameObject obj)
        {
            return (obj.GetComponent<UnityEventsBehaviour>() ?? obj.AddComponent<UnityEventsBehaviour>()).Listeners;
        }

        public static UnityEvents Event(this Component comp) => comp.gameObject.Event();
    }
}