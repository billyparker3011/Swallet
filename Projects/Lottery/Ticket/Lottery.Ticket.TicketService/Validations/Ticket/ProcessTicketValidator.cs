using FluentValidation;
using Lottery.Ticket.TicketService.Requests.Ticket;

namespace Lottery.Ticket.TicketService.Validations.Ticket;

public class ProcessTicketValidator : AbstractValidator<ProcessTicketRequest>
{
    public ProcessTicketValidator()
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
                var groupByNumber = f.GroupBy(f1 => f1.Number).Select(f1 => new { f1.Key, Count = f1.Count() }).ToList();
                if (groupByNumber.Any(f1 => f1.Count > 1))
                {
                    context.AddFailure(Core.Localizations.Messages.Ticket.NumbersAreDuplicate);
                }
            });
    }
}
