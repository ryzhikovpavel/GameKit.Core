using System;
using UnityEngine;

namespace GameKit
{
    public class EventDestroy : MonoBehaviour
    {
        private Action _eventDestroy = delegate {  };

        public static void Subscribe(GameObject gameObject, Action onDestroy)
        {
            var e = gameObject.GetComponent<EventDestroy>() ?? gameObject.AddComponent<EventDestroy>();
            e._eventDestroy += onDestroy;
        }

        public static void Unsubscribe(GameObject gameObject, Action onDestroy)
        {
            var e = gameObject.GetComponent<EventDestroy>() ?? gameObject.AddComponent<EventDestroy>();
            e._eventDestroy -= onDestroy;
        }

        private void OnDestroy()
        {
            _eventDestroy();
        }
    }
}