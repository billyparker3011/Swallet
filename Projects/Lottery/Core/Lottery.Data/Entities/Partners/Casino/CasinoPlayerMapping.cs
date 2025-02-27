﻿using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.Casino
{
    [Table("CasinoPlayerMappings")]
    [Index(nameof(BookiePlayerId), IsUnique = true)]
    public class CasinoPlayerMapping : DefaultBaseEntity<long>
    {
        [Required]
        public long PlayerId { get; set; }
        [Required, MaxLength(50)]
        public string BookiePlayerId { get; set; }
        [MaxLength(500)]
        public string NickName { get; set; }
        [Required, DefaultValue(true)]
        public bool IsAccountEnable { get; set; }
        [Required, DefaultValue(true)]
        public bool IsAlowedToBet { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public virtual Player Player { get; set; }
    }
}
