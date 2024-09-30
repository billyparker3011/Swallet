using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Core.Converters;
using SWallet.Core.Models;
using SWallet.Data.Core.Entities;
using SWallet.Data.Repositories.Banks;
using SWallet.Data.Repositories.Customers;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Customer
{
    public class CustomerBankAccountService : SWalletBaseService<CustomerBankAccountService>, ICustomerBankAccountService
    {
        public CustomerBankAccountService(ILogger<CustomerBankAccountService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext clientContext, ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
        }

        public async Task AddOrUpdate(AddOrUpdateCustomerBankAccountModel model, long id = 0L)
        {
            if (ClientContext.Customer == null) throw new ForbiddenException();

            var bankRepository = SWalletUow.GetRepository<IBankRepository>();
            var bank = await bankRepository.FindByIdAsync(model.BankId);
            if (bank == null) throw new NotFoundException();

            var customerBankAccountRepository = SWalletUow.GetRepository<ICustomerBankAccountRepository>();
            CustomerBankAccount cutomerBankAccount = null;
            if (id > 0L)
            {
                cutomerBankAccount = await customerBankAccountRepository.FindQueryBy(f => f.CustomerId == ClientContext.Customer.CustomerId && f.Id == id).FirstOrDefaultAsync();
                if (cutomerBankAccount == null) throw new NotFoundException();
            }
            if (cutomerBankAccount == null)
            {
                cutomerBankAccount = new CustomerBankAccount
                {
                    BankId = model.BankId,
                    CardHolder = model.CardHolder,
                    NumberAccount = model.NumberAccount,
                    CreatedBy = ClientContext.Customer.CustomerId,
                    CreatedAt = ClockService.GetUtcNow(),
                    CustomerId = ClientContext.Customer.CustomerId
                };
                customerBankAccountRepository.Add(cutomerBankAccount);
            }
            else
            {
                cutomerBankAccount.BankId = model.BankId;
                cutomerBankAccount.CardHolder = model.CardHolder;
                cutomerBankAccount.NumberAccount = model.NumberAccount;
                cutomerBankAccount.UpdatedAt = ClockService.GetUtcNow();
                cutomerBankAccount.UpdatedBy = ClientContext.Customer.CustomerId;
                customerBankAccountRepository.Update(cutomerBankAccount);
            }

            await SWalletUow.SaveChangesAsync();
        }

        public async Task<List<CustomerBankAccountModel>> GetCurrentCustomerBankAccounts()
        {
            if (ClientContext.Customer == null) throw new ForbiddenException();
            return await GetCustomerBankAccountsByCustomerId(ClientContext.Customer.CustomerId);
        }

        public async Task<List<CustomerBankAccountModel>> GetCustomerBankAccounts(long customerId)
        {
            return await GetCustomerBankAccountsByCustomerId(customerId);
        }

        private async Task<List<CustomerBankAccountModel>> GetCustomerBankAccountsByCustomerId(long customerId)
        {
            var customerBankAccountRepository = SWalletUow.GetRepository<ICustomerBankAccountRepository>();
            return await customerBankAccountRepository.FindQueryBy(f => f.CustomerId == customerId).Include(f => f.Bank).Select(f => f.ToCustomerBankAccountModel()).ToListAsync();
        }
    }
}
