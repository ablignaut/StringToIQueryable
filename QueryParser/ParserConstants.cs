
namespace QueryParser
{
    public static class ParserConstants
    {
        public const char ExpressionSeparator = '/';
        public const char InnerExpressionSeparator = '-';
        public const char ListDeliminator = ',';
        public const string SortIndicator = "sort-";
        public const string SkipIndicator = "skip-";
        public const string TakeIndicator = "take-";
        public const string DescendingIndicator = "desc-";
        public const string TakeAllIndicator = "all";
        public const int DefaultPageSize = 10;
        public const string PageIndicator = "page-";
    }
}
