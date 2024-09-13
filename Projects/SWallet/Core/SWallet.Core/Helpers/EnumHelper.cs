using SWallet.Core.Attributes;
using SWallet.Core.Enums;
using SWallet.Core.Models.Enums;

namespace SWallet.Core.Helpers
{
    public static class EnumHelper
    {
        public static List<CustomerLevelInfoModel> GetListCustomerLevelInfo()
        {
            var d = new List<CustomerLevelInfoModel>();

            var values = Enum.GetValues(typeof(CustomerLevel));
            foreach (CustomerLevel item in values)
            {
                var val = GetCustomerLevelInfoModel(item);
                if (val == null)
                {
                    continue;
                }

                d.Add(val);
            }

            return d;
        }

        public static CustomerLevelInfoModel GetCustomerLevelInfoModel(CustomerLevel value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes = (EnumCustomerLevelAttribute[])fi.GetCustomAttributes(typeof(EnumCustomerLevelAttribute), false);
            var firstDescription = attributes.FirstOrDefault();
            if (firstDescription == null)
            {
                return null;
            }

            return new CustomerLevelInfoModel
            {
                Value = value,
                Code = firstDescription.LevelCode
            };
        }
    }
}
