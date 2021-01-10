using System;

namespace AAS.Architecture.Extensions
{
    public static class DateTimeExtensions
    {
        public static int AsDaysSinceEpoch(this DateTime dateTime)
            => (dateTime - new DateTime()).Days;
        
        public static DateTime AsDateTime(this decimal unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds((long)unixTime);
        }
    }
}