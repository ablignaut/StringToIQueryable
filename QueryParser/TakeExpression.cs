using System;
using System.Linq;

namespace QueryParser
{
    public class TakeExpression<T> : IParserExpression<T>
    {
        private const int TakeAll = -1;

        public TakeExpression(int takeNumber)
        {
            if (takeNumber < TakeAll)
                throw new InvalidOperationException("take cannot be negative");
            Take = takeNumber;
        }

        public int Take { get; private set; }

        public static TakeExpression<T> All()
        {
            return new TakeExpression<T>(TakeAll);
        }

        public IQueryable<T> Map(IQueryable<T> queriable)
        {
            if (Take == TakeAll)
                return queriable;
            return queriable.Take(Take);
        }

        public override string ToString()
        {
            return ParserConstants.TakeIndicator +
                   (Take == TakeAll ? ParserConstants.TakeAllIndicator : Take.ToString());
        }
    }
}
