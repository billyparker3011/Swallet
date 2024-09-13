using HnMicro.Framework.Enums;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Core.Models;
using SWallet.Core.Models.Bank;
using SWallet.Core.Models.Bank.GetBanks;
using SWallet.Data.Repositories.Banks;
using SWallet.Data.UnitOfWorks;
using System.Linq.Expressions;

namespace SWallet.Core.Services.Bank
{
    public class BankService : SWalletBaseService<BankService>, IBankService
    {
        public BankService(ILogger<BankService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ISWalletClientContext clientContext,
            ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
        }

        public async Task CreateBank(CreateBankModel model)
        {
            var bankRepos = SWalletUow.GetRepository<IBankRepository>();
            await bankRepos.AddAsync(new Data.Core.Entities.Bank
            {
                Name = model.Name,
                Icon = model.Icon,
                DepositEnabled = model.DepositEnabled,
                WithdrawEnabled = model.WithdrawEnabled,
                CreatedAt = ClockService.GetUtcNow(),
                CreatedBy = ClientContext.Manager.ManagerId
            });

            await SWalletUow.SaveChangesAsync();
        }

        public async Task DeleteBank(int id)
        {
            var bankRepos = SWalletUow.GetRepository<IBankRepository>();
            var bank = await bankRepos.FindByIdAsync(id) ?? throw new NotFoundException();
            bankRepos.Delete(bank);

            await SWalletUow.SaveChangesAsync();
        }

        public async Task<GetBanksResult> GetBanks(GetBanksModel query)
        {
            var bankRepos = SWalletUow.GetRepository<IBankRepository>();
            var bankQuery = bankRepos.FindQuery();
            if (!string.IsNullOrEmpty(query.SearchName))
            {
                bankQuery = bankQuery.Where(x => x.Name.Contains(query.SearchName));
            }
            if (query.SortType == SortType.Descending)
            {
                bankQuery = bankQuery.OrderByDescending(GetSortBankProperty(query));
            }
            else
            {
                bankQuery = bankQuery.OrderBy(GetSortBankProperty(query));
            }
            var result = await bankRepos.PagingByAsync(bankQuery, query.PageIndex, query.PageSize);
            return new GetBanksResult
            {
                Banks = result.Items.Select(x => new BankModel
                {
                    BankId = x.BankId,
                    BankName = x.Name,
                    DepositEnabled = x.DepositEnabled,
                    Icon = x.Icon,
                    WithdrawEnabled = x.WithdrawEnabled
                }),
                Metadata = new HnMicro.Framework.Responses.ApiResponseMetadata
                {
                    NoOfPages = result.Metadata.NoOfPages,
                    NoOfRows = result.Metadata.NoOfRows,
                    NoOfRowsPerPage = result.Metadata.NoOfRowsPerPage,
                    Page = result.Metadata.Page
                }
            };
        }

        private static Expression<Func<Data.Core.Entities.Bank, object>> GetSortBankProperty(GetBanksModel query)
        {
            if (string.IsNullOrEmpty(query.SortName)) return bank => bank.Name;
            return query.SortName?.ToLower() switch
            {
                "name" => bank => bank.Name,
                "depositenabled" => bank => bank.DepositEnabled,
                "withdrawenabled" => bank => bank.WithdrawEnabled
            };
        }

        public async Task UpdateBank(int id, CreateBankModel model)
        {
            var bankRepos = SWalletUow.GetRepository<IBankRepository>();
            var bank = await bankRepos.FindByIdAsync(id) ?? throw new NotFoundException();

            bank.Name = model.Name;
            bank.Icon = model.Icon;
            bank.DepositEnabled = model.DepositEnabled;
            bank.WithdrawEnabled = model.WithdrawEnabled;
            bank.UpdatedAt = ClockService.GetUtcNow();
            bank.UpdatedBy = ClientContext.Manager.ManagerId;

            await SWalletUow.SaveChangesAsync();
        }
    }
}
