using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Models;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Core.Converters;
using SWallet.Core.Enums;
using SWallet.Core.Helpers;
using SWallet.Core.Models.Auth;
using SWallet.Data.Repositories.Customers;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Auth
{
    public class CustomerAuthenticationService : SWalletBaseService<CustomerAuthenticationService>, ICustomerAuthenticationService
    {
        private readonly IBuildTokenService _buildTokenService;

        public CustomerAuthenticationService(ILogger<CustomerAuthenticationService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext sWalletClientContext, ISWalletUow sWalletUow,
            IBuildTokenService buildTokenService) : base(logger, serviceProvider, configuration, clockService, sWalletClientContext, sWalletUow)
        {
            _buildTokenService = buildTokenService;
        }

        public async Task<JwtToken> Auth(AuthModel model)
        {
            var customerRepository = SWalletUow.GetRepository<ICustomerRepository>();
            var customer = await customerRepository.FindByUsername(model.Username.ToUpper()) ?? throw new BadRequestException(ErrorCodeHelper.Auth.UsernameOrPasswordIsWrong);
            var decodePassword = model.Password.DecodePassword().Md5();
            if (!customer.Password.Equals(decodePassword)) throw new BadRequestException(ErrorCodeHelper.Auth.UsernameOrPasswordIsWrong);
            if (customer.State == CustomerState.Closed.ToInt()) throw new BadRequestException(ErrorCodeHelper.Auth.UserClosed);
            if (customer.Lock) throw new BadRequestException(ErrorCodeHelper.Auth.UserLocked);

            var clientInformation = ClientContext.GetClientInformation();

            var hash = StringHelper.MaxHashLength.RandomString();
            var customerSessionRepository = SWalletUow.GetRepository<ICustomerSessionRepository>();
            var customerSession = await customerSessionRepository.FindByCustomerId(customer.CustomerId);
            if (customerSession == null)
            {
                customerSession = new Data.Core.Entities.CustomerSession
                {
                    CustomerId = customer.CustomerId,
                    Hash = hash,
                    IpAddress = clientInformation?.IpAddress,
                    UserAgent = clientInformation?.UserAgent,
                    Platform = clientInformation?.Platform,
                    State = SessionState.Online.ToInt(),
                    LatestDoingTime = ClockService.GetUtcNow()
                };
                customerSessionRepository.Add(customerSession);
            }
            else
            {
                customerSession.Hash = hash;
                customerSession.IpAddress = clientInformation?.IpAddress;
                customerSession.UserAgent = clientInformation?.UserAgent;
                customerSession.Platform = clientInformation?.Platform;
                customerSession.State = SessionState.Online.ToInt();
                customerSession.LatestDoingTime = ClockService.GetUtcNow();
                customerSessionRepository.Update(customerSession);
            }
            await SWalletUow.SaveChangesAsync();

            return _buildTokenService.BuildToken(customer.ToCustomerModel(), customerSession.ToCustomerSessionModel());
        }
    }
}
