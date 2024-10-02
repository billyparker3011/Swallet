using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Consts;
using SWallet.Core.Contexts;
using SWallet.Core.Models.Auth;
using SWallet.Data.Repositories.Customers;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Auth
{
    public class PasswordService : SWalletBaseService<PasswordService>, IPasswordService
    {
        public PasswordService(ILogger<PasswordService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext clientContext, ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
        }

        public async Task ChangeCustomerPassword(ChangePasswordModel model)
        {
            var customerRepository = SWalletUow.GetRepository<ICustomerRepository>();
            var customerId = model.CustomerId == 0L ? ClientContext.Customer.CustomerId : model.CustomerId;

            var customer = await customerRepository.FindByIdAsync(customerId);
            if (customer == null) throw new NotFoundException();

            var decodeOldPassword = model.OldPassword.DecodePassword();
            if (decodeOldPassword.Md5().Equals(customer.Password, StringComparison.OrdinalIgnoreCase)) throw new BadRequestException(CommonMessageConsts.OldPasswordDoesNotMatch);

            var decodeNewPassword = model.NewPassword.DecodePassword();
            if (!decodeNewPassword.IsStrongPassword()) throw new BadRequestException(CommonMessageConsts.PasswordIsTooWeak);

            customer.Password = decodeNewPassword.Md5();
            customer.ChangedPasswordAt = ClockService.GetUtcNow();
            await SWalletUow.SaveChangesAsync();
        }
    }
}
