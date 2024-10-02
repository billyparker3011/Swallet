using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Core.Converters;
using SWallet.Core.Enums;
using SWallet.Core.Models.Transactions;
using SWallet.Data.Repositories.Banks;
using SWallet.Data.Repositories.Customers;
using SWallet.Data.Repositories.Discounts;
using SWallet.Data.Repositories.Transactions;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Transaction
{
    public class TransactionService : SWalletBaseService<TransactionService>, ITransactionService
    {
        public TransactionService(ILogger<TransactionService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext clientContext, ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
        }

        public async Task CompletedTransaction(CompletedTransactionModel model)
        {
            var transactionRepository = SWalletUow.GetRepository<ITransactionRepository>();
            var transaction = await transactionRepository.FindByIdAsync(model.TransactionId) ?? throw new NotFoundException();
            if (transaction.TransactionState != TransactionState.Processing.ToInt()) throw new BadRequestException();

            var customerRepository = SWalletUow.GetRepository<ICustomerRepository>();
            var customer = await customerRepository.FindByIdAsync(transaction.CustomerId) ?? throw new NotFoundException();

            var balanceCustomerRepository = SWalletUow.GetRepository<IBalanceCustomerRepository>();
            var balance = await balanceCustomerRepository.FindByCustomerId(transaction.CustomerId) ?? throw new NotFoundException();

            var realAmount = 0m;
            if (customer.DiscountId.HasValue)
            {
                var discountRepository = SWalletUow.GetRepository<IDiscountRepository>();
                var discountDetailRepository = SWalletUow.GetRepository<IDiscountDetailRepository>();
                var discount = await discountRepository.FindByIdAsync(customer.DiscountId.Value) ?? throw new NotFoundException();
                var discountModel = discount.ToDiscountModel();
                if (discountModel.IsEnabled && transaction.TransactionType == TransactionType.Deposit.ToInt() && discountModel.Setting.Deposit != null)
                {
                    var discountDetails = await discountDetailRepository.FindByDiscountId(discountModel.DiscountId);
                    if ((discountDetails.Count + 1) <= discountModel.Setting.Deposit.NoOfApplyDiscount)
                    {
                        realAmount = model.Amount.GetDepositDiscountAmount(discountModel.Setting.Deposit);
                        var referenceId = Guid.NewGuid();

                        discountDetailRepository.Add(new Data.Core.Entities.DiscountDetail
                        {
                            CreatedAt = ClockService.GetUtcNow(),
                            CreatedBy = ClientContext.Manager.ManagerId,
                            CustomerId = customer.CustomerId,
                            DiscountId = discountModel.DiscountId,
                            ReferenceTransaction = referenceId
                        });

                        transactionRepository.Add(new Data.Core.Entities.Transaction
                        {
                            TransactionName = discountModel.DiscountName,
                            CustomerId = customer.CustomerId,
                            TransactionType = TransactionType.Discount.ToInt(),
                            OriginAmount = realAmount,
                            Amount = realAmount,
                            TransactionState = TransactionState.Success.ToInt(),
                            DiscountId = discountModel.DiscountId,
                            ReferenceTransactionId = transaction.TransactionId,
                            ReferenceDiscountDetail = referenceId,
                            CreatedAt = ClockService.GetUtcNow(),
                            CreatedBy = customer.CustomerId
                        });
                    }
                }
            }

            if (transaction.TransactionType == TransactionType.Deposit.ToInt())
            {
                balance.Balance += model.Amount + realAmount;
                balance.UpdatedAt = ClockService.GetUtcNow();
                balance.UpdatedBy = customer.CustomerId;
            }
            balanceCustomerRepository.Update(balance);

            if (transaction.TransactionType == TransactionType.Withdraw.ToInt())
            {
                var bankAccountRepository = SWalletUow.GetRepository<IBankAccountRepository>();
                var bankAccount = await bankAccountRepository.FindByBankAndBankAccount(model.BankId, model.BankAccountId) ?? throw new NotFoundException();

                transaction.WithdrawBankName = bankAccount.Bank.Name;
                transaction.WithdrawNumberAccount = bankAccount.NumberAccount;
                transaction.WithdrawCardHolder = bankAccount.CardHolder;
            }

            transaction.TransactionState = TransactionState.Success.ToInt();
            transaction.UpdatedAt = ClockService.GetUtcNow();
            transaction.UpdatedBy = ClientContext.Manager.ManagerId;
            transactionRepository.Update(transaction);

            await SWalletUow.SaveChangesAsync();
        }

        public async Task<TransactionsHistoryResultModel> GetTransactionsHistory(GetTransactionsHistoryModel model)
        {
            if (ClientContext.Customer == null) throw new ForbiddenException();

            var transactionRepository = SWalletUow.GetRepository<ITransactionRepository>();

            var targetCustomerId = model.CustomerId == 0L ? ClientContext.Customer.CustomerId : model.CustomerId;
            var query = model.GetAllCustomerTrans ? transactionRepository.FindQuery().Include(f => f.Customer) : transactionRepository.FindQueryBy(f => f.CustomerId == targetCustomerId).Include(f => f.Customer).AsQueryable();
            query = query.Where(f => f.CreatedAt >= model.From.UtcDateTime && f.CreatedAt <= model.To.UtcDateTime);
            if (model.TransactionType.HasValue) query = query.Where(f => f.TransactionType == model.TransactionType.Value);
            if (model.State.HasValue) query = query.Where(f => f.TransactionState == model.State.Value);

            query = query.OrderByDescending(f => f.CreatedAt);

            var result = await transactionRepository.PagingByAsync(query, model.PageIndex, model.PageSize);

            return new TransactionsHistoryResultModel
            {
                Transactions = result.Items.Select(f => f.ToTransactionModel()).ToList(),
                Metadata = new HnMicro.Framework.Responses.ApiResponseMetadata
                {
                    NoOfPages = result.Metadata.NoOfPages,
                    NoOfRows = result.Metadata.NoOfRows,
                    NoOfRowsPerPage = result.Metadata.NoOfRowsPerPage,
                    Page = result.Metadata.Page
                }
            };
        }

        public async Task<bool> RejectedTransaction(long transactionId)
        {
            var transactionRepository = SWalletUow.GetRepository<ITransactionRepository>();
            var transaction = await transactionRepository.FindByIdAsync(transactionId) ?? throw new NotFoundException();
            if (transaction.TransactionState == TransactionState.Rejected.ToInt()) return true;
            if (transaction.TransactionState == TransactionState.Processing.ToInt())
            {
                transaction.TransactionState = TransactionState.Rejected.ToInt();
                transaction.UpdatedAt = ClockService.GetUtcNow();
                transaction.UpdatedBy = ClientContext.Manager.ManagerId;
            }
            var discountDetailRepository = SWalletUow.GetRepository<IDiscountDetailRepository>();
            var customerRepository = SWalletUow.GetRepository<ICustomerRepository>();
            var customerBalanceRepository = SWalletUow.GetRepository<IBalanceCustomerRepository>();
            var customer = await customerRepository.FindByCustomerId(transaction.CustomerId) ?? throw new NotFoundException();
            if (transaction.TransactionState == TransactionState.Success.ToInt())
            {
                transaction.TransactionState = TransactionState.Rejected.ToInt();

                if (transaction.TransactionType == TransactionType.Discount.ToInt())
                {
                    var discountDetail = await discountDetailRepository.FindByReferenceTransaction(transaction.ReferenceDiscountDetail);
                    if (discountDetail != null) discountDetailRepository.Delete(discountDetail);

                    customer.CustomerBalance.Balance -= transaction.Amount;
                    customer.CustomerBalance.UpdatedAt = ClockService.GetUtcNow();
                    customer.CustomerBalance.UpdatedBy = ClientContext.Manager.ManagerId;
                    customerBalanceRepository.Update(customer.CustomerBalance);
                }
                if (transaction.TransactionType == TransactionType.Deposit.ToInt())
                {
                    customer.CustomerBalance.Balance -= transaction.Amount;
                    customer.CustomerBalance.UpdatedAt = ClockService.GetUtcNow();
                    customer.CustomerBalance.UpdatedBy = ClientContext.Manager.ManagerId;
                    customerBalanceRepository.Update(customer.CustomerBalance);
                }
                if (transaction.TransactionType == TransactionType.Withdraw.ToInt())
                {
                    customer.CustomerBalance.Balance += transaction.Amount;
                    customer.CustomerBalance.UpdatedAt = ClockService.GetUtcNow();
                    customer.CustomerBalance.UpdatedBy = ClientContext.Manager.ManagerId;
                    customerBalanceRepository.Update(customer.CustomerBalance);
                }
            }
            transactionRepository.Update(transaction);
            await SWalletUow.SaveChangesAsync();
            return true;
        }
    }
}
