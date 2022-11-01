using System;

namespace GameKit
{
    [Flags]
    public enum LogType
    {
        Disable = 0,
        
        All = Debug | Info | Warning | Error,
        Normal = Info | Warning | Error,
        Important = Warning | Error,
        
        Debug = 1,
        Info = 2,
        Warning = 4,
        Error = 8
    }
}