using HnMicro.Framework.Enums;

namespace HnMicro.Framework.Models
{
    public class QueryAdvance
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string SortName { get; set; }
        public SortType SortType { get; set; } = SortType.Descending;
    }
}
