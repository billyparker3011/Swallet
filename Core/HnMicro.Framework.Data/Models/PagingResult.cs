namespace HnMicro.Framework.Data.Repositories
{
    public class PagingResult<T>
    {
        public required IEnumerable<T> Items { get; set; }
        public required PagingMetadata Metadata { get; set; }
    }
}
