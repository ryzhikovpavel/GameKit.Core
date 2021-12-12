using System.Collections.Generic;
using System.Linq;

namespace GameKit
{
    public class Analytics
    {
        private Pool<AnalyticsEvent> _eventsPool;
        private List<IAnalytic> _analytics;
        
        public Analytics()
        {
            _eventsPool = Pool.Build<AnalyticsEvent>();
            _analytics = new List<IAnalytic>();
            Loop.EventEndFrame += OnEventEndFrame;
        }

        public void Add(IAnalytic analytic)
        {
            _analytics.Add(analytic);
        }
        
        internal AnalyticsEvent Pull() => _eventsPool.Pull();

        internal void Send(AnalyticsEvent @event)
        {
            foreach (var analytic in _analytics)
            {
                bool send = @event.Services is null;
                if (send == false)
                {
                    send = @event.Services.Contains(analytic.Name);
                    if (@event.IsExclusionList) send = !send;
                }
                if (send)
                    analytic.LogEvent(@event.EventData);
            }
            _eventsPool.PushInstance(@event);
        }

        private void OnEventEndFrame()
        {
            if (_eventsPool.IssuedCount > 0)
            {
                foreach (var @event in _eventsPool)
                {
                    Send(@event);
                }
            }
        }
    }

    public interface IAnalytic
    {
        AnalyticName Name { get; }
        void LogEvent(Dictionary<string, object> values);
    }
}