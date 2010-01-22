using System;
using System.Linq;

namespace QueryParser
{
    public class SkipExpression<T> : IParserExpression<T>
    {
        public SkipExpression(int skipNumber)
        {
            if (skipNumber < 0)
                throw new InvalidOperationException("skip number cannot be negative");
            SkipNumber = skipNumber;
        }

        public int SkipNumber { get; private set; }

        public IQueryable<T> Map(IQueryable<T> queriable)
        {
            return queriable.Skip(SkipNumber);
        }

        public override string ToString()
        {
            return ParserConstants.SkipIndicator + SkipNumber;
        }
    }
}
