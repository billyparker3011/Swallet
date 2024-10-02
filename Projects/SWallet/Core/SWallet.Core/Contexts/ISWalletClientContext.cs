using HnMicro.Core.Scopes;
using HnMicro.Framework.Contexts;
using SWallet.Core.Models.Clients;

namespace SWallet.Core.Contexts
{
    public interface ISWalletClientContext : IBaseClientContext, ISingletonDependency
    {
        ClientOfManagerModel Manager { get; }
        ClientOfCustomerModel Customer { get; }

        void ValidationPrepareToken();
    }
}
