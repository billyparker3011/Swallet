namespace HnMicro.Framework.Logger.Models
{
    public class LogRequestModel
    {
        public string CategoryName { get; set; }
        public string Message { get; set; }
        public string Stacktrace { get; set; }
        public int RoleId { get; set; }
        public long CreatedBy { get; set; }
    }
}
