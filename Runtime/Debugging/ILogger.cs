using System;

namespace GameKit
{
    public interface ILogger
    {
        bool IsDebugAllowed { get; }
        bool IsInfoAllowed { get; }
        bool IsWarningAllowed { get; }
        bool IsErrorAllowed { get; }

        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Error(Exception exception);

        void SetAllowed(LogType allowed);
    }
}