using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Core.Converters;
using SWallet.Core.Models.Discounts;
using SWallet.Data.Repositories.Discounts;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Discount
{
    public class DiscountService : SWalletBaseService<DiscountService>, IDiscountService
    {
        public DiscountService(ILogger<DiscountService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext clientContext, ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
        }

        public async Task<List<DiscountModel>> GetStaticDiscount()
        {
            var discountRepository = SWalletUow.GetRepository<IDiscountRepository>();
            return (await discountRepository.FindStaticDiscounts()).Select(f => f.ToDiscountModel()).ToList();
        }
    }
}
