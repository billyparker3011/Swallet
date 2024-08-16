using HnMicro.Core.Scopes;
using HnMicro.Framework.Exceptions;
using Lottery.Core.Helpers;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Casino
{
    public interface ICasinoAlibetService: IScopedDependency
    {
        Task CreatePlayer(object data);

        Task GenerateUrl(object data);

        Task UpdateBetSetting(object data);
    }
}
