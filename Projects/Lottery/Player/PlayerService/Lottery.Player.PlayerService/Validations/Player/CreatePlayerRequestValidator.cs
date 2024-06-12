using FluentValidation;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Localizations;
using Lottery.Player.PlayerService.Requests.Player;

namespace Lottery.Player.PlayerService.Validations.Player
{
    public class CreatePlayerRequestValidator : AbstractValidator<CreatePlayerRequest>
    {
        public CreatePlayerRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage(Messages.Agent.UserNameIsRequired)
                .Matches(@"[^\s]")
                .WithMessage(Messages.Agent.UserNameNotContainsWhiteSpace);
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(Messages.Agent.PasswordIsRequired);
            RuleFor(x => x.Credit)
                .NotNull()
                .WithMessage(Messages.Agent.CreditIsRequired);
            RuleForEach(x => x.BetSettings)
                .SetValidator(setting =>
                {
                    return new BetSettingValidation();
                });
        }

        public class BetSettingValidation : AbstractValidator<AgentBetSettingDto>
        {
            public BetSettingValidation()
            {
                RuleFor(item => item.ActualBuy).NotNull().GreaterThanOrEqualTo(item => item.MinBuy).LessThanOrEqualTo(x => x.MaxBuy);
                RuleFor(item => item.ActualMinBet).NotNull().GreaterThanOrEqualTo(item => item.DefaultMinBet);
                RuleFor(item => item.ActualMaxBet).NotNull().LessThanOrEqualTo(item => item.DefaultMaxBet);
                RuleFor(item => item.ActualMaxPerNumber).NotNull().LessThanOrEqualTo(item => item.DefaultMaxPerNumber);
            }
        }
    }
}
