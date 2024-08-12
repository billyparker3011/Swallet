using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("BookieSettings")]
    public class BookieSetting : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int BookieTypeId { get; set; }
        public BookieSettingValue SettingValue { get; set; }
    }

    public class BookieSettingValue
    {
        public string ApiAddress { get; set; }
        public string PrivateKey { get; set; }
        public string PartnerAccountId { get; set; }
        public string GameClientId { get; set; }
        public string AuthValue { get; set; }
        public string ApplicationStaticToken { get; set; }
    }

    public class CABookieSettingValue
    {
        public string ApiURL { get; set; }
        public string ContentType { get; set; }
        public string OperatorId { get; set; }
        public string AllbetApiKey { get; set; }
        public string PartnerApiKey { get; set; }
    }

    public class BookieSettingConfiguration : IEntityTypeConfiguration<BookieSetting>
    {
        public void Configure(EntityTypeBuilder<BookieSetting> builder)
        {
            builder.Property(e => e.SettingValue).HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<BookieSettingValue>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}
