#pragma warning disable 649
using System;

namespace GameKit
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static double ToUnixTimestamp(this DateTime dateTime)
        {
            return (dateTime.Subtract(UnixEpoch)).TotalSeconds;
        }
        
        public static DateTime FromUnixTimestamp(this double timeStamp)
        {
            return UnixEpoch.AddSeconds(timeStamp);
        }
    }
}