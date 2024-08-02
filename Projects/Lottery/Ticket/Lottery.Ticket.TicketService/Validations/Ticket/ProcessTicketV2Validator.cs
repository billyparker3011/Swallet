using FluentValidation;
using Lottery.Ticket.TicketService.Requests.Ticket;

namespace Lottery.Ticket.TicketService.Validations.Ticket;

public class ProcessTicketV2Validator : AbstractValidator<ProcessTicketV2Request>
{
    public ProcessTicketV2Validator()
    {
        RuleFor(f => f.MatchId)
            .NotNull().WithMessage(Core.Localizations.Messages.Ticket.MatchIdIsBlank)
            .NotEmpty().WithMessage(Core.Localizations.Messages.Ticket.MatchIdIsBlank);

        RuleFor(f => f.Details)
            .NotNull().WithMessage(Core.Localizations.Messages.Ticket.NumbersAreBlank)
            .Custom((details, context) =>
            {
                foreach (var item in details)
                {
                    if (item.BetKindId == 0) context.AddFailure(Core.Localizations.Messages.Ticket.BetKindIdIsBlank);
                    if (item.ChannelId == 0) context.AddFailure(Core.Localizations.Messages.Ticket.ChannelIdIsBlank);
                    var groupByNumber = item.Numbers.GroupBy(f1 => f1.Number).Select(f1 => new { f1.Key, Count = f1.Count() }).ToList();
                    if (groupByNumber.Any(f1 => f1.Count > 1)) context.AddFailure(Core.Localizations.Messages.Ticket.NumbersAreDuplicate);
                }
            });
    }
}