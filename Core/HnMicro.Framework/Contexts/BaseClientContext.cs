using HnMicro.Framework.Helpers;
using HnMicro.Framework.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace HnMicro.Framework.Contexts
{
    public abstract class BaseClientContext : IBaseClientContext
    {
        protected IHttpContextAccessor HttpContextAccessor;

        public BaseClientContext(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public ClientInformation GetClientInformation()
        {
            var headers = new List<string>();
            if (HttpContextAccessor.HttpContext.Request.Headers != null)
            {
                foreach (var item in HttpContextAccessor.HttpContext.Request.Headers)
                {
                    headers.Add($"{item.Key}={item.Value}");
                }
            }

            var userAgent = GetUserAgent();
            var browser = userAgent.GetBrowser();

            return new ClientInformation
            {
                IpAddress = GetIpAddress(),
                UserAgent = userAgent,
                Platform = GetPlatform(),
                Header = string.Join(Environment.NewLine, headers),
                Browser = browser,
                Domain = GetUserDomain()
            };
        }

        private string GetUserDomain()
        {
            if (HttpContextAccessor.HttpContext.Request.Headers == null) return string.Empty;
            if (!HttpContextAccessor.HttpContext.Request.Headers.TryGetValue("Origin", out StringValues domainValue)) return string.Empty;
            return domainValue.ToString().Replace("\"", "");
        }

        private string GetPlatform()
        {
            if (HttpContextAccessor.HttpContext.Request.Headers == null) return string.Empty;
            if (!HttpContextAccessor.HttpContext.Request.Headers.TryGetValue("sec-ch-ua-platform", out StringValues platformValue)) return string.Empty;
            return platformValue.ToString().Replace("\"", "");
        }

        private string GetUserAgent()
        {
            if (HttpContextAccessor.HttpContext.Request.Headers == null) return string.Empty;
            if (!HttpContextAccessor.HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentValue)) return string.Empty;
            return userAgentValue.ToString();
        }

        private string GetIpAddress()
        {
            if (HttpContextAccessor.HttpContext.Request.Headers == null || HttpContextAccessor.HttpContext.Connection.RemoteIpAddress == null) return string.Empty;
            if (HttpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues ipAddressValue))
            {
                var ipAddress = ipAddressValue.ToString();
                ipAddress = string.IsNullOrEmpty(ipAddress) ? string.Empty : ipAddress;
                if (!string.IsNullOrEmpty(ipAddress)) return ipAddress.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            }
            return IPAddress.IsLoopback(HttpContextAccessor.HttpContext.Connection.RemoteIpAddress)
                        ? IPAddress.Loopback.ToString()
                        : HttpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
