using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Payments
{
    public class PaymentMethodRepository : EntityFrameworkCoreRepository<int, PaymentMethod, SWalletContext>, IPaymentMethodRepository
    {
        public PaymentMethodRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<List<PaymentMethod>> FindByPaymentPartner(int paymentPartner)
        {
            return await DbSet.Where(f => f.PaymentPartner == paymentPartner).ToListAsync();
        }

        public async Task<PaymentMethod> FindByPaymentPartnerAndPaymentMethodCode(int paymentPartner, string paymentMethodCode)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.PaymentPartner == paymentPartner && f.Code == paymentMethodCode);
        }

        public async Task<List<PaymentMethod>> FindEnabledPaymentMethods(int paymentPartner)
        {
            return await DbSet.Where(f => f.PaymentPartner == paymentPartner && f.Enabled).ToListAsync();
        }
    }
}
