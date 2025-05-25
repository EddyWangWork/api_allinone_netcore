namespace Allinone.Helper.Extension
{
    public static class ExtensionHelper
    {
        public static List<int> ToIntList(this string data) =>
            [.. data.Split(',').Select(int.Parse)];

        public static bool IsNullOrEmpty(this object data)
        {
            if (null == data) return true;
            if (data is List<object> list && !list.Any()) return true;
            if (data is string && string.IsNullOrEmpty(data.ToString())) return true;
            if (data is DateTime && Convert.ToDateTime(data) == DateTime.MinValue) return true;
            if (data is DBNull) return true;
            return false;
        }

        public static bool IsNotNullOrEmpty(this object data)
        {
            return !data.IsNullOrEmpty();
        }
    }
}
