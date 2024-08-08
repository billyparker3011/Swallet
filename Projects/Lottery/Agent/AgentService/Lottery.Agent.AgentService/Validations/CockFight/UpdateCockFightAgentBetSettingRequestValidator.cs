using FluentValidation;
using Lottery.Agent.AgentService.Requests.CockFight;

namespace Lottery.Agent.AgentService.Validations.CockFight
{
    public class UpdateCockFightAgentBetSettingRequestValidator : AbstractValidator<UpdateCockFightAgentBetSettingRequest>
    {
        public UpdateCockFightAgentBetSettingRequestValidator()
        {
            RuleFor(item => item.MainLimitAmountPerFight).NotNull().LessThanOrEqualTo(x => x.DefaultMaxMainLimitAmountPerFight);
            RuleFor(item => item.DrawLimitAmountPerFight).NotNull().LessThanOrEqualTo(item => item.DefaultMaxDrawLimitAmountPerFight);
            RuleFor(item => item.LimitNumTicketPerFight).NotNull().LessThanOrEqualTo(item => item.DefaultMaxLimitNumTicketPerFight);
        }
    }
}
