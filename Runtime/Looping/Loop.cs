#pragma warning disable 649
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameKit.Internal;

namespace GameKit
{
    public static class Loop
    {
        public delegate void ApplicationPauseEvent(bool pause);

        public delegate void ApplicationFocusEvent(bool focus);


        private static readonly CancellationTokenSource UnityRuntimeTokenSource = new CancellationTokenSource();
        private static List<IEnumerator> _coRoutines;
        private static UnityLoopBehaviour _unityLoop;

        public static event Action EventDispose = delegate { };
        public static event Action EventQuit = delegate { };
        public static event Action EventStart = delegate { };
        public static event ApplicationPauseEvent EventFocus = delegate { };
        public static event ApplicationFocusEvent EventPause = delegate { };
        public static event Action EventSuspend = delegate { };
        public static event Action EventResume = delegate { };
        public static event Action EventUpdate = delegate { };
        public static event Action EventLateUpdate = delegate { };
        public static event Action EventFixedUpdate = delegate { };
        public static event Action EventEndFrame = delegate { };

        public static bool IsStarted => _unityLoop != null;
        public static bool IsPaused { get; private set; }
        public static bool IsQuitting { get; private set; }

        public static CancellationToken Token => UnityRuntimeTokenSource.Token;


        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            var go = new GameObject("[Loop]");
            go.hideFlags = HideFlags.HideInHierarchy;
                
            _unityLoop = go.AddComponent<UnityLoopBehaviour>();
            _unityLoop.EventUpdate += () => EventUpdate();
            _unityLoop.EventStart += OnEventStart;
            _unityLoop.EventQuit += OnApplicationEventQuit;
            _unityLoop.EventFocus += (f) => EventFocus(f);
            _unityLoop.EventPause += OnApplicationEventPause;
            _unityLoop.EventFixedUpdate += () => EventFixedUpdate();
            _unityLoop.EventLateUpdate += () => EventLateUpdate();

            UnityEngine.Object.DontDestroyOnLoad(go);
            
            Application.focusChanged += OnApplicationFocus;
        }
        
        public static void StartCoroutine(IEnumerator enumerator)
        {
            if (IsStarted)
            {
                _unityLoop.StartCoroutine(enumerator);
                return;
            }

            if (_coRoutines == null) _coRoutines = new List<IEnumerator>();
            _coRoutines.Add(enumerator);
        }

        public static void StopCoroutine(IEnumerator enumerator)
        {
            if (IsStarted)
            {
                _unityLoop.StopCoroutine(enumerator);
                return;
            }
            if (_coRoutines == null) return;
            int idx = _coRoutines.IndexOf(enumerator);
            if (idx >= 0) _coRoutines.RemoveAt(idx);
        }

        public static void InvokeDelayed(float delaySeconds, Action load)
        {
            StartCoroutine(Delayd(delaySeconds, load));
        }

        private static IEnumerator Delayd(float delaySeconds, Action load)
        {
            yield return new WaitForSeconds(delaySeconds);
            load?.Invoke();
        }

        private static void OnApplicationEventPause(bool pause)
        {
            if (IsPaused && pause == false) EventResume();
            if (IsPaused == false && pause) EventSuspend();
            IsPaused = pause;
            EventPause(pause);
        }

        private static void OnApplicationFocus(bool focus)
        {
            if (IsPaused == true && focus == true) EventResume();
            if (IsPaused == false && focus == false) EventSuspend();
            IsPaused = !focus;
        }

        private static void OnApplicationEventQuit()
        {
            if (IsPaused == false) EventSuspend();
            IsPaused = true;
            IsQuitting = true;
            EventQuit();
            EventDispose();
            UnityRuntimeTokenSource.Cancel();
            Dispose();
        }

        private static void OnEventStart()
        {
            if (_coRoutines != null)
            {
                foreach (var routine in _coRoutines)
                {
                    _unityLoop.StartCoroutine(routine);
                }

                _coRoutines.Clear();
                _coRoutines = null;
            }

            EventStart();
			
			_unityLoop.StartCoroutine(FrameProcess());
        }

        private static IEnumerator FrameProcess()
        {
            var waitEndFrame = new WaitForEndOfFrame();

            while (IsQuitting == false)
            {
                yield return waitEndFrame;
                EventEndFrame();
            }
        }

        private static void Dispose()
        {
            if (_coRoutines != null) _coRoutines.Clear();
            _unityLoop = null;
            EventDispose = delegate { };
            EventQuit = delegate { };
            EventStart = delegate { };
            EventFocus = delegate { };
            EventPause = delegate { };
            EventSuspend = delegate { };
            EventResume = delegate { };
            EventUpdate = delegate { };
            EventLateUpdate = delegate { };
            EventFixedUpdate = delegate { };
            EventEndFrame = delegate { };

            IsPaused = false;
            IsQuitting = false;
        }
    }
}