﻿using System;

namespace GameKit
{
    public static class Logger<T>
    {
        private static ILogger _logger;
        public static ILogger Instance => GetInstance();

        public static bool IsDebugAllowed => Instance.IsDebugAllowed;
        public static bool IsInfoAllowed => Instance.IsInfoAllowed;
        public static bool IsWarningAllowed => Instance.IsWarningAllowed;
        public static bool IsErrorAllowed => Instance.IsErrorAllowed;
        public static void Debug(string message) => Instance.Debug(message);
        public static void Info(string message) => Instance.Info(message);
        public static void Warning(string message) => Instance.Warning(message);
        public static void Error(string message) => Instance.Error(message);
        public static void Error(Exception exception) => Instance.Error(exception);
        public static void SetAllowed(LogType allowed) => Instance.SetAllowed(allowed);

        public static ILogger Bind(ILogger loggerProvider)
        {
            _logger = loggerProvider;
            return _logger;
        }
        
        private static ILogger GetInstance()
        {
            if (_logger is null) Bind(new UnityLogger(typeof(T).Name, LogType.Normal));
            return _logger;
        }
    }
}