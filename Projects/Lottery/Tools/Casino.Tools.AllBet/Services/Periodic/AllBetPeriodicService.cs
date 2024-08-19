using HnMicro.Framework.Options;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Partners;
using Lottery.Core.Partners.Models;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Partners.Periodic;
using Lottery.Core.Repositories.BetKind;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Casino.Tools.AllBet.Services.Periodic
{
    public class AllBetPeriodicService : AbstractPeriodicService<AllBetPeriodicService>
    {
        public AllBetPeriodicService(ILogger<AllBetPeriodicService> logger, IServiceProvider serviceProvider, ServiceOption serviceOption) : base(logger, serviceProvider, serviceOption)
        {
        }

        protected override PartnerType Partner { get; set; } = PartnerType.Allbet;

        protected override async Task InternalProcessMessages(List<IBaseMessageModel> messages)
        {
            using var scope = ServiceProvider.CreateScope();

            var lotteryUow = scope.ServiceProvider.GetService<ILotteryUow>();
            var betKindRepository = lotteryUow.GetRepository<IBetKindRepository>();
            var caService = scope.ServiceProvider.GetService<IPartnerService>();

            //  TODO Process message here...
            foreach (var item in messages)
            {
                try
                {
                    //  Belong to message type to perform.
                    var typeOfItem = item.GetType();
                    if (typeOfItem == typeof(CasinoAllBetPlayerModel))
                    {
                        var derivedItem = item as CasinoAllBetPlayerModel;
                        await caService.CreatePlayer(derivedItem);
                        Logger.LogInformation($"Partner: {item.Partner}. Create PlayerId: {derivedItem.PlayerId}.");
                    }
                    else if (typeOfItem == typeof(CasinoAllBetPlayerBetSettingModel))
                    {
                        var derivedItem = item as CasinoAllBetPlayerBetSettingModel;
                        await caService.UpdateBetSetting(derivedItem);
                        Logger.LogInformation($"Partner: {item.Partner}. Update bet setting player: {derivedItem.PlayerId}.");
                    }
                    else if (typeOfItem == typeof(CasinoAllBetPlayerLoginModel))
                    {
                        var derivedItem = item as CasinoAllBetPlayerLoginModel;
                        await caService.GenerateUrl(derivedItem);
                        Logger.LogInformation($"Partner: {item.Partner}. Login player: {derivedItem.PlayerId}.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error {ex.Message} when processing message: {item}.");
                    continue;
                }

            }
        }
    }
}
