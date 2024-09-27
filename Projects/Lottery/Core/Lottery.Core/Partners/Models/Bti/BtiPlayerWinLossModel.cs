using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Models.Bti
{
    public class BtiPlayerWinLossModel
    {
    }

    public class GetBtiPlayerWinlossDetailModel
    {
        public long PlayerId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
