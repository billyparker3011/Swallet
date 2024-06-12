using System.ComponentModel.DataAnnotations;

namespace HnMicro.Framework.Data.Entities;

public class DefaultBaseEntityNoneAudit<T>
{
    [Key]
    public T Id { get; set; }
}