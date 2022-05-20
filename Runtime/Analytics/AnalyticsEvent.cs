using System;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using GameKit.Implementation;
using JetBrains.Annotations;

namespace GameKit
{
    [PublicAPI]
    public class AnalyticsEvent: IPoolEntity
    {
        private const string EVENT_NAME_KEY = "name";
        private const string ST1_NAME_KEY = "ST1";
        private const string ST2_NAME_KEY = "ST2";
        private const string ST3_NAME_KEY = "ST3";
        
        public Dictionary<string, object> EventData { get; private set; } = new Dictionary<string, object>(20);

        internal string[] Services;
        internal bool IsExclusionList;
        private int _id;
        private IPoolContainer _owner;

        public string GetFullEventName()
        {
            var s = new StringBuilder();
            if (EventData.TryGetValue(ST1_NAME_KEY, out var st1))
            {
                s.Append(st1);
                s.Append("_");
            }
            
            if (EventData.TryGetValue(ST2_NAME_KEY, out var st2))
            {
                s.Append(st2);
                s.Append("_");
            }
            
            if (EventData.TryGetValue(ST3_NAME_KEY, out var st3))
            {
                s.Append(st3);
                s.Append("_");
            }

            s.Append(EventName);
            return s.ToString();
        }
        
        public string EventName => GetStringValue(EVENT_NAME_KEY);

        public string ST1
        {
            get => GetStringValue(ST1_NAME_KEY);
            set => EventData[ST1_NAME_KEY] = value;
        }
        
        public string ST2
        {
            get => GetStringValue(ST1_NAME_KEY);
            set => EventData[ST1_NAME_KEY] = value;
        }
        
        public string ST3
        {
            get => GetStringValue(ST1_NAME_KEY);
            set => EventData[ST1_NAME_KEY] = value;
        }
        
        public static AnalyticsEvent Create( string eventName )
        {
            AssertNameValid(eventName);
            return Service<Analytics>.Instance.Pull().AddEventData(EVENT_NAME_KEY, eventName);
        }
        
        public static AnalyticsEvent Create(string st1, string eventName)
        {
            var e = Create(eventName);
            e.ST1 = st1;
            return e;
        }
        
        public static AnalyticsEvent Create(string st1, string st2, string eventName)
        {
            var e = Create(eventName);
            e.ST1 = st1;
            e.ST2 = st2;
            return e;
        }
        
        public static AnalyticsEvent Create(string st1, string st2, string st3, string eventName)
        {
            var e = Create(eventName);
            e.ST1 = st1;
            e.ST2 = st2;
            e.ST3 = st3;
            return e;
        }

        /// <summary>
        /// Add parameter to analytics event.
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent Parameter( string name, bool value )
        {
            AddEventData( name, value );
            return this;
        }

        /// <summary>
        /// Add parameter to analytics event.
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent Parameter( string name, int value )
        {
            AddEventData( name, value );
            return this;
        }

        /// <summary>
        /// Add parameter to analytics event.
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent Parameter( string name, float value )
        {
            AddEventData( name, Convert.ToDouble( value ) );
            return this;
        }

        /// <summary>
        /// Add parameter to analytics event.
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent Parameter( string name, long value )
        {
            AddEventData( name, Convert.ToInt64( value ) );
            return this;
        }

        /// <summary>
        /// Add parameter to analytics event.
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent Parameter( string name, double value )
        {
            AddEventData( name, value );
            return this;
        }

        /// <summary>
        /// Add parameter to analytics event.
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent Parameter( string name, string value )
        {
            AddEventData( name, value );
            return this;
        }

        public void Send()
        {
            Service<Analytics>.Instance.Send(this);
        }

        public void SendOnly(params string[] analytics)
        {
            Services = analytics;
            IsExclusionList = false;
            Send();
        }

        public void SendWithout(params string[] analytics)
        {
            Services = analytics;
            IsExclusionList = true;
            Send();
        }

        internal void Clear()
        {
            Services = null;
            EventData.Clear();
        }

        private string GetStringValue(string key)
        {
            if (EventData is null) return string.Empty;
            if (EventData.TryGetValue(key, out var value)) return (string)value;
            return string.Empty;
        }

        private static void AssertNameValid( string eventName )
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Logger<Analytics>.Error( "[AnalyticsEvent] Event name can't be null or empty value" );
                throw new Exception("[AnalyticsEvent] Event name can't be null or empty value");
            }
        }

        private AnalyticsEvent AddEventData( string name, object value )
        {
            if ( value is null )
            {
                Logger<Analytics>.Error( $"[AnalyticsEvent] Parameter can't be null. Parameter {name} won't be added." );
                return this;
            }

            if ( value is string str && string.IsNullOrEmpty(str))
            {
                Logger<Analytics>.Error( $"[AnalyticsEvent] String parameter can't be empty. Parameter {name} won't be added." );
                return this;
            }
            
            EventData[name] = value;
            return this;
        }

        int IPoolEntity.Id
        {
            get => _id;
            set => _id = value;
        }

        IPoolContainer IPoolEntity.Owner
        {
            get => _owner;
            set => _owner = value;
        }

        void IPoolEntity.Reset()
        {
            EventData?.Clear();
        }
    }
}