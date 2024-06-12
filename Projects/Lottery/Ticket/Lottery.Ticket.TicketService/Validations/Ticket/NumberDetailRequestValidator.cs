using FluentValidation;
using Lottery.Ticket.TicketService.Requests.Ticket;

namespace Lottery.Ticket.TicketService.Validations.Ticket;

public class NumberDetailRequestValidator : AbstractValidator<NumberDetailRequest>
{
    public NumberDetailRequestValidator()
    {
        RuleFor(f => f.Number)
            .GreaterThanOrEqualTo(f => 0)
            .LessThanOrEqualTo(f => 99);

        RuleFor(f => f.Point)
            .GreaterThan(f => 0);

        RuleFor(f => f.Odd)
            .GreaterThan(f => 0);
    }
}