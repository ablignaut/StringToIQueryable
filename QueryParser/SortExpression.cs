using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QueryParser
{
    public class SortExpression<T> : IParserExpression<T>
    {
        public SortExpression(string[] columns, bool desc)
        {
            if (columns == null || columns.Length == 0)
                throw new InvalidOperationException("Must specify columns for sort");

            Descending = desc;
            Columns = columns;
        }

        public SortExpression(string[] columns)
            :this(columns, false)
        { }

        public SortExpression(params Expression<Func<T, object>>[] props)
            : this(false, props)
        { }

        public SortExpression(bool desc, params Expression<Func<T, object>>[] props)
        {
            if (props == null || props.Length == 0)
                throw new InvalidOperationException("Must specify columns for sort");

            Columns = props.Select(x => x.ToProperty()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            Descending = desc;
        }

        public bool Descending { get; private set; }
        public string[] Columns { get; private set; }

        public IQueryable<T> Map(IQueryable<T> queriable)
        {
            var properties = from column in Columns
                             let prop = PropertyNameResolver.FindProperty<T>(column)
                             where prop != null
                             select prop;

            string method = Descending ? "OrderByDescending" : "OrderBy";
            Expression expr = queriable.Expression;

            foreach(var property in properties)
            {
                var param = Expression.Parameter(typeof(T), "val");
                expr = Expression.Call(
                            typeof(Queryable),
                            method,
                            new Type[] { queriable.ElementType, property.PropertyType },
                            expr,
                            Expression.Quote(Expression.Lambda(Expression.Property(param, property), param)));

                method = Descending ? "ThenByDescending" : "ThenBy";
            }

            return queriable.Provider.CreateQuery<T>(expr);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(ParserConstants.SortIndicator);

            if (Descending)
                builder.Append(ParserConstants.DescendingIndicator);

            builder.Append(Columns[0]);

            for (int i = 1; i < Columns.Length; i++)
                builder.Append(ParserConstants.ListDeliminator)
                       .Append(Columns[i]);
            return builder.ToString();            
        }
    }
}