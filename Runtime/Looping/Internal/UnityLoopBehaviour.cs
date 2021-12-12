#pragma warning disable 649
using System;
using UnityEngine;

namespace GameKit.Internal
{
    internal class UnityLoopBehaviour : MonoBehaviour
    {
        public event Action EventStart = delegate { };
        public event Action EventQuit = delegate { };
        public event Action<bool> EventFocus = delegate { };
        public event Action<bool> EventPause = delegate { };
        public event Action EventUpdate = delegate { };
        public event Action EventLateUpdate = delegate { };
        public event Action EventFixedUpdate = delegate { };

        private void Start()
        {
            EventStart();
        }

        private void Update()
        {
            EventUpdate();
        }

        private void LateUpdate()
        {
            EventLateUpdate();
        }

        private void FixedUpdate()
        {
            EventFixedUpdate();
        }

        private void OnApplicationFocus(bool focus)
        {
            EventFocus(focus);
        }

        private void OnApplicationPause(bool pause)
        {
            EventPause(pause);
        }

        private void OnApplicationQuit()
        {
            EventQuit();
        }
    }
}