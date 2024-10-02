using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Models;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Consts;
using SWallet.Core.Contexts;
using SWallet.Core.Converters;
using SWallet.Core.Enums;
using SWallet.Core.Models.Customers;
using SWallet.Core.Services.Auth;
using SWallet.Data.Core.Entities;
using SWallet.Data.Repositories.Customers;
using SWallet.Data.Repositories.Managers;
using SWallet.Data.Repositories.Roles;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Customer
{
    public class RegistrationService : SWalletBaseService<RegistrationService>, IRegistrationService
    {
        private readonly IBuildTokenService _buildCustomerTokenService;

        public RegistrationService(ILogger<RegistrationService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext clientContext, ISWalletUow sWalletUow,
            IBuildTokenService buildCustomerTokenService) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
            _buildCustomerTokenService = buildCustomerTokenService;
        }

        public async Task<JwtToken> Register(RegisterModel model)
        {
            var firstName = string.IsNullOrEmpty(model.FirstName) ? string.Empty : model.FirstName;
            if (string.IsNullOrEmpty(firstName)) throw new BadRequestException(-1, CommonMessageConsts.FirstNameIsRequired);

            var lastName = string.IsNullOrEmpty(model.LastName) ? string.Empty : model.LastName;
            if (string.IsNullOrEmpty(lastName)) throw new BadRequestException(-1, CommonMessageConsts.LastNameIsRequired);

            var email = string.IsNullOrEmpty(model.Email) ? string.Empty : model.Email.ToLower();
            if (string.IsNullOrEmpty(email)) throw new BadRequestException(-1, CommonMessageConsts.EmailIsRequired);
            if (!email.IsEmail()) throw new BadRequestException(-1, CommonMessageConsts.EmailIsNotValid);

            var username = string.IsNullOrEmpty(model.Username) ? string.Empty : model.Username;
            if (string.IsNullOrEmpty(username)) throw new BadRequestException(-1, CommonMessageConsts.UserNameIsRequired);

            var password = string.IsNullOrEmpty(model.Password) ? string.Empty : model.Password;
            if (string.IsNullOrEmpty(password)) throw new BadRequestException(-1, CommonMessageConsts.PasswordIsRequired);
            var decodePassword = password.DecodePassword();
            if (!decodePassword.IsStrongPassword()) throw new BadRequestException(-1, CommonMessageConsts.PasswordIsTooWeak);

            var telegram = string.IsNullOrEmpty(model.Telegram) ? string.Empty : model.Telegram;
            var phone = string.IsNullOrEmpty(model.Phone) ? string.Empty : model.Phone;
            var affiliateCode = string.IsNullOrEmpty(model.AffiliateCode) ? string.Empty : model.AffiliateCode;

            var customerRepository = SWalletUow.GetRepository<ICustomerRepository>();
            var customerSessionRepository = SWalletUow.GetRepository<ICustomerSessionRepository>();

            var customerByEmail = await customerRepository.FindByEmail(email);
            if (customerByEmail != null) throw new BadRequestException(-1, CommonMessageConsts.EmailWasUsed);

            var customerByUsername = await customerRepository.FindByUsername(username.ToUpper());
            if (customerByUsername != null) throw new BadRequestException(-1, CommonMessageConsts.UsernameWasUsed);

            var roleRepository = SWalletUow.GetRepository<IRoleRepository>();
            var role = await roleRepository.GetRoleByRoleCode(RoleConsts.RoleAsCustomer);
            if (role == null) throw new BadRequestException(-1, CommonMessageConsts.RoleHasNotBeenInitialYet);

            var clientInformation = ClientContext.GetClientInformation();
            var hash = StringHelper.MaxHashLength.RandomString();

            var customer = new Data.Core.Entities.Customer
            {
                Username = username,
                UsernameUpper = username.ToUpper(),
                Password = decodePassword.Md5(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Telegram = telegram,
                State = CustomerState.Open.ToInt(),
                RoleId = role.RoleId,
                CreatedAt = ClockService.GetUtcNow(),
                CreatedBy = 0L,
                DepositAllowed = true,
                DiscountAllowed = true,
                WithdrawAllowed = true,
                DiscountId = model.DiscountId
            };

            var customerSession = new CustomerSession
            {
                Customer = customer,
                Hash = hash,
                IpAddress = clientInformation?.IpAddress,
                Platform = clientInformation?.Platform,
                UserAgent = clientInformation?.UserAgent,
                State = SessionState.Initial.ToInt()
            };

            customer.CustomerSession = customerSession;

            Data.Core.Entities.Manager managerByAffiliate = null;
            if (!string.IsNullOrEmpty(affiliateCode))
            {
                var managerRepository = SWalletUow.GetRepository<IManagerRepository>();
                managerByAffiliate = await managerRepository.FindByAffiliateCode(affiliateCode);
                if (managerByAffiliate == null) throw new BadRequestException(-1, CommonMessageConsts.CouldNotFindAgentPromoCode);
            }

            if (managerByAffiliate != null)
            {
                customer.IsAffiliate = true;
                customer.AgentId = managerByAffiliate.ManagerId;
                customer.MasterId = managerByAffiliate.MasterId;
                customer.SupermasterId = managerByAffiliate.SupermasterId;
            }

            var balanceCustomerRepository = SWalletUow.GetRepository<IBalanceCustomerRepository>();
            balanceCustomerRepository.Add(new BalanceCustomer
            {
                CreatedAt = ClockService.GetUtcNow(),
                CreatedBy = 0L,
                Customer = customer,
                Balance = 0m
            });
            customerRepository.Add(customer);
            customerSessionRepository.Add(customerSession);

            await SWalletUow.SaveChangesAsync();

            return _buildCustomerTokenService.BuildToken(customer.ToCustomerModel(), customerSession.ToCustomerSessionModel());
        }
    }
}
