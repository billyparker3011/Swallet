using FluentValidation;
using Lottery.Ticket.TicketService.Requests.Ticket;

namespace Lottery.Ticket.TicketService.Validations.Ticket;

public class ProcessMixedTicketRequestValidator : AbstractValidator<ProcessMixedTicketRequest>
{
    public ProcessMixedTicketRequestValidator()
    {
        RuleFor(f => f.BetKindId)
            .NotNull().WithMessage(Core.Localizations.Messages.Ticket.BetKindIdIsBlank)
            .NotEmpty().WithMessage(Core.Localizations.Messages.Ticket.BetKindIdIsBlank);


        RuleFor(f => f.MatchId)
            .NotNull().WithMessage(Core.Localizations.Messages.Ticket.MatchIdIsBlank)
            .NotEmpty().WithMessage(Core.Localizations.Messages.Ticket.MatchIdIsBlank);

        RuleFor(f => f.Numbers)
            .NotNull().WithMessage(Core.Localizations.Messages.Ticket.NumbersAreBlank)
            .Custom((f, context) =>
            {
                bool okNumber = true;
                foreach (var number in f)
                {
                    if (number >= 0 && number <= 99) continue;
                    okNumber = false;
                    break;
                }
                if (!okNumber)
                {
                    context.AddFailure(Core.Localizations.Messages.Ticket.NumberIsGreaterThanOrEqualToZero_LessThanOrEqual99);
                }

                var groupByNumber = f.GroupBy(f1 => f1).Select(f1 => new { f1.Key, Count = f1.Count() }).ToList();
                if (groupByNumber.Any(f1 => f1.Count > 1))
                {
                    context.AddFailure(Core.Localizations.Messages.Ticket.NumbersAreDuplicate);
                }
            });

        RuleFor(f => f.Points)
            .Custom((f, context) =>
            {
                bool okPoint = true;
                foreach (var point in f)
                {
                    if (point.Value > 0) continue;
                    okPoint = false;
                    break;
                }
                if (!okPoint)
                {
                    context.AddFailure(Core.Localizations.Messages.Ticket.PointIsGreaterThanToZero);
                }
            });
    }
}
