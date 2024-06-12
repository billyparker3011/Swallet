using HnMicro.Core.Scopes;
using HnMicro.Framework.Contexts;
using Lottery.Core.Models.Client;

namespace Lottery.Core.Contexts
{
    public interface ILotteryClientContext : IBaseClientContext, ISingletonDependency
    {
        ClientAgentModel Agent { get; }
        ClientPlayerModel Player { get; }
    }
}
