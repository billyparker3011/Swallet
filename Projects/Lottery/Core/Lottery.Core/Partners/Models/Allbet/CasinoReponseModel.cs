namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoReponseModel
    {
        public CasinoReponseModel(int resultCode, string message)
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
}
