using HnMicro.Core.Helpers;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Enums;
using Lottery.Core.InMemory.Ticket;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Ticket.TicketService.Services.InternalInitial
{
    public class InternalInitialService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public InternalInitialService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var inMemoryUow = scope.ServiceProvider.GetService<IInMemoryUnitOfWork>();
            var ticketInMemoryRepository = inMemoryUow.GetRepository<ITicketInMemoryRepository>();

            var lotteryUow = scope.ServiceProvider.GetService<ILotteryUow>();
            var ticketRepository = lotteryUow.GetRepository<ITicketRepository>();
            var waitingTickets = await ticketRepository.FindQueryBy(f => f.State == TicketState.Waiting.ToInt()).ToListAsync();
            var waitingRootTickets = waitingTickets.Where(f => !f.ParentId.HasValue).ToList();
            foreach (var rootTicket in waitingRootTickets)
            {
                ticketInMemoryRepository.Add(new Core.Models.Ticket.TicketModel
                {
                    CreatedAt = rootTicket.CreatedAt,
                    IsLive = rootTicket.IsLive,
                    TicketId = rootTicket.TicketId,
                    Children = waitingTickets.Where(f => f.ParentId == rootTicket.TicketId).Select(f => f.TicketId).ToList()
                });
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
