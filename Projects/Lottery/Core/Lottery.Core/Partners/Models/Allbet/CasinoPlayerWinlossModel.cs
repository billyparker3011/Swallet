
namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoPlayerWinlossModel
    {
    }

    public class GetCasinoPlayerWinlossDetailModel
    {
        public long PlayerId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
