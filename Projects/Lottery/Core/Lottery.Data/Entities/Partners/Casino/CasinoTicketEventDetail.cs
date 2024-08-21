using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Data.Entities.Partners.Casino
{
    [Table("CasinoTicketEventDetail")]
    [Index(nameof(CasinoTicketId))]
    public class CasinoTicketEventDetail : DefaultBaseEntity<long>
    {
        [Required]
        public int EventType { get; set; }
        [Required, MaxLength(20)]
        public string EventCode { get; set; }
        [Required]
        public long EventRecordNum { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public long ExchangeRate { get; set; }
        [Required, MaxLength(50)]
        public string SettleTime { get; set; }

        [Required]
        public long CasinoTicketId { get; set; }

        [ForeignKey(nameof(CasinoTicketId))]
        public virtual CasinoTicket CasinoTicket { get; set; }
    }
}
