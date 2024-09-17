using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Core.Models;
using SWallet.Core.Models.Bank;
using SWallet.Core.Models.Bank.GetBankAccounts;
using SWallet.Data.Repositories.Banks;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Bank
{
    public class BankAccountService : SWalletBaseService<BankAccountService>, IBankAccountService
    {
        public BankAccountService(ILogger<BankAccountService> logger, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IClockService clockService, 
            ISWalletClientContext clientContext, 
            ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
        }

        public async Task CreateBankAccount(CreateBankAccountModel model)
        {
            var bankAccountRepos = SWalletUow.GetRepository<IBankAccountRepository>();
            await bankAccountRepos.AddAsync(new Data.Core.Entities.BankAccount
            {
                BankId = model.BankId,
                NumberAccount = model.NumberAccount,
                CardHolder = model.CardHolder,
                CreatedAt = ClockService.GetUtcNow(),
                CreatedBy = ClientContext.Manager.ManagerId
            });

            await SWalletUow.SaveChangesAsync();
        }

        public async Task DeleteBankAccount(int id)
        {
            var bankAccountRepos = SWalletUow.GetRepository<IBankAccountRepository>();
            var bankAccount = await bankAccountRepos.FindByIdAsync(id) ?? throw new NotFoundException();
            bankAccountRepos.Delete(bankAccount);

            await SWalletUow.SaveChangesAsync();
        }

        public async Task<GetBankAccountsResult> GetBankAccounts()
        {
            var bankRepos = SWalletUow.GetRepository<IBankRepository>();
            var bankAccountRepos = SWalletUow.GetRepository<IBankAccountRepository>();
            var bankAccounts = await bankAccountRepos.FindQuery().Select(x => new BankAccountModel
            {
                BankAccountId = x.BankAccountId,
                BankId = x.BankId,
                CardHolder = x.CardHolder,
                NumberAccount = x.NumberAccount
            }).ToListAsync();
            var bankIds = bankAccounts.Select(x => x.BankId).ToList();
            var banks = await bankRepos.FindQueryBy(x => bankIds.Contains(x.BankId)).ToListAsync();
            foreach (var account in bankAccounts)
            {
                account.BankName = banks.FirstOrDefault(x => x.BankId == account.BankId)?.Name;
                account.NumberAccount = account.NumberAccount.Count()  >= 4 ? account.NumberAccount.Substring(0, account.NumberAccount.Length - 4) + "XXXX" : account.NumberAccount;
            }
            return new GetBankAccountsResult
            {
                BankAccounts = bankAccounts
            };
        }

        public async Task UpdateBankAccount(int id, CreateBankAccountModel model)
        {
            var bankAccountRepos = SWalletUow.GetRepository<IBankAccountRepository>();
            var bankAccount = await bankAccountRepos.FindByIdAsync(id) ?? throw new NotFoundException();

            bankAccount.BankId = model.BankId;
            bankAccount.NumberAccount = model.NumberAccount;
            bankAccount.CardHolder = model.CardHolder;
            bankAccount.UpdatedAt = ClockService.GetUtcNow();
            bankAccount.UpdatedBy = ClientContext.Manager.ManagerId;

            await SWalletUow.SaveChangesAsync();
        }
    }
}
