using FluentValidation;
using Lottery.Match.MatchService.Requests.Match;

namespace Lottery.Match.MatchService.Validations.Match
{
    public class CreateOrUpdateMatchValidator : AbstractValidator<CreateOrUpdateMatchRequest>
    {
        public CreateOrUpdateMatchValidator()
        {
            var vnCurrentTime = DateTime.UtcNow.AddHours(7);

            RuleFor(f => f.KickOff.Date)
                .LessThanOrEqualTo(vnCurrentTime.Date)
                .WithMessage(Core.Localizations.Messages.Match.KickOffIsGreaterOrEqualsCurrentDate);
        }
    }
}
