using FluentValidation;
using Lottery.Agent.AgentService.Requests.CockFight;

namespace Lottery.Agent.AgentService.Validations.CockFight
{
    public class UpdateCockFightAgentPositionTakingRequestValidator : AbstractValidator<UpdateCockFightAgentPositionTakingRequest>
    {
        public UpdateCockFightAgentPositionTakingRequestValidator()
        {
            RuleFor(item => item.ActualPositionTaking).NotNull().LessThanOrEqualTo(x => x.DefaultPositionTaking);
        }
    }
}
