using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoRequestService : IScopedDependency
    {
        Task<HttpResponseMessage> SendRequestAsync(HttpMethod httpMethod, string path, string requestBody);

        string GeneralAuthorizationHeader(string httpMethod, string path, string contentMD5, string contentType, string requestTime, string allbetApiKey, string operatorId);

        string GeneralUsername(string playerId, string suffix);

    }
}
