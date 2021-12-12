using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameKit
{
    public class MockAnalytics: IAnalytic
    {
        private StringBuilder _builder = new StringBuilder();
        
        public AnalyticName Name { get; } = new AnalyticName("Mock");
        
        public void LogEvent(Dictionary<string, object> values)
        {
            foreach (var pair in values)
            {
                _builder.Append($"{pair.Key}={pair.Value};");
            }
            
            Service<Analytics>.Logger.Info($"Event: {values[AnalyticsEvent.EVENT_NAME_KEY]}; Values: {_builder}");
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