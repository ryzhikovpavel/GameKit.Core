using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameKit
{
    public class MockAnalytics: IAnalytic
    {
        private StringBuilder _builder = new StringBuilder();
        
        public string Name { get; } = nameof(MockAnalytics);
        
        public void LogEvent(AnalyticsEvent e)
        {
            foreach (var pair in e.EventData)
            {
                _builder.Append($"{pair.Key}={pair.Value};");
            }
            
            Logger<Analytics>.Info($"Event: {e.GetFullEventName()}; Values: {_builder}");
            _builder.Clear();
        }
        
        #if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            Service<Analytics>.Instance.Add(new MockAnalytics());
        }
        #endif
    }
}