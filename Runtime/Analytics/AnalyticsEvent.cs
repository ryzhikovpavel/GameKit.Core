using System;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;

namespace GameKit
{
    [PublicAPI]
    public class AnalyticsEvent
    {
        public const string EVENT_NAME_KEY = "name";
        public Dictionary<string, object> EventData { get; private set; }

        internal AnalyticName[] Services;
        internal bool IsExclusionList;

        public string EventName
        {
            get
            {
                if ( EventData != null && EventData.ContainsKey( EVENT_NAME_KEY ) )
                {
                    return EventData[EVENT_NAME_KEY] as string;
                }

                return string.Empty;
            }
        }
        
        public static AnalyticsEvent Create( string eventName )
        {
            AssertNameValid(eventName);
            return Service<Analytics>.Instance.Pull().AddEventData(EVENT_NAME_KEY, eventName);
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

        public void SendOnly(params AnalyticName[] analytics)
        {
            Services = analytics;
            IsExclusionList = false;
            Send();
        }

        public void SendWithout(params AnalyticName[] analytics)
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
        
        private static void AssertNameValid( string eventName )
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Service<Analytics>.Logger.Error( "[AnalyticsEvent] Event name can't be null or empty value" );
                throw new Exception("[AnalyticsEvent] Event name can't be null or empty value");
            }
        }
        
        private AnalyticsEvent AddEventData( string name, object value )
        {
            if ( value is null )
            {
                Service<Analytics>.Logger.Error( $"[AnalyticsEvent] Parameter can't be null. Parameter {name} won't be added." );
                return this;
            }

            if ( value is string str && string.IsNullOrEmpty(str))
            {
                Service<Analytics>.Logger.Error( $"[AnalyticsEvent] String parameter can't be empty. Parameter {name} won't be added." );
                return this;
            }

            if ( EventData == null ) EventData = new Dictionary<string, object>(20);
            EventData[name] = value;
            return this;
        }
    }
}