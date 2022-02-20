using System.Collections.Generic;

namespace GameKit
{
    public interface IAnalytic
    {
        string Name { get; }
        void LogEvent(AnalyticsEvent e);
    }
}