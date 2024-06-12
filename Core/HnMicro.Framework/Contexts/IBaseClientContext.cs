using HnMicro.Framework.Models;

namespace HnMicro.Framework.Contexts
{
    public interface IBaseClientContext
    {
        ClientInformation GetClientInformation();
    }
}
