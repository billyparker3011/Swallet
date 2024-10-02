using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Payments
{
    public interface IPaymentMethodRepository : IEntityFrameworkCoreRepository<int, PaymentMethod, SWalletContext>
    {
        Task<List<PaymentMethod>> FindByPaymentPartner(int paymentPartner);
        Task<PaymentMethod> FindByPaymentPartnerAndPaymentMethodCode(int paymentPartner, string paymentMethodCode);
        Task<List<PaymentMethod>> FindEnabledPaymentMethods(int paymentPartner);
    }
}
