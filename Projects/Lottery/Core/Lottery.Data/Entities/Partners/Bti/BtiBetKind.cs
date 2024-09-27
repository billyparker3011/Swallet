using HnMicro.Framework.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Data.Entities.Partners.Bti
{
    [Table("BtiBetKinds")]
    [Index(nameof(BranchId), IsUnique = true)]
    public class BtiBetKind
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Name { get; set; }

        [Required]
        public bool Enabled { get; set; }

        public long? BranchId { get; set; }
    }
}
