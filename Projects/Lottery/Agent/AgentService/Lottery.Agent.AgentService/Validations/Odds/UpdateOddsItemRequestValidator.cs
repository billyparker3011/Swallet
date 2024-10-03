using FluentValidation;
using Lottery.Agent.AgentService.Requests.Odds;
using Lottery.Core.Localizations;

namespace Lottery.Agent.AgentService.Validations.Odds
{
    public class UpdateOddsItemRequestValidator : AbstractValidator<UpdateOddsItemRequest>
    {
        public UpdateOddsItemRequestValidator()
        {
            RuleFor(f => f.Id)
                .NotEmpty().WithMessage(Messages.Odd.OddIdIsBlank)
                .GreaterThan(0L).WithMessage(Messages.Odd.OddIdIsBlank);

            RuleFor(f => f.BetKindId)
                .NotEmpty().WithMessage(Messages.Odd.BetKindIsBlank)
                .GreaterThan(0).WithMessage(Messages.Odd.OddIdIsBlank);

            RuleFor(f => f.MinBuy)
                .NotEmpty().WithMessage(Messages.Odd.MinBuyIsBlank)
                .GreaterThan(0).WithMessage(Messages.Odd.MinBuyIsBlank);

            RuleFor(f => f.MaxBuy)
                .NotEmpty().WithMessage(Messages.Odd.MaxBuyIsBlank)
                .GreaterThanOrEqualTo(f => f.MinBuy).WithMessage(Messages.Odd.MaxBuyIsGreaterThanOrEqualToMinBuy);

            RuleFor(f => f.Buy)
                .NotEmpty().WithMessage(Messages.Odd.BuyIsBlank)
                .GreaterThanOrEqualTo(f => f.MinBuy).WithMessage(Messages.Odd.BuyIsGreaterThanOrEqualToMinBuy)
                .LessThanOrEqualTo(f => f.MaxBuy).WithMessage(Messages.Odd.BuyIsLessThanOrEqualToMaxBuy);

            RuleFor(f => f.MinBet)
                .NotEmpty().WithMessage(Messages.Odd.MinBetIsBlank)
                .GreaterThan(0).WithMessage(Messages.Odd.MinBetIsGreaterThanZero);

            RuleFor(f => f.MaxBet)
                .NotEmpty().WithMessage(Messages.Odd.MaxBetIsBlank)
                .GreaterThanOrEqualTo(f => f.MinBet).WithMessage(Messages.Odd.MaxBetIsGreaterThanOrEqualToMinBet);

            RuleFor(f => f.MaxPerNumber)
                .NotEmpty().WithMessage(Messages.Odd.MaxPerNumberIsBlank)
                .GreaterThan(0).WithMessage(Messages.Odd.MaxPerNumberIsGreaterThanZero);
        }
    }
}
