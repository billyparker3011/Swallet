namespace Lottery.Core.Partners
{
    public interface IPartnerService
    {
        Task<HttpResponseMessage> CreatePlayer(object data);
        Task UpdateBetSetting(object data);
        Task GenerateUrl(object data);
    }
}
