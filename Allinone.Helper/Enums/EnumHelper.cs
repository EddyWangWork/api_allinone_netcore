namespace Allinone.Helper.Enums
{
    public static class EnumHelper
    {
        public static TEnum GetEnumValue<TEnum>(int id) where TEnum : Enum
        {
            if (Enum.IsDefined(typeof(TEnum), id))
            {
                return (TEnum)Enum.ToObject(typeof(TEnum), id);
            }
            throw new ArgumentException($"Invalid ID: {id} for enum {typeof(TEnum).Name}");
        }

        public static string GetEnumStringValue<TEnum>(int id) where TEnum : Enum
        {
            if (Enum.IsDefined(typeof(TEnum), id))
            {
                return Enum.ToObject(typeof(TEnum), id).ToString();
            }
            throw new ArgumentException($"Invalid ID: {id} for enum {typeof(TEnum).Name}");
        }

        public static int GetEnumId<TEnum>(string enumName) where TEnum : Enum
        {
            var match = Enum.GetValues(typeof(TEnum))
                            .Cast<TEnum>()
                            .FirstOrDefault(e => e.ToString().Equals(enumName, StringComparison.OrdinalIgnoreCase));

            return match != null ? Convert.ToInt32(match) : -1; // Return -1 for invalid names
        }
    }
}
