using System;

namespace GameKit
{
    [Flags]
    public enum LogType
    {
        Disable = 0,
        Debug = 1,
        Info = 2,
        Warning = 4,
        Error = 8,
        
        Normal = Info | Warning | Error,
        Important = Warning | Error,
        All = Debug | Info | Warning | Error 
    }
}