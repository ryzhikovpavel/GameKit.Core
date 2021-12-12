using System;

namespace GameKit
{
    [Serializable]
    public struct DateTimeSerializable
    {
        public long value;
        public static implicit operator DateTime(DateTimeSerializable jdt)
        {
            return DateTime.FromFileTime(jdt.value);
        }
        public static implicit operator DateTimeSerializable(DateTime dt)
        {
            DateTimeSerializable jdt = new DateTimeSerializable();
            jdt.value = dt.ToFileTime();
            return jdt;
        }

        public override string ToString()
        {
            return DateTime.FromFileTime(value).ToString();
        }
    }
}