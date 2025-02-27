﻿using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket;

namespace Lottery.Core.Services.Ticket;

public interface INormalizeTicketService : IScopedDependency
{
    void NormalizePlayer(List<TicketDetailModel> data, Dictionary<long, string> players);
    void NormalizeTicket(List<TicketDetailModel> data);
}
