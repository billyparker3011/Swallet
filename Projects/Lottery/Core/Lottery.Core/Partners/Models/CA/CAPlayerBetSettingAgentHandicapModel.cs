using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Models.CA
{
    public class CAPlayerBetSettingAgentHandicapModel
    {
        public long CAPlayerBetSettingId { get; set; }

        public long CAAgentHandicapId { get; set; }
    }
}
