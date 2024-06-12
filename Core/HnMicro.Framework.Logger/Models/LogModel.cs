using Microsoft.Extensions.Logging;

namespace HnMicro.Framework.Logger.Models
{
    public sealed class LogModel
    {
        public LogLevel LogLevel { get; set; }
        public string CategoryName { get; set; }
        public string Message { get; set; }
        public string Stacktrace { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatedBy { get; set; }
        public int RoleId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string Version { get; set; }
    }
}
