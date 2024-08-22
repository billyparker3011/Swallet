﻿using Lottery.Core.Partners.Helpers;
using Newtonsoft.Json;

namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoTicketModel
    {
        public CasinoTicketModel() 
        {
            //SetDetails();
        }
        public long TranId { get; set; }
        public string Player { get; set; }
        public Decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Reason { get; set; }
        public int Type { get; set; }
        public bool IsRetry { get; set; }
        public object Details { get; set; }

        private IEnumerable<CasinoTicketBetDetailModel> _casinoTicketBetDetailModels;

        private IEnumerable<CasinoTicketEventDetailModel> _casinoTicketEventDetailModels;

        public IEnumerable<CasinoTicketBetDetailModel> CasinoTicketBetDetailModels
        {
            get
            {
                if (_casinoTicketBetDetailModels == null && CasinoHelper.TypeTransfer.BetDetails.Contains(Type) && Details != null)
                {
                    _casinoTicketBetDetailModels = JsonConvert.DeserializeObject<IEnumerable<CasinoTicketBetDetailModel>>(Details.ToString());
                }
                return _casinoTicketBetDetailModels;
            }
            set
            {
                _casinoTicketBetDetailModels = value;
            }
        }

        public IEnumerable<CasinoTicketEventDetailModel> CasinoTicketEventDetailModels
        {
            get
            {
                if (_casinoTicketEventDetailModels == null && CasinoHelper.TypeTransfer.EventDetails.Contains(Type) && Details != null)
                {
                    _casinoTicketEventDetailModels = JsonConvert.DeserializeObject<IEnumerable<CasinoTicketEventDetailModel>>(Details.ToString());
                }
                return _casinoTicketEventDetailModels;
            }
            set
            {
                _casinoTicketEventDetailModels = value;
            }
        }


        public void SetDetails()
        {
            if(Details == null) return;
            if (CasinoHelper.TypeTransfer.BetDetails.Contains(Type) && Details != null) CasinoTicketBetDetailModels = JsonConvert.DeserializeObject<IEnumerable<CasinoTicketBetDetailModel>>(Details.ToString());
            if (CasinoHelper.TypeTransfer.EventDetails.Contains(Type) && Details != null) CasinoTicketEventDetailModels = JsonConvert.DeserializeObject<IEnumerable<CasinoTicketEventDetailModel>>(Details.ToString());
        }

        public CasinoTicketModel GetCasinoTicketModel(string json)
        {
            if (string.IsNullOrWhiteSpace(json) || Details == null) return null;
            var model = JsonConvert.DeserializeObject<CasinoTicketModel>(json);
            if (CasinoHelper.TypeTransfer.BetDetails.Contains(Type)) model.CasinoTicketBetDetailModels = JsonConvert.DeserializeObject<IEnumerable<CasinoTicketBetDetailModel>>(model.Details.ToString());
            if (CasinoHelper.TypeTransfer.EventDetails.Contains(Type)) model.CasinoTicketEventDetailModels = JsonConvert.DeserializeObject<IEnumerable<CasinoTicketEventDetailModel>>(model.Details.ToString());
            return model;
        }
    }

    public class CasinoCancelTicketModel
    {
        public long TranId { get; set; }
        public string Player { get; set; }
        public string Reason { get; set; }
        public bool IsRetry { get; set; }
        public long OriginalTranId { get; set; }
        public IEnumerable<CasinoTicketBetDetailModel> OriginalDetails { get; set; }

        public CasinoCancelTicketModel GetCancelCasinoTicketModel(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            return JsonConvert.DeserializeObject<CasinoCancelTicketModel>(json);
        }
    }

    public class CasinoTicketBetDetailModel
    {
        public long BetNum { get; set; }
        public long GameRoundId { get; set; }
        public int Status { get; set; }
        public decimal BetAmount { get; set; }
        public decimal Deposit { get; set; }
        public int GameType { get; set; }
        public int BetType { get; set; }
        public int Commission { get; set; }
        public decimal ExchangeRate { get; set; }
        public string GameResult { get; set; }
        public string GameResult2 { get; set; }
        public long? WinOrLossAmount { get; set; }
        public long? ValidAmount { get; set; }
        public string BetTime { get; set; }
        public string TableName { get; set; }
        public long BetMethod { get; set; }
        public long AppType { get; set; }
        public string GameRoundStartTime { get; set; }
        public string GameRoundEndTime { get; set; }
        public string Ip { get; set; }
    }

    public class CasinoTicketEventDetailModel
    {
        public int EventType { get; set; }
        public string EventCode { get; set; }
        public long EventRecordNum { get; set; }
        public decimal Amount { get; set; }
        public long ExchangeRate { get; set; }
        public string SettleTime { get; set; }

    }
}