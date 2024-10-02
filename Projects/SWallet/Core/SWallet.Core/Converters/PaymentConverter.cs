using SWallet.Core.Models.Payment;
using SWallet.Data.Core.Entities;

namespace SWallet.Core.Converters
{
    public static class PaymentConverter
    {
        public static PaymentMethodModel ToPaymentMethodModel(this PaymentMethod method)
        {
            return new PaymentMethodModel
            {
                Id = method.Id,
                Code = method.Code,
                Enabled = method.Enabled,
                Fee = method.Fee,
                Max = method.Max,
                Min = method.Min,
                Name = method.Name,
                Icon = method.Icon
            };
        }
    }
}