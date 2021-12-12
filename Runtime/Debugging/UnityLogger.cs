using System;
using System.Runtime.CompilerServices;

namespace GameKit
{
    public class UnityLogger : ILogger
    {
        private readonly string _environment;
        
        public bool IsDebugAllowed { get; private set; }
        public bool IsInfoAllowed { get; private set; }
        public bool IsWarningAllowed { get; private set; }
        public bool IsErrorAllowed { get; private set; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Debug(string message)
        {
            UnityEngine.Debug.Log($"{_environment}|{message}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Info(string message)
        {
            UnityEngine.Debug.Log($"{_environment}|{message}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Warning(string message)
        {
            UnityEngine.Debug.LogWarning($"{_environment}|{message}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Error(string message)
        {
            UnityEngine.Debug.LogError($"{_environment}|{message}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Error(Exception exception)
        {
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