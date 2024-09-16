using Lottery.Core.Helpers;
using Lottery.Core.Partners.Helpers;
using static Lottery.Core.Partners.Helpers.BtiHelper;

namespace Lottery.Core.Partners.Models.Bti
{
    public class BtiResponseModel
    {
        public BtiResponseModel(int code)
        {
            error_code = code;
            error_message = GetMessage(code);
        }
        public int error_code { get; set; }
        public string error_message { get; set; }
        public decimal balance { get; set; }
        private string GetMessage(int code)
        {
            return PartnerHelper.FindStaticFieldName(typeof(BtiResponseCodeHelper), code);
        }
    }

    public class BtiValidateTokenResponseModel : BtiResponseModel
    {
        public BtiValidateTokenResponseModel(int code = 0) : base(code) { }

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
        public BtiReserveResponseModel(int code = 0) : base(code) { }
        public long trx_id { get; set; }
        public Decimal BonusUsed { get; set; }
    }

    public class BtiDebitReserveResponseModel : BtiResponseModel
    {
        public BtiDebitReserveResponseModel(int code = 0) : base(code) { }
        public long trx_id { get; set; }
        public int ExtBonusID { get; set; }
    }

    public class BtiBaseResponseModel : BtiResponseModel
    {
        public BtiBaseResponseModel(int code = 0) : base(code) { }
        public long trx_id { get; set; }
    }
}
