using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Models.Bti
{
    public class BtiTokenModel
    {
        public long PlayerId { get; set; }
        public DateTime ExpiryTime { get; set; }
    }

    public class BtiOutTokenModel : BtiTokenModel
    {
        public bool IsExpired => ExpiryTime <= DateTime.UtcNow;
        public bool IsValid => PlayerId > 0 && !IsExpired;
    }
}
