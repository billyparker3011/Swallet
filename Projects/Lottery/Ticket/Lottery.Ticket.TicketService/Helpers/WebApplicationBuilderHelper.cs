using Lottery.Ticket.TicketService.Services.InternalInitial;

namespace Lottery.Ticket.TicketService.Helpers
{
    public static class WebApplicationBuilderHelper
    {
        public static void BuildInternalPlayerService(this WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService<InternalInitialService>();
        }
    }
}
