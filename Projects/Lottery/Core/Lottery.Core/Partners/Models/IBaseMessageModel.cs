using Lottery.Core.Enums.Partner;

namespace Lottery.Core.Partners.Models
{
    public interface IBaseMessageModel
    {
        public PartnerType Partner { get; set; }
    }
}
