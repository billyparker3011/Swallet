namespace HnMicro.Framework.Responses
{
    public class ApiResponseMetadata
    {
        public long NoOfRows { get; set; }
        public int NoOfRowsPerPage { get; set; }
        public int Page { get; set; }
        public long NoOfPages { get; set; }
    }
}
