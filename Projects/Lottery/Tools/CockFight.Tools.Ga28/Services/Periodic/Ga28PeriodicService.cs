using HnMicro.Framework.Options;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Partners;
using Lottery.Core.Partners.Models;
using Lottery.Core.Partners.Models.Ga28;
using Lottery.Core.Partners.Periodic;
using Lottery.Core.Repositories.BetKind;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CockFight.Tools.Ga28.Services.Periodic
{
    public class Ga28PeriodicService : AbstractPeriodicService<Ga28PeriodicService>
    {
        public Ga28PeriodicService(ILogger<Ga28PeriodicService> logger, IServiceProvider serviceProvider, ServiceOption serviceOption) : base(logger, serviceProvider, serviceOption)
        {
        }

        protected override PartnerType Partner { get; set; } = PartnerType.GA28;

        protected override async Task InternalProcessMessages(List<IBaseMessageModel> messages)
        {
            using var scope = ServiceProvider.CreateScope();

            var lotteryUow = scope.ServiceProvider.GetService<ILotteryUow>();
            var betKindRepository = lotteryUow.GetRepository<IBetKindRepository>();
            var ga28Service = scope.ServiceProvider.GetService<IPartnerService>();

            //  TODO Process message here...
            foreach (var item in messages)
            {
                //  Belong to message type to perform.
                var typeOfItem = item.GetType();
                if (typeOfItem == typeof(Ga28CreateMemberModel))
                {
                    var derivedItem = item as Ga28CreateMemberModel;
                    await ga28Service.CreatePlayer(derivedItem);
                    Logger.LogInformation($"Partner: {item.Partner}. PlayerId: {derivedItem.PlayerId}.");
                }
                else if (typeOfItem == typeof(Ga28SyncUpBetSettingModel))
                {
                    var derivedItem = item as Ga28SyncUpBetSettingModel;
                    await ga28Service.UpdateBetSetting(derivedItem);
                    Logger.LogInformation($"Partner: {item.Partner}. Update bet setting member: {derivedItem.MemberRefId}.");
                }
                else if (typeOfItem == typeof(Ga28LoginPlayerModel))
                {
                    var derivedItem = item as Ga28LoginPlayerModel;
                    await ga28Service.GenerateUrl(derivedItem);
                    Logger.LogInformation($"Partner: {item.Partner}. Login member: {derivedItem.MemberRefId}.");
                }
            }
        }
    }
}
