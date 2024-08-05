namespace Lottery.Core.Partners.Models.Tests
{
    public class Ga28LoginPlayerDataReturnModel
    {
        public string Token { get; set; }
        public string Game_client_url { get; set; }
        public MemberInfo Member { get; set; }
    }

    public class MemberInfo
    {
        public string Member_ref_id { get; set; }
        public string Display_name { get; set; }
        public string Account_id { get; set; }
        public bool Enabled { get; set; }
        public bool Freeze { get; set; }
        public decimal? Main_limit_amount_per_fight { get; set; }
        public decimal? Draw_limit_amount_per_fight { get; set; }
        public decimal? Limit_num_ticket_per_fight { get; set; }
    }
}
