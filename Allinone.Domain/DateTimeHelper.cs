namespace Allinone.Domain
{
    public class DateTimeHelper
    {
        public static DateTime UnixToDateTimeSec(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return unixTimeStamp == 0 ? DateTime.UtcNow.AddHours(8) : dtDateTime;
        }

        public static DateTime UnixToDateTimeMSec(double javaTimeStamp)
        {
            // Java timestamp is milliseconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
            return javaTimeStamp == 0 ? DateTime.UtcNow.AddHours(8) : dtDateTime;
        }
    }
}
