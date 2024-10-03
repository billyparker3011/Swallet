namespace Lottery.Core.Partners
{
    public interface IPartnerService
    {
        Task CreatePlayer(object data);
        Task UpdateBetSetting(object data);
        Task GenerateUrl(object data);
        Task<Dictionary<string, object>> ScanTickets(Dictionary<string, object> metadata);
    }
}
