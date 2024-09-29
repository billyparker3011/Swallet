using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
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

        public async Task AddOrUpdateDiscount(AddOrUpdateDiscountModel model)
        {
            var discountRepository = SWalletUow.GetRepository<IDiscountRepository>();
            var discountId = model.DiscountId;
            if (discountId == 0)
            {
                discountRepository.Add(new Data.Core.Entities.Discount
                {
                    DiscountName = model.Name,
                    IsStatic = model.IsStatic,
                    IsEnabled = model.IsEnabled,
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = ClientContext.Manager.ManagerId,
                    Description = model.Description,
                    StartDate = model.StartedDate.HasValue ? model.StartedDate.Value.DateTime : null,
                    EndDate = model.EndedDate.HasValue ? model.EndedDate.Value.DateTime : null,
                    SportKindId = model.SportKindId,
                    Setting = model.Setting.NormalizeJsonString()
                });
            }
            else
            {
                var discount = await discountRepository.FindByIdAsync(discountId) ?? throw new NotFoundException();
                discount.DiscountName = model.Name;
                discount.IsEnabled = model.IsEnabled;
                discount.Description = model.Description;
                discount.StartDate = model.StartedDate.HasValue ? model.StartedDate.Value.DateTime : null;
                discount.EndDate = model.EndedDate.HasValue ? model.EndedDate.Value.DateTime : null;
                discount.SportKindId = model.SportKindId;
                discount.Setting = model.Setting.NormalizeJsonString();
                discountRepository.Update(discount);
            }

            await SWalletUow.SaveChangesAsync();
        }

        public async Task<GetDiscountsResultModel> GetDiscounts(GetDiscountsModel model)
        {
            var discountRepository = SWalletUow.GetRepository<IDiscountRepository>();
            var discountQuery = discountRepository.FindQueryBy(f => true);

            var keyword = string.IsNullOrEmpty(model.Keyword) ? string.Empty : model.Keyword.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                discountQuery = discountQuery.Where(f => f.DiscountName.Contains(keyword) || f.Description.Contains(keyword));
            }

            if (model.IsStatic.HasValue)
            {
                discountQuery = discountQuery.Where(f => f.IsStatic == model.IsStatic.Value);
            }

            if (model.SportKindId.HasValue)
            {
                discountQuery = discountQuery.Where(f => f.SportKindId.HasValue && f.SportKindId.Value == model.SportKindId.Value);
            }

            discountQuery = discountQuery.OrderByDescending(f => f.CreatedAt).ThenBy(f => f.DiscountName);
            var result = await discountRepository.PagingByAsync(discountQuery, model.PageIndex, model.PageSize);
            return new GetDiscountsResultModel
            {
                Discounts = result.Items.Select(f => f.ToDiscountModel()).ToList(),
                Metadata = new HnMicro.Framework.Responses.ApiResponseMetadata
                {
                    NoOfPages = result.Metadata.NoOfPages,
                    NoOfRows = result.Metadata.NoOfRows,
                    NoOfRowsPerPage = result.Metadata.NoOfRowsPerPage,
                    Page = result.Metadata.Page
                }
            };
        }

        public async Task<List<DiscountModel>> GetStaticDiscount()
        {
            var discountRepository = SWalletUow.GetRepository<IDiscountRepository>();
            return (await discountRepository.FindStaticDiscounts()).Select(f => f.ToDiscountModel()).ToList();
        }
    }
}
