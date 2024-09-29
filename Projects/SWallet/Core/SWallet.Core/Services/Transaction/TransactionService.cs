using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Core.Converters;
using SWallet.Core.Models.Transactions;
using SWallet.Data.Repositories.Transactions;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Transaction
{
    public class TransactionService : SWalletBaseService<TransactionService>, ITransactionService
    {
        public TransactionService(ILogger<TransactionService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext clientContext, ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
        }

        public async Task<TransactionsHistoryResultModel> GetTransactionsHistory(GetTransactionsHistoryModel model)
        {
            if (ClientContext.Customer == null) throw new ForbiddenException();

            var transactionRepository = SWalletUow.GetRepository<ITransactionRepository>();

            var targetCustomerId = model.CustomerId == 0L ? ClientContext.Customer.CustomerId : model.CustomerId;
            var query = transactionRepository.FindQueryBy(f => f.CustomerId == targetCustomerId && f.CreatedAt >= model.From.UtcDateTime && f.CreatedAt <= model.To.UtcDateTime);
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
    }
}
