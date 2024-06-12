namespace HnMicro.Framework.Data.Models
{
    public class SqlConnectionOption
    {
        public const string AppSettingName = "ConnectionStrings:DefaultConnection";

        public string Connection { get; set; }
        public bool UsePool { get; set; }
    }
}
