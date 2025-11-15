namespace Allinone.Helper.Datetimes
{
    public static class DatetimeHelper
    {
        public static DateTime UTC8Now()
        {
            return DateTime.UtcNow.AddHours(8);
        }
    }
}
