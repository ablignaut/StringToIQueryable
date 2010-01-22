using System;
using System.Linq;
using QueryParser;
using Xunit;
using Xunit.Extensions;

namespace QueryParserTests
{
    public class TakeExpressionTests
    {
        [Theory, InlineData(1), InlineData(6), InlineData(30), InlineData(54)]
        public void TakeExpression_SpecifiedTakeNumber_MapReturnsQueriableWithCountEqualToTake(int take)
        {
            var init = Enumerable.Range(0, 100).AsQueryable();

            var takeExpr = new TakeExpression<int>(take);

            Assert.Equal(take, takeExpr.Map(init).Count());
        }

        [Fact]
        public void TakeExpression_WithTakeAll_MapReturnsQueriableWithAll()
        {
            var init = Enumerable.Range(0, 100).AsQueryable();
            var end = init.Count();

            var takeExpr = TakeExpression<int>.All();

            Assert.Equal(end, takeExpr.Map(init).Count());
        }

        [Fact]
        public void TakeExpression_ToString_StartsWithTakeIndicator()
        {
            var takeExpr = new TakeExpression<int>(3);
            Assert.True(takeExpr.ToString().StartsWith(ParserConstants.TakeIndicator));
        }

        [Fact]
        public void TakeExpression_ToString_EndsWithTakeValue()
        {
            int takeVal = 29;
            var takeExpr = new TakeExpression<int>(takeVal);
            Assert.True(takeVal.ToString().EndsWith(takeVal.ToString()));
        }

        [Fact]
        public void TakeExpression_WithTakeAll_EndsWithAll()
        {
            var takeExpr = TakeExpression<int>.All();
            Assert.True(takeExpr.ToString().EndsWith(ParserConstants.TakeAllIndicator));
        }

        [Fact]
        public void TakeExpression_Constructor_ThrowException_WhenNegativeTakeValue()
        {
            Assert.Throws<InvalidOperationException>(() => new TakeExpression<TestClass>(-2));
        }
    }
}
