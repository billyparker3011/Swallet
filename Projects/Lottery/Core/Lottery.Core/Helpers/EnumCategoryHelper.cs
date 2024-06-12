using Lottery.Core.Attributes;
using Lottery.Core.Models.Enums;

namespace Lottery.Core.Helpers
{
    public static class EnumCategoryHelper
    {
        public static List<EnumRegionInformation<T>> GetListEnumRegionInformation<T>(this Type @enum)
            where T : Enum
        {
            var d = new List<EnumRegionInformation<T>>();

            var values = Enum.GetValues(@enum);
            foreach (T item in values)
            {
                var val = item.GetEnumRegionInformation();
                if (val == null)
                {
                    continue;
                }

                d.Add(val);
            }

            return d;
        }

        public static EnumRegionInformation<T> GetEnumRegionInformation<T>(this T value)
            where T : Enum
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes = (EnumRegionDescriptionAttribute[])fi.GetCustomAttributes(typeof(EnumRegionDescriptionAttribute), false);
            var firstDescription = attributes.FirstOrDefault();
            if (firstDescription == null)
            {
                return null;
            }

            return new EnumRegionInformation<T>
            {
                Value = value,
                Code = firstDescription.Code,
                Name = firstDescription.Name,
                NoOfPrize = firstDescription.NoOfPrize
            };
        }

        public static List<EnumCategoryInformation<T>> GetListEnumCategoryInformation<T>(this Type @enum)
            where T : Enum
        {
            var d = new List<EnumCategoryInformation<T>>();

            var values = Enum.GetValues(@enum);
            foreach (T item in values)
            {
                var val = item.GetEnumCategoryInformation();
                if (val == null)
                {
                    continue;
                }

                d.Add(val);
            }

            return d;
        }

        public static EnumCategoryInformation<T> GetEnumCategoryInformation<T>(this T value)
            where T : Enum
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes = (EnumCategoryDescriptionAttribute[])fi.GetCustomAttributes(typeof(EnumCategoryDescriptionAttribute), false);
            var firstDescription = attributes.FirstOrDefault();
            if (firstDescription == null)
            {
                return null;
            }

            return new EnumCategoryInformation<T>
            {
                Code = firstDescription.Code,
                Name = firstDescription.Name,
                OrderBy = firstDescription.OrderBy,
                Region = firstDescription.Region,
                Value = value
            };
        }

        public static List<EnumSubCategoryInformation<T>> GetListEnumSubCategoryInformation<T>(this Type @enum)
            where T : Enum
        {
            var d = new List<EnumSubCategoryInformation<T>>();

            var values = Enum.GetValues(@enum);
            foreach (T item in values)
            {
                var val = item.GetEnumSubCategoryInformation();
                if (val == null)
                {
                    continue;
                }

                d.Add(val);
            }

            return d;
        }

        public static EnumSubCategoryInformation<T> GetEnumSubCategoryInformation<T>(this T value)
            where T : Enum
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes = (EnumSubCategoryDescriptionAttribute[])fi.GetCustomAttributes(typeof(EnumSubCategoryDescriptionAttribute), false);
            var firstDescription = attributes.FirstOrDefault();
            if (firstDescription == null)
            {
                return null;
            }

            return new EnumSubCategoryInformation<T>
            {
                Name = firstDescription.Name,
                OrderBy = firstDescription.OrderBy,
                Category = firstDescription.Category,
                BetKind = firstDescription.BetKind,
                Value = value,
                SubBetKinds = firstDescription.SubBetKind
            };
        }
    }
}
