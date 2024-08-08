using FluentValidation;
using Lottery.Player.PlayerService.Requests.CockFight;

namespace Lottery.Player.PlayerService.Validations.CockFight
{
    public class UpdateCockFightPlayerBetSettingRequestValidator : AbstractValidator<UpdateCockFightPlayerBetSettingRequest>
    {
        public UpdateCockFightPlayerBetSettingRequestValidator()
        {
            RuleFor(item => item.MainLimitAmountPerFight).NotNull().LessThanOrEqualTo(x => x.DefaultMaxMainLimitAmountPerFight);
            RuleFor(item => item.DrawLimitAmountPerFight).NotNull().LessThanOrEqualTo(item => item.DefaultMaxDrawLimitAmountPerFight);
            RuleFor(item => item.LimitNumTicketPerFight).NotNull().LessThanOrEqualTo(item => item.DefaultMaxLimitNumTicketPerFight);
        }
    }
}
