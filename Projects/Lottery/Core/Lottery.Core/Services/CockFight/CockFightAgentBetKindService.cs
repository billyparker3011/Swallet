using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Models.CockFight.GetCockFightAgentBetKind;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.CockFight
{
    public class CockFightAgentBetKindService : LotteryBaseService<CockFightAgentBetKindService>, ICockFightAgentBetKindService
    {
        public CockFightAgentBetKindService(ILogger<CockFightAgentBetKindService> logger, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IClockService clockService, 
            ILotteryClientContext clientContext, 
            ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task<GetCockFightAgentBetKindModel> GetCockFightAgentBetKind()
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var cockfightBetKindRepos = LotteryUow.GetRepository<ICockFightBetKindRepository>();
<<<<<<< HEAD
            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            return await cockfightBetKindRepos.FindQuery().Select(x => new GetCockFightAgentBetKindModel 
            { 
=======
            _ = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            return await cockfightBetKindRepos.FindQuery().Select(x => new GetCockFightAgentBetKindModel 
            { 
                Id = x.Id,
>>>>>>> develop
                Name = x.Name,
                Enabled = x.Enabled
            }).FirstOrDefaultAsync();

        }

        public async Task UpdateCockFightAgentBetKind(GetCockFightAgentBetKindModel model)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var cockfightBetKindRepos = LotteryUow.GetRepository<ICockFightBetKindRepository>();
<<<<<<< HEAD
            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            var masterCockFightBetKind = await cockfightBetKindRepos.FindQuery().FirstOrDefaultAsync() ?? throw new NotFoundException();
=======
            _ = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            var masterCockFightBetKind = await cockfightBetKindRepos.FindByIdAsync(model.Id) ?? throw new NotFoundException();
>>>>>>> develop

            masterCockFightBetKind.Name = model.Name ?? masterCockFightBetKind.Name;
            masterCockFightBetKind.Enabled = model.Enabled ?? masterCockFightBetKind.Enabled;

            await LotteryUow.SaveChangesAsync();
        }
    }
}
