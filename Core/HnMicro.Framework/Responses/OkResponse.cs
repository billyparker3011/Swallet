namespace HnMicro.Framework.Responses
{
    public class OkResponse
    {
        public static ApiResponse<T> Create<T>(T data)
        {
            return new ApiResponse<T> { Data = data };
        }

        public static ApiResponse<T> Create<T>(T data, ApiResponseMetadata metadata)
        {
            return new ApiResponse<T> { Data = data, Metadata = metadata };
        }
    }
}
