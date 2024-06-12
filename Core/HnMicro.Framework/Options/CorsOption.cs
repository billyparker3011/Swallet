namespace HnMicro.Framework.Options
{
    public class CorsOption
    {
        public const string AppSettingName = "Cors";

        public string Name { get; set; }
        public List<string> Urls { get; set; }
    }
}
