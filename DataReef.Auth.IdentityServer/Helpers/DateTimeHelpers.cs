using System;

namespace DataReef.Auth.IdentityServer.Helpers
{
    public static class DateTimeHelpers
    {
        internal static long ToEpochTime(this DateTime dateTime)
        {
            TimeSpan elapsedTime = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            long seconds = Convert.ToInt64(elapsedTime.TotalSeconds);

            return seconds;
        }
    }
}