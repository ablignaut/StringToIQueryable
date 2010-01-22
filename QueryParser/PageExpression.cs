using System;
using System.Linq;

namespace QueryParser
{
    public class PageExpression<T> : IParserExpression<T>
    {
        public PageExpression(int pageNumber, int pageSize)
        {
            if (pageSize <= 0)
                throw new InvalidOperationException("Page Size cannot be less than 1");
            if (pageNumber <= 0)
                throw new InvalidOperationException("Page Number cannot be less than 1");

            Skip = (pageNumber - 1) * pageSize;
            Take = pageSize;
        }

        public int Skip { get; private set; }

        public int Take { get; private set; }

        public IQueryable<T> Map(IQueryable<T> queriable)
        {
            return queriable.Skip(Skip).Take(Take);
        }

        public override string ToString()
        {
            var returnString = ParserConstants.PageIndicator + ((Skip / Take) + 1);

            if (Take != ParserConstants.DefaultPageSize)
                returnString = returnString + ParserConstants.InnerExpressionSeparator + Take;

            return returnString;
        }
    }
}
