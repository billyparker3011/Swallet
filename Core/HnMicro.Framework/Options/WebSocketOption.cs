namespace HnMicro.Framework.Options
{
    public class WebSocketOption
    {
        public const string AppSettingName = "Socket";

        public bool EnableDetailedErrors { get; set; }
        public int KeepAliveIntervalInSeconds { get; set; }
        public int ClientTimeoutIntervalInSeconds { get; set; }
    }
}
