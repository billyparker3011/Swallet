namespace HnMicro.Framework.Responses
{
    public class ApiResponseMessage
    {
        public ApiResponseMessage(int errCode, string errMsg, string errStacktrace)
        {
            ErrCode = errCode;
            ErrMsg = errMsg;
            ErrStacktrace = errStacktrace;
        }

        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public string ErrStacktrace { get; set; }
    }
}
