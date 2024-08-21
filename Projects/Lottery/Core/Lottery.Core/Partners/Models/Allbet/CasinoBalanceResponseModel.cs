namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoBalanceResponseModel : CasinoReponseModel
    {
        public CasinoBalanceResponseModel(int resultCode, string message, decimal? balance, decimal? version) : base(resultCode, message)
        {
            Balance = balance;
            Version = version;
        }

        public decimal? Balance { get; set; }
        public decimal? Version { get; set; }
    }
}
