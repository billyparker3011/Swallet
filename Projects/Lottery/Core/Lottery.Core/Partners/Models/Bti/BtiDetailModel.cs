using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Lottery.Core.Partners.Models.Bti
{
    public class BtiDetailModel
    {
        public string ChangeID { get; set; }
        public int CustomerID { get; set; }
        public string MerchantCustomerCode { get; set; }
        public int BetID { get; set; }
        public int PurchaseID { get; set; }
        public int seq_num { get; set; }
        public DateTime CreationDateUTC { get; set; }
        public int BetTypeID { get; set; }
        public string BetTypeName { get; set; }
        public int LineID { get; set; }
        public string NewlineID { get; set; }
        public int LineTypeID { get; set; }
        public string LineTypeName { get; set; }
        public int RowTypeID { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public int MasterLeagueID { get; set; }
        public int LeagueID { get; set; }
        public int NewLeagueID { get; set; }
        public string LeagueName { get; set; }
        public DateTime CreationDate { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public decimal Stake { get; set; }
        public decimal OrgStake { get; set; }
        public decimal Gain { get; set; }
        public string ActionType { get; set; }
        public string PartialCashOutOriginalBetID { get; set; }
        public string RemainingStakeAfterPartialCashout { get; set; }
        public string CurrentBalance { get; set; }
        public int Odds { get; set; }
        public decimal OddsDec { get; set; }
        public decimal Points { get; set; }
        public string Score { get; set; }
        public string CurrentResult { get; set; }
        public int ComboBetNumersLines { get; set; }
        public int Combinations { get; set; }
        public string CurrentStatus { get; set; }
        public int NumberOfLines { get; set; }
        public int NumberOfOpenLines { get; set; }
        public int NumberOfLostLines { get; set; }
        public int NumberOfWonLines { get; set; }
        public int NumberOfCanceledLines { get; set; }
        public int NumberOfDrawLines { get; set; }
        public int NumberOfCashoutLines { get; set; }
        public int NumberOfSettledLines { get; set; }
        public string cust_id { get; set; }
        public int reserve_id { get; set; }
        public decimal amount { get; set; }
        public int NumberOfBets { get; set; }
        public int IsFreeBet { get; set; }
        public string FreebetAmount { get; set; }
        public string RealAmount { get; set; }
        public int CommomStatusID { get; set; }
        public int BonusID { get; set; }
        public decimal OddsInUserStyle { get; set; }
        public string UserOddStyle { get; set; }
        public decimal RealStake { get; set; }
        public string Status { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public string YourBet { get; set; }
        public int IsLive { get; set; }
        public int EventTypeID { get; set; }
        public string EventTypeName { get; set; }
        public int TeamMappingID { get; set; }
        public string LiveScore1 { get; set; }
        public string LiveScore2 { get; set; }
        public DateTime EventDate { get; set; }
        public int MasterEventID { get; set; }
        public int NewMasterEventID { get; set; }
        public int EventID { get; set; }
        public int NewEventID { get; set; }
        public string SOGEIID { get; set; }
        public int WebProviderID { get; set; }
        public string EachWaySetting { get; set; }
        public string EventState { get; set; }
        public int IsResettlement { get; set; }
        public string Platform { get; set; }
        public string currency_code { get; set; }
        public int DepositBonusId { get; set; }
        public int DepositBonusStatus { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal RolloverState { get; set; }
        public string TransEventName { get; set; }
        public string TransLeagueName { get; set; }
        public string TransBranchName { get; set; }
        public string TransYourBet { get; set; }
        public string TransEventTypeName { get; set; }
        public string TransAwayTeam { get; set; }
        public string TransHomeTeam { get; set; }
        public int ReferenceID { get; set; }
        public string ReserveAmountType { get; set; }
        public int ReserveAmountTypeID { get; set; }
        public string EventName { get; set; }
        public int SRIJbetCode { get; set; }
        public string Country { get; set; }
        public int Index { get; set; }
        public string MarketID { get; set; }
        public string CustomerName { get; set; }
        public int DomainID { get; set; }
        public int ReserveID { get; set; }
        public int MerchantReserveID { get; set; }
        public int ExtBonusContribution { get; set; }
        public decimal DecimalOdds { get; set; }
        public DateTime EventDateUTC { get; set; }
        public string EncodedID { get; set; }
        public int BestOddsApplied { get; set; }
        public int SelectionID { get; set; }
        public decimal PrevBalance { get; set; }
        public decimal NewBalance { get; set; }
        public DateTime DateUTC { get; set; }
        public string TriggeredResult { get; set; }
        public string BetType { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal AmountBeforeTax { get; set; }
        public decimal TOTax { get; set; }
        public decimal TaxedStake { get; set; }
        public DateTime BetSettledDate { get; set; }
        public string BonusCampaignID { get; set; }
        public int FreebetSource { get; set; }
        public string ContributionAmount { get; set; }
        public int ContributionType { get; set; }
    }


    [XmlRoot("Credit")]
    public class BtiCredit
    {
        [XmlAttribute("CustomerID")]
        public string CustomerID { get; set; }

        [XmlAttribute("CustomerName")]
        public string CustomerName { get; set; }

        [XmlAttribute("MerchantCustomerCode")]
        public string MerchantCustomerCode { get; set; }

        [XmlAttribute("Amount")]
        public decimal Amount { get; set; }

        [XmlElement("Purchases")]
        public BtiPurchases Purchases { get; set; }
    }

    public class BtiPurchases
    {
        [XmlElement("Purchase")]
        public List<BtiPurchase> PurchaseList { get; set; }
    }

    public class BtiPurchase
    {
        [XmlAttribute("ReserveID")]
        public string ReserveID { get; set; }

        [XmlAttribute("PurchaseID")]
        public string PurchaseID { get; set; }

        [XmlAttribute("Amount")]
        public decimal Amount { get; set; }

        [XmlAttribute("CreationDateUTC")]
        public DateTime CreationDateUTC { get; set; }

        [XmlAttribute("CurrentBalance")]
        public decimal CurrentBalance { get; set; }

        [XmlAttribute("seq_num")]
        public int SeqNum { get; set; }

        [XmlAttribute("CurrentStatus")]
        public string CurrentStatus { get; set; }

        [XmlAttribute("ExtBonusContribution")]
        public decimal ExtBonusContribution { get; set; }

        [XmlElement("Selections")]
        public BtiSelections Selections { get; set; }
    }

    public class BtiSelections
    {
        [XmlElement("Selection")]
        public List<BtiSelection> SelectionList { get; set; }
    }

    public class BtiSelection
    {
        [XmlAttribute("LineID")]
        public string LineID { get; set; }

        [XmlAttribute("DecimalOdds")]
        public decimal DecimalOdds { get; set; }

        [XmlAttribute("UserOddStyle")]
        public string UserOddStyle { get; set; }

        [XmlAttribute("OddsInUserStyle")]
        public int OddsInUserStyle { get; set; }

        [XmlAttribute("Points")]
        public decimal Points { get; set; }

        [XmlAttribute("BranchID")]
        public int BranchID { get; set; }

        [XmlAttribute("BranchName")]
        public string BranchName { get; set; }

        [XmlAttribute("LineTypeID")]
        public int LineTypeID { get; set; }

        [XmlAttribute("LineTypeName")]
        public string LineTypeName { get; set; }

        [XmlAttribute("LeagueID")]
        public int LeagueID { get; set; }

        [XmlAttribute("LeagueName")]
        public string LeagueName { get; set; }

        [XmlAttribute("HomeTeam")]
        public string HomeTeam { get; set; }

        [XmlAttribute("AwayTeam")]
        public string AwayTeam { get; set; }

        [XmlAttribute("Score")]
        public string Score { get; set; }

        [XmlAttribute("YourBet")]
        public string YourBet { get; set; }

        [XmlAttribute("EventTypeID")]
        public int EventTypeID { get; set; }

        [XmlAttribute("EventTypeName")]
        public string EventTypeName { get; set; }

        [XmlAttribute("EventDateUTC")]
        public DateTime EventDateUTC { get; set; }

        [XmlAttribute("MasterEventID")]
        public string MasterEventID { get; set; }

        [XmlAttribute("EventID")]
        public string EventID { get; set; }

        [XmlAttribute("EncodedID")]
        public string EncodedID { get; set; }

        [XmlAttribute("EventName")]
        public string EventName { get; set; }

        [XmlAttribute("EventState")]
        public string EventState { get; set; }

        [XmlAttribute("IsLive")]
        public int IsLive { get; set; }

        [XmlElement("Changes")]
        public BtiChanges Changes { get; set; }
    }

    public class BtiChanges
    {
        [XmlElement("Change")]
        public List<BtiChange> ChangeList { get; set; }
    }

    public class BtiChange
    {
        [XmlAttribute("ID")]
        public string ChangeID { get; set; }

        [XmlAttribute("Amount")]
        public decimal Amount { get; set; }

        [XmlAttribute("OldStatus")]
        public string OldStatus { get; set; }

        [XmlAttribute("NewStatus")]
        public string NewStatus { get; set; }

        [XmlAttribute("PrevBalance")]
        public decimal PrevBalance { get; set; }

        [XmlAttribute("NewBalance")]
        public decimal NewBalance { get; set; }

        [XmlAttribute("DateUTC")]
        public DateTime DateUTC { get; set; }

        [XmlElement("Bets")]
        public BtiBets Bets { get; set; }
    }

    [XmlRoot("Bets")]
    public class BtiBets
    {
        [XmlAttribute("cust_id")]
        public string CustomerId { get; set; }

        [XmlAttribute("reserve_id")]
        public string ReserveId { get; set; }

        [XmlAttribute("amount")]
        public decimal Amount { get; set; }

        [XmlAttribute("currency_code")]
        public string CurrencyCode { get; set; }

        [XmlAttribute("platform")]
        public string Platform { get; set; }

        [XmlElement("Bet")]
        public List<BtiBet> BetList { get; set; }
    }

    public class BtiBet
    {
        [XmlAttribute("BetID")]
        public string BetId { get; set; }

        [XmlAttribute("BetTypeID")]
        public int BetTypeId { get; set; }

        [XmlAttribute("BetTypeName")]
        public string BetTypeName { get; set; }

        [XmlAttribute("Stake")]
        public decimal Stake { get; set; }

        [XmlAttribute("OrgStake")]
        public decimal OrgStake { get; set; }

        [XmlAttribute("Gain")]
        public decimal Gain { get; set; }

        [XmlAttribute("IsLive")]
        public int IsLive { get; set; }

        [XmlAttribute("NumberOfBets")]
        public int NumberOfBets { get; set; }

        [XmlAttribute("Status")]
        public string Status { get; set; }

        [XmlAttribute("IsFreeBet")]
        public int IsFreeBet { get; set; }

        [XmlAttribute("FreebetAmount")]
        public decimal FreebetAmount { get; set; }

        [XmlAttribute("RealAmount")]
        public decimal RealAmount { get; set; }

        [XmlAttribute("CreationDate")]
        public DateTime CreationDate { get; set; }

        [XmlAttribute("PurchaseBetID")]
        public string PurchaseBetId { get; set; }

        [XmlAttribute("CommomStatusID")]
        public int CommonStatusId { get; set; }

        [XmlAttribute("Odds")]
        public int Odds { get; set; }

        [XmlAttribute("OddsDec")]
        public decimal OddsDec { get; set; }

        [XmlAttribute("BonusID")]
        public int BonusId { get; set; }

        [XmlAttribute("ComboBetNumersLines")]
        public int ComboBetNumersLines { get; set; }

        [XmlAttribute("ReferenceID")]
        public string ReferenceId { get; set; }

        [XmlAttribute("ReserveAmountType")]
        public string ReserveAmountType { get; set; }

        [XmlAttribute("ReserveAmountTypeID")]
        public int ReserveAmountTypeId { get; set; }

        [XmlAttribute("ContributionAmount")]
        public decimal ContributionAmount { get; set; }

        [XmlAttribute("ContributionType")]
        public int ContributionType { get; set; }

        [XmlElement("Lines")]
        public List<BtiLine> Lines { get; set; }
    }

    public class BtiLine
    {
        [XmlAttribute("LineID")]
        public string LineId { get; set; }

        [XmlAttribute("Stake")]
        public decimal Stake { get; set; }

        [XmlAttribute("OddsDec")]
        public decimal OddsDec { get; set; }

        [XmlAttribute("Gain")]
        public decimal Gain { get; set; }

        [XmlAttribute("LiveScore1")]
        public int LiveScore1 { get; set; }

        [XmlAttribute("LiveScore2")]
        public int LiveScore2 { get; set; }

        [XmlAttribute("HomeTeam")]
        public string HomeTeam { get; set; }

        [XmlAttribute("AwayTeam")]
        public string AwayTeam { get; set; }

        [XmlAttribute("TransEventName")]
        public string TransEventName { get; set; }

        [XmlAttribute("TransEventTypeName")]
        public string TransEventTypeName { get; set; }

        [XmlAttribute("TransLeagueName")]
        public string TransLeagueName { get; set; }

        [XmlAttribute("TransYourBet")]
        public string TransYourBet { get; set; }

        [XmlAttribute("TransHomeTeam")]
        public string TransHomeTeam { get; set; }

        [XmlAttribute("TransAwayTeam")]
        public string TransAwayTeam { get; set; }

        [XmlAttribute("TransBranchName")]
        public string TransBranchName { get; set; }

        [XmlAttribute("Status")]
        public string Status { get; set; }

        [XmlAttribute("EventState")]
        public string EventState { get; set; }

        [XmlAttribute("CustomerID")]
        public string CustomerId { get; set; }

        [XmlAttribute("BetID")]
        public string BetId { get; set; }

        [XmlAttribute("BetTypeName")]
        public string BetTypeName { get; set; }

        [XmlAttribute("LineTypeID")]
        public int LineTypeId { get; set; }

        [XmlAttribute("LineTypeName")]
        public string LineTypeName { get; set; }

        [XmlAttribute("RowTypeID")]
        public int RowTypeId { get; set; }

        [XmlAttribute("BranchID")]
        public int BranchId { get; set; }

        [XmlAttribute("BranchName")]
        public string BranchName { get; set; }

        [XmlAttribute("LeagueID")]
        public string LeagueId { get; set; }

        [XmlAttribute("LeagueName")]
        public string LeagueName { get; set; }

        [XmlAttribute("MasterLeagueID")]
        public string MasterLeagueId { get; set; }

        [XmlAttribute("CreationDate")]
        public DateTime CreationDate { get; set; }

        [XmlAttribute("YourBet")]
        public string YourBet { get; set; }

        [XmlAttribute("EventTypeID")]
        public int EventTypeId { get; set; }

        [XmlAttribute("EventTypeName")]
        public string EventTypeName { get; set; }

        [XmlAttribute("EventDate")]
        public DateTime EventDate { get; set; }

        [XmlAttribute("MasterEventID")]
        public string MasterEventId { get; set; }

        [XmlAttribute("EventID")]
        public string EventId { get; set; }

        [XmlAttribute("NewMasterEventID")]
        public string NewMasterEventId { get; set; }

        [XmlAttribute("NewEventID")]
        public string NewEventId { get; set; }

        [XmlAttribute("NewLeagueID")]
        public string NewLeagueId { get; set; }

        [XmlAttribute("NewLineID")]
        public string NewLineId { get; set; }

        [XmlAttribute("EventName")]
        public string EventName { get; set; }

        [XmlAttribute("TeamMappingID")]
        public int TeamMappingId { get; set; }

        [XmlAttribute("Odds")]
        public int Odds { get; set; }

        [XmlAttribute("Score")]
        public string Score { get; set; }

        [XmlAttribute("IsLive")]
        public int IsLive { get; set; }

        [XmlAttribute("EachWaySetting")]
        public string EachWaySetting { get; set; }
    }

}
