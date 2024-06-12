using System.Net;

namespace HnMicro.Core.Helpers
{
    public static class NetworkHelper
    {
        public static string GetHostname()
        {
            return Dns.GetHostName();
        }
    }
}
