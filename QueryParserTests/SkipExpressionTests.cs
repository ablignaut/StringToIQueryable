using System;
using System.Linq;
using QueryParser;
using Xunit;
using Xunit.Extensions;

namespace QueryParserTests
{
    public class SkipExpressionTests
    {
        [Theory, InlineData(0), InlineData(12), InlineData(34), InlineData(77), InlineData(100)]
        public void SkipExpression_WithSpecifiedSkipNumber_ReturnsCorrectEnumerableWithOffsetCount(int skip)
        {
            var query = Enumerable.Range(0, 100).AsQueryable();
            int range = query.Count() - skip;

            var skipExpr = new SkipExpression<int>(skip);

            Assert.Equal(range, skipExpr.Map(query).Count());
        }

        [Fact]
        public void SkipExpression_Constructor_ThrowsException_WhenNegativeSkipValue()
        {
            Assert.Throws<InvalidOperationException>(() => new SkipExpression<TestClass>(-1));
        }

        [Fact]
        public void SkipExpression_ToString_ContainsSkipValue()
        { 
            int skipValue = 2003;
            var skipExpr = new SkipExpression<TestClass>(skipValue);
            Assert.Contains(skipValue.ToString(), skipExpr.ToString());
        }

        [Fact]
        public void SkipExpression_ToString_StartsWithSkipPrefix()
        {
            var skipExpr = new SkipExpression<TestClass>(5);
            Assert.True(skipExpr.ToString().StartsWith(ParserConstants.SkipIndicator));
        }
    }
}
