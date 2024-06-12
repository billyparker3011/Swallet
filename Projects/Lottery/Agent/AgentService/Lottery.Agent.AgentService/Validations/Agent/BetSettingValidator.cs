using FluentValidation;
using Lottery.Core.Dtos.Agent;

namespace Lottery.Agent.AgentService.Validations.Agent
{
    public class BetSettingValidator : AbstractValidator<AgentBetSettingDto>
    {
        public BetSettingValidator()
        {
            RuleFor(item => item.ActualBuy).NotNull().GreaterThanOrEqualTo(item => item.MinBuy).LessThanOrEqualTo(x => x.MaxBuy);
            RuleFor(item => item.ActualMinBet).NotNull().GreaterThanOrEqualTo(item => item.DefaultMinBet);
            RuleFor(item => item.ActualMaxBet).NotNull().LessThanOrEqualTo(item => item.DefaultMaxBet);
            RuleFor(item => item.ActualMaxPerNumber).NotNull().LessThanOrEqualTo(item => item.DefaultMaxPerNumber);
        }
    }
}
