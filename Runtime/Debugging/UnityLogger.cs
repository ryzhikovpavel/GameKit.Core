using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameKit
{
    internal class UnityLogger : ILogger
    {
        private readonly string _environment;
        
        public bool IsDebugAllowed { get; private set; }
        public bool IsInfoAllowed { get; private set; }
        public bool IsWarningAllowed { get; private set; }
        public bool IsErrorAllowed { get; private set; }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public void Debug(string message)
        {
            if (IsDebugAllowed)
                UnityEngine.Debug.Log($"{_environment}|{message}");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public void Info(string message)
        {
            if (IsInfoAllowed)
                UnityEngine.Debug.Log($"{_environment}|{message}");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public void Warning(string message)
        {
            if (IsWarningAllowed)
                UnityEngine.Debug.LogWarning($"{_environment}|{message}");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public void Error(string message)
        {
            if (IsErrorAllowed)
                UnityEngine.Debug.LogError($"{_environment}|{message}");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public void Error(Exception exception)
        {
            if (IsErrorAllowed)
                UnityEngine.Debug.LogException(exception);
        }

        public void SetAllowed(LogType allowed)
        {
            IsDebugAllowed = allowed.HasFlag(LogType.Debug);
            IsInfoAllowed = allowed.HasFlag(LogType.Info);
            IsWarningAllowed = allowed.HasFlag(LogType.Warning);
            IsErrorAllowed = allowed.HasFlag(LogType.Error);
        }

        public UnityLogger(string environment, LogType allowed)
        {
            SetAllowed(allowed);
            _environment = environment;
        }
    }
}