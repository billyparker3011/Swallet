using HnMicro.Core.Attributes;
using HnMicro.Core.Models;

namespace HnMicro.Core.Helpers
{
    public static class EnumHelper
    {
        public static int ToInt<TEnum>(this TEnum data)
            where TEnum : Enum
        {
            return (int)(object)data;
        }

        public static TEnum ToEnum<TEnum>(this int data)
            where TEnum : Enum
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), data);
        }

        public static bool Is<TEnum>(this TEnum x, TEnum y)
            where TEnum : Enum
        {
            return x.ToInt() == y.ToInt();
        }

        public static List<EnumInformation<T>> GetListEnumInformation<T>(this Type @enum)
            where T : Enum
        {
            var d = new List<EnumInformation<T>>();

            var values = Enum.GetValues(@enum);
            foreach (T item in values)
            {
                var val = item.GetEnumInformation();
                if (val == null)
                {
                    continue;
                }

                d.Add(val);
            }

            return d;
        }

        public static EnumInformation<T> GetEnumInformation<T>(this T value)
            where T : Enum
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes = (EnumDescriptionAttribute[])fi.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
            var firstDescription = attributes.FirstOrDefault();
            if (firstDescription == null)
            {
                return null;
            }

            return new EnumInformation<T>
            {
                Code = firstDescription.Code,
                Value = value
            };
        }
    }
}
