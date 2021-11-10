#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    public class EditorCoroutine: IEnumerator
    {
        public delegate void EventErrorHandler(string error);

        public event EventErrorHandler EventError = delegate { };
        public event Action EventComplete = delegate { };

        private IEnumerator _routine;
        private bool _complete;
        
        private EditorCoroutine(IEnumerator routine)
        {
            _complete = false;
            _routine = routine;
            EditorApplication.update += Update;
        }
        
        public static EditorCoroutine Start(IEnumerator enumerator) => new EditorCoroutine(enumerator);

        public bool MoveNext() => _complete == false;

        public void Reset()
        {
            _complete = true;
            _routine = null;
            EditorApplication.update -= Update;
        }
        
        public object Current => null;
        
        private void Update()
        {
            try
            {
                if (Continue(_routine)) return;
                EventComplete();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EventError(e.Message);
            }
            Reset();
        }

        private static bool Continue(IEnumerator routine)
        {
            if (routine.Current is AsyncOperation operation && operation.isDone == false) return true;
            if (routine.Current is IEnumerator current && Continue(current)) return true;
            return routine.MoveNext();
        }
    }
}
#endif