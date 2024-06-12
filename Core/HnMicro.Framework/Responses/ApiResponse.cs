using HnMicro.Core.Helpers;

namespace HnMicro.Framework.Responses
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public ApiResponseMessage Message { get; set; }
        public ApiResponseMetadata Metadata { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, StringHelper.CamelCaseJsonSetting);
        }
    }
}
