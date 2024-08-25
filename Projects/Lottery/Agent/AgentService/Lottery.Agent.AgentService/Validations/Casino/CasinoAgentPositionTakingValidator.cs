using FluentValidation;
using Lottery.Core.Partners.Models.Allbet;

namespace Lottery.Agent.AgentService.Validations.Casino
{
    public class CasinoAgentPositionTakingValidator : AbstractValidator<UpdateCasinoAgentPositionTakingModel>
    {
        public CasinoAgentPositionTakingValidator()
        {
            RuleFor(item => item.AgentId).NotNull();
            RuleFor(item => item.BetKindId).NotNull();
            RuleFor(item => item.PositionTaking).NotNull().LessThanOrEqualTo(x => x.DefaultPositionTaking);
        }
    }

    public class UpdateCasinoAgentPositionTakingModelListValidator : AbstractValidator<List<UpdateCasinoAgentPositionTakingModel>>
    {
        public UpdateCasinoAgentPositionTakingModelListValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .Must(HaveAllNonNullElements)
                .Must(HaveAllUniqueElements);

            RuleForEach(x => x).SetValidator(new CasinoAgentPositionTakingValidator());
        }

        private bool HaveAllNonNullElements(List<UpdateCasinoAgentPositionTakingModel> positions)
        {
            return positions.All(p => p != null);
        }

        private bool HaveAllUniqueElements(List<UpdateCasinoAgentPositionTakingModel> positions)
        {
            return positions.GroupBy(c => c.BetKindId).Any(c => c.Count() <= 1);
        }
    }
}
