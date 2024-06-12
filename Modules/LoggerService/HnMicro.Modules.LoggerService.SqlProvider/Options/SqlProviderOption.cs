using HnMicro.Framework.Data.Models;

namespace HnMicro.Modules.LoggerService.SqlProvider.Options
{
    public class SqlProviderOption
    {
        public const string AppSettingName = "SqlLogging";

        public bool Enabled { get; set; }
        public SqlConnectionOption ConnectionStrings { get; set; }
    }
}
