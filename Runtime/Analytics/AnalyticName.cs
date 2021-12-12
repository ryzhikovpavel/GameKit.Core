using System;
using System.Collections.Generic;

namespace GameKit
{
    public readonly struct AnalyticName: IComparable<AnalyticName>, IEqualityComparer<AnalyticName>
    {
        private readonly string _value;

        public AnalyticName(string name)
        {
            _value = name;
        }

        public int CompareTo(AnalyticName other)
        {
            return String.Compare(_value, other._value, StringComparison.Ordinal);
        }

        public bool Equals(AnalyticName x, AnalyticName y)
        {
            return x._value.Equals(y._value);
        }
        
        public bool Equals(AnalyticName other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is AnalyticName other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (_value != null ? _value.GetHashCode() : 0);
        }

        public int GetHashCode(AnalyticName obj)
        {
            return obj.GetHashCode();
        }
        
        public static bool operator ==(AnalyticName x, AnalyticName y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(AnalyticName x, AnalyticName y)
        {
            return !(x.Equals(y));
        }
    }
}