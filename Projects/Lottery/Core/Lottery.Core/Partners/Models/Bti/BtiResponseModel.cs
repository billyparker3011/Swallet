using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Models.Bti
{
    public class BtiResponseModel
    {
        public int error_code { get; set; }
        public string error_message { get; set; }
        public decimal balance { get; set; }
      
    }

    public class BtiValidateTokenResponseModel : BtiResponseModel
    {
        public string cust_id { get; set; }
        public string cust_login { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string currency_code { get; set; }
        public string extSessionID { get; set; }
        public string data { get; set; }
    }

    public class BtiReserveResponseModel : BtiResponseModel
    {
        public long trx_id { get; set; }
        public Decimal BonusUsed { get; set; }
    }

    public class BtiDebitReserveResponseModel : BtiResponseModel
    {
        public long trx_id { get; set; }
        public int ExtBonusID { get; set; }
    }

    public class BtiBaseResponseModel : BtiResponseModel
    {
        public long trx_id { get; set; }
    }
}
