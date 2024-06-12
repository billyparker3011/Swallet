using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HnMicro.Modules.LoggerService.SqlProvider.Entities
{
    [Table("Logs")]
    [Index(nameof(RoleId))]
    [Index(nameof(ServiceCode))]
    [Index(nameof(ServiceName))]
    [Index(nameof(CreatedBy))]
    public class LogEntry : DefaultBaseEntityNoneAudit<long>
    {
        [MaxLength(400)]
        public string CategoryName { get; set; }
        [Required, MaxLength(250)]
        public string ServiceCode { get; set; }
        [Required, MaxLength(400)]
        public string ServiceName { get; set; }
        [Required, MaxLength(50)]
        public string Version { get; set; }
        [Required, MaxLength(2000)]
        public string Message { get; set; }
        [MaxLength(4000)]
        public string Stacktrace { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatedBy { get; set; }
    }
}
