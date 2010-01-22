using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryParser
{
    public class WhereExpression<T> : IParserExpression<T>
    {
        public WhereExpression(string propertyName, WhereCombinator combinator, object value)
        {
            PropertyName = propertyName;
            Combinator = combinator;
            Value = "null".Equals(value) ? null : value;
        }

        public WhereExpression(Expression<Func<T, object>> property, WhereCombinator combinator, object value)
            : this(property.ToProperty(), combinator, value)
        { }

        public string PropertyName { get; private set; }
        public WhereCombinator Combinator { get; private set; }
        public object Value { get; private set; }

        public IQueryable<T> Map(IQueryable<T> queriable)
        {
            return queriable.Where(ComposeExpression());
        }

        private Expression<Func<T, bool>> ComposeExpression()
        {
            var property = PropertyNameResolver.FindProperty<T>(PropertyName);

            // ignore non-properties
            if (property == null || !ValidCombinator(property))
                return x => true;

            Func<Expression, Expression, Expression> combinator = Expression.Equal;
            
            switch (Combinator)
            {
                case WhereCombinator.equals:
                    break;
                case WhereCombinator.like:
                    combinator = (left, right) => Expression.Call(
                                                    Expression.Call(left, "ToString", new Type[0]),
                                                    typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                                                    Expression.Call(right, "ToString", new Type[0]));
                    break;
                case WhereCombinator.greaterthan:
                    combinator = Expression.GreaterThan;
                    break;
                case WhereCombinator.greaterthanequal:
                    combinator = Expression.GreaterThanOrEqual;
                    break;
                case WhereCombinator.lessthan:
                    combinator = Expression.LessThan;
                    break;
                case WhereCombinator.lessthanequal:
                    combinator = Expression.LessThanOrEqual;
                    break;
                case WhereCombinator.not:
                    combinator = Expression.NotEqual;
                    break;
            }
            var param = Expression.Parameter(typeof(T), "value");
            var constant = Expression.Constant(ChangeType(Value, property.PropertyType), property.PropertyType);
            return Expression.Lambda<Func<T, bool>>(combinator(Expression.Property(param, property), constant), param);
        }

        private static object ChangeType(object value, Type conversionType)
        {
            if (conversionType == null)
                throw new ArgumentNullException("conversionType");

            if (IsNullableType(conversionType))
            {
                if (value == null)
                    return null;
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            return Convert.ChangeType(value, conversionType);
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private bool ValidCombinator(PropertyInfo property)
        {
            if(Combinator.Equals(WhereCombinator.equals | WhereCombinator.like | WhereCombinator.not))
                return true;

            Type type = property.PropertyType;
            type = IsNullableType(type) ? type.GetGenericArguments()[0] : type;
            
            if (type.IsEnum) 
                return true;

            switch (Type.GetTypeCode(type)) {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return PropertyName + ParserConstants.InnerExpressionSeparator +
                   Combinator.ToString() + ParserConstants.InnerExpressionSeparator +
                   (Value == null ? "null" : Value.ToString());
                                
        }
    }

    public enum WhereCombinator
    {
        equals,
        like,
        greaterthan,
        greaterthanequal,
        lessthan,
        lessthanequal,
        not
    }
}
