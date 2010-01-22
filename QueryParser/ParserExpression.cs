using System.Linq;

namespace QueryParser
{
    public interface IParserExpression<T>
    {
        IQueryable<T> Map(IQueryable<T> queriable);
    }
}
