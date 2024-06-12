namespace HnMicro.Core.Helpers
{
    public static class PagingHelper
    {
        public const int MinRowPerPage = 10;
        public const int MaxRowPerPage = 500;
        public const int DefaultPageSize = 100;

        public static List<int> RowPerPage = new List<int> { MinRowPerPage, 15, 20, 30, 50, 100, 200, MaxRowPerPage };

        public static int NormalizedPage(this int page)
        {
            return page < 0 ? 0 : page;
        }

        public static int NormalizedRowPerPage(this int rowPerPage)
        {
            return rowPerPage <= 0
                        ? MinRowPerPage
                        : rowPerPage > MaxRowPerPage
                            ? MaxRowPerPage
                            : RowPerPage.Contains(rowPerPage)
                                ? rowPerPage
                                : MinRowPerPage;
        }
    }
}
