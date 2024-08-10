namespace Lottery.Core.Partners.Models.CA
{
    public class CAReponseModel
    {
        public CAReponseModel(int resultCode, string message) 
        {
            ResultCode = resultCode;
            Message = message ?? GetMessage(resultCode);
        }

        public int ResultCode { get; set; }
        public string Message { get; set; }

        private string GetMessage(int resultCode)
        {
            switch (resultCode)
            {
                case 0: return "Success";
                case 10000: return "Invalid Operator ID";
                case 10001: return "Invalid Signature";
                case 10003: return "Player account does not exist.";
                case 10005: return "Player account is disabled or not allowed to log in.";
                case 10006: return "Transaction not existed";
                case 10007: return "Invalid status";
                case 10008: return "Player is offline / logged out.";
                case 10100: return "Prohibit to bet.";
                case 10101: return "Credit is not enough.";
                case 10200: return "System is under maintenance.";
                case 40000: return "Invalid request parameter";
                case 50000: return "Server error";
            };
            return null;
        }
    }

    public class CABalanceResponseModel : CAReponseModel
    {
        public CABalanceResponseModel(int resultCode, string message, decimal balance, decimal version) : base(resultCode, message)
        {
            Balance = balance;
            Version = version;
        } 

        public decimal Balance { get; set; }
        public decimal Version { get; set; }
    }

    public class BetDetail
    {
        public int AppType { get; set; }
        public decimal BetAmount { get; set; }
        public int BetMethod { get; set; }
        public long BetNum { get; set; }
        //public DateTime BetTime { get; set; }
        public int BetType { get; set; }
        public decimal Commission { get; set; }
        public decimal Deposit { get; set; }
        public decimal ExchangeRate { get; set; }
        public long GameRoundId { get; set; }
        //public DateTime GameRoundStartTime { get; set; }
        public int GameType { get; set; }
        public string Ip { get; set; }
        public int Status { get; set; }
        public string TableName { get; set; }
    }

    public class Transaction
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public List<BetDetail> Details { get; set; }
        public bool IsRetry { get; set; }
        public string Player { get; set; }
        public long TranId { get; set; }
        public int Type { get; set; }
    }

}
