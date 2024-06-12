using System.ComponentModel.DataAnnotations;

namespace HnMicro.Framework.Data.Entities;

public class DefaultBaseEntity<T> : AuditEntity
{
    [Key]
    public T Id { get; set; }
}
