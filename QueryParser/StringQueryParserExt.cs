using System.Linq;

namespace QueryParser
{
    public static class StringQueryParserExt
    {
        public static IQueryable<T> Parse<T>(this IQueryable<T> queriable, string expressions)
        {
            var parser = new StringQueryParser<T>(expressions);
            return parser.Map(queriable);
        }
    }
}
