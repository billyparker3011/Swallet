using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Models.CA
{
    public class CABookieSettingModel
    {
        public string ApiURL { get; set; }
        public string ContentType { get; set; }
        public string OperatorId { get; set; }
        public string AllbetApiKey { get; set; }
        public string PartnerApiKey { get; set; }
    }
}
