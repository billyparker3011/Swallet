namespace HnMicro.Framework.Exceptions
{
    public class BadRequestException : Exception
    {
        public int OverrideErrCode { get; set; }
        public string OverrideErrMsg { get; set; }

        public BadRequestException() : base()
        {
        }

        public BadRequestException(string message) : base(message)
        {
        }

        public BadRequestException(string message, params object[] args) : base(string.Format(message, args))
        {
        }

        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public BadRequestException(int overrideErrCode) : base()
        {
            OverrideErrCode = overrideErrCode;
        }

        public BadRequestException(int overrideErrCode, string overrideErrMsg) : base()
        {
            OverrideErrCode = overrideErrCode;
            OverrideErrMsg = overrideErrMsg;
        }
    }
}
