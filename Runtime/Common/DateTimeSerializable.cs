using System;
using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;

namespace GameKit
{
    [PublicAPI]
    [Serializable]
    public struct DateTimeSerializable
    {
        [SerializeField]
        private double unixTime;
        private DateTime? _time;

        private DateTime GetTime()
        {
            if (_time.HasValue) return _time.Value;
            _time = unixTime.FromUnixTimestamp();
            return _time.Value;
        }

        public static implicit operator DateTime(DateTimeSerializable other)
        {
            return other.GetTime();
        }
        public static implicit operator DateTimeSerializable(DateTime dt)
        {
            return new DateTimeSerializable
            {
                unixTime = dt.ToUnixTimestamp()
            };
        }

        public override string ToString()
        {
            return GetTime().ToString(CultureInfo.InvariantCulture);
        }

        public System.DateTime Date => GetTime().Date;
        public int Day => GetTime().Day;
        public System.DayOfWeek DayOfWeek => GetTime().DayOfWeek;
        public int DayOfYear => GetTime().DayOfYear;
        public int Hour => GetTime().Hour;
        public System.DateTimeKind Kind => GetTime().Kind;
        public int Millisecond => GetTime().Millisecond;
        public int Minute => GetTime().Minute;
        public int Month => GetTime().Month;
        public int Second => GetTime().Second;
        public long Ticks => GetTime().Ticks;
        public System.TimeSpan TimeOfDay => GetTime().TimeOfDay;
        public int Year => GetTime().Year;
        public System.DateTime Add(System.TimeSpan value) => GetTime().Add(value);
        public System.DateTime AddDays(double value) => GetTime().AddDays(value);
        public System.DateTime AddHours(double value) => GetTime().AddHours(value);
        public System.DateTime AddMilliseconds(double value) => GetTime().AddMilliseconds(value);
        public System.DateTime AddMinutes(double value) => GetTime().AddMinutes(value);
        public System.DateTime AddMonths(int months) => GetTime().AddMonths(months);
        public System.DateTime AddSeconds(double value) => GetTime().AddSeconds(value);
        public System.DateTime AddTicks(long value) => GetTime().AddTicks(value);
        public System.DateTime AddYears(int value) => GetTime().AddYears(value);
    }
}