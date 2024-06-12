namespace HnMicro.Framework.Data.Repositories
{
    public class PagingMetadata
    {
        public long NoOfRows { get; set; }
        public int NoOfRowsPerPage { get; set; }
        public int Page { get; set; }
        public long NoOfPages
        {
            get
            {
                var noOfPages = NoOfRows / NoOfRowsPerPage;
                if (noOfPages * NoOfRowsPerPage < NoOfRows)
                {
                    return noOfPages;
                }

                return noOfPages;
            }
        }
    }
}
