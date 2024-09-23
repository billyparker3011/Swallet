using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("Customers")]
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(UsernameUpper), IsUnique = true)]
    [Index(nameof(Email))]
    public class Customer : BaseEntity
    {
        [Key]
        public long CustomerId { get; set; }

        [Required]
        [MaxLength(300)]
        public string Username { get; set; }

        [Required]
        [MaxLength(300)]
        public string UsernameUpper { get; set; }

        [Required]
        [MaxLength(300)]
        public string Password { get; set; }

        [Required]
        [MaxLength(300)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(300)]
        public string LastName { get; set; }

        [MaxLength(300)]
        public string Email { get; set; }

        [MaxLength(500)]
        public string Telegram { get; set; }

        [Required]
        public int State { get; set; }

        [MaxLength(300)]
        public string Phone { get; set; }

        [Required]
        public int RoleId { get; set; }

        public virtual CustomerSession CustomerSession { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }

        [Required]
        public bool IsAffiliate { get; set; }

        [Required, DefaultValue(0L)]
        public long AgentId { get; set; }

        [Required, DefaultValue(0L)]
        public long MasterId { get; set; }

        [Required, DefaultValue(0L)]
        public long SupermasterId { get; set; }

        public DateTime? ChangedPasswordAt { get; set; }

        [Required, DefaultValue(false)]
        public bool Lock { get; set; }

        [Required, DefaultValue(true)]
        public bool DepositAllowed { get; set; }

        [Required, DefaultValue(true)]
        public bool WithdrawAllowed { get; set; }

        [Required, DefaultValue(true)]
        public bool DiscountAllowed { get; set; }
    }
}
