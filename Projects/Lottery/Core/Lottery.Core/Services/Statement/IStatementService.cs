using HnMicro.Core.Scopes;
using Lottery.Core.Models.Statement;

namespace Lottery.Core.Services.Statement
{
    public interface IStatementService : IScopedDependency
    {
        Task<List<StatementModel>> GetMyStatement();
    }
}
