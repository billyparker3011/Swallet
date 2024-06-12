using FluentValidation;
using Lottery.Agent.AgentService.Requests.Odds;
using Lottery.Core.Localizations;

namespace Lottery.Agent.AgentService.Validations.Odds
{
    public class UpdateOddRequestValidator : AbstractValidator<UpdateOddsRequest>
    {
        public UpdateOddRequestValidator()
        {
            RuleFor(f => f.Odds)
                .NotEmpty()
                .WithMessage(Messages.Odd.OddsIsBlank);

            RuleForEach(x => x.Odds)
                .SetValidator(setting =>
                {
                    return new UpdateOddsItemRequestValidator();
                });
        }
    }
}
