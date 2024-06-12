using System.ComponentModel.DataAnnotations;

namespace HnMicro.Framework.Data.Entities;

public class AuditEntity
{
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
}
