using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryParser
{
    public class StringQueryGenerator<T>
    {
        private IList<IParserExpression<T>> _expressions;
        public StringQueryGenerator(IEnumerable<IParserExpression<T>> parserExpressions)
        {
            _expressions = parserExpressions == null ? null : parserExpressions.ToList();
        }

        public string Generate()
        {
            if (_expressions == null || _expressions.Count == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();

            foreach (var expression in _expressions)
                builder.Append(ParserConstants.ExpressionSeparator)
                       .Append(expression.ToString());

            builder.Append(ParserConstants.ExpressionSeparator);

            return builder.ToString().ToLower();
        }
    }
}
