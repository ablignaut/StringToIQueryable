using System.Linq;
using QueryParser;
using Xunit;
using Xunit.Extensions;

namespace QueryParserTests
{
    public class WhereExpressionTests
    {
        [Theory, InlineData(1), InlineData(44), InlineData(65), InlineData(76)]
        public void WhereExpression_GivenValidIntPropertyAndEqualsCombinatorWithSingleValue_ReturnsWhereWithOneElement(int testNum)
        {
            var data = QueryTestHelper.FormTestData((inst, index) => inst.IntProp = index, 100);
            var whereExpr = new WhereExpression<TestClass>("IntProp", WhereCombinator.equals, testNum);
            Assert.Equal(1, whereExpr.Map(data).Count());
        }

        [Theory, InlineData(1), InlineData(44), InlineData(65), InlineData(76)]
        public void WhereExpression_GivenValidIntPropertyAndGreaterThanCombinatorWithSingleValue_ReturnsWhereWithCorrectNumElements(int greaterthannum)
        {
            var data = QueryTestHelper.FormTestData((inst, index) => inst.IntProp = index, 100);
            int expectedCount = data.Count() - greaterthannum - 1;
            var whereExpr = new WhereExpression<TestClass>("IntProp", WhereCombinator.greaterthan, greaterthannum);
            Assert.Equal(expectedCount, whereExpr.Map(data).Count());
        }

        [Theory, InlineData(1), InlineData(44), InlineData(65), InlineData(76)]
        public void WhereExpression_GivenValidIntPropertyAndGreaterThanEqualCombinatorWithSingleValue_ReturnsWhereWithCorrectNumElements(int greaterthannum)
        {
            var data = QueryTestHelper.FormTestData((inst, index) => inst.IntProp = index, 100);
            int expectedCount = data.Count() - greaterthannum;
            var whereExpr = new WhereExpression<TestClass>("IntProp", WhereCombinator.greaterthanequal, greaterthannum);
            Assert.Equal(expectedCount, whereExpr.Map(data).Count());
        }

        [Theory, InlineData(1), InlineData(44), InlineData(65), InlineData(76)]
        public void WhereExpression_GivenValidIntPropertyAndLessThanCombinatorWithSingleValue_ReturnsWhereWithCorrectNumElement(int lessthannum)
        {
            var data = QueryTestHelper.FormTestData((inst, index) => inst.IntProp = index, 100);
            int expectedCount = 100 - (data.Count() - lessthannum);
            var whereExpr = new WhereExpression<TestClass>("IntProp", WhereCombinator.lessthan, lessthannum);
            Assert.Equal(expectedCount, whereExpr.Map(data).Count());
        }

        [Theory, InlineData(1), InlineData(44), InlineData(65), InlineData(76)]
        public void WhereExpression_GivenValidIntPropertyAndLessThanEqualCombinatorWithSingleValue_ReturnsWhereWithCorrectNumElement(int lessthannum)
        {
            var data = QueryTestHelper.FormTestData((inst, index) => inst.IntProp = index, 100);
            int expectedCount = 100 - (data.Count() - lessthannum - 1);
            var whereExpr = new WhereExpression<TestClass>("IntProp", WhereCombinator.lessthanequal, lessthannum);
            Assert.Equal(expectedCount, whereExpr.Map(data).Count());
        }

        [Theory, InlineData(1), InlineData(44), InlineData(65), InlineData(76)]
        public void WhereExpression_GivenValidIntPropertyAndNotCombinatorWithSingleValue_ReturnsWhereWithCorrectNumElement(int notvalue)
        {
            var data = QueryTestHelper.FormTestData((inst, index) => inst.IntProp = index, 100);
            int expectedCount = data.Count() - 1;
            var whereExpr = new WhereExpression<TestClass>("IntProp", WhereCombinator.not, notvalue);
            Assert.Equal(expectedCount, whereExpr.Map(data).Count());
        }

        [Fact]
        public void WhereExpression_GivenValidStringPropertyAndInvalidCombinator_ReturnsSameQueryCount()
        {
            var data = QueryTestHelper.FormTestData((inst, index) => inst.IntProp = index, 100);
            int expectedCount = data.Count();
            var whereExpr = new WhereExpression<TestClass>("StringProp", WhereCombinator.greaterthanequal, "");
            Assert.Equal(expectedCount, whereExpr.Map(data).Count());
        }

        [Fact]
        public void WhereExpression_GivenNullableProperty_Map_ReturnsQueriableWhereNullableIsTakenIntoAccountWithNotCombinator()
        {
            var data = QueryTestHelper.FormTestData((x, i) =>
            {
                if (i % 2 == 0)
                    x.IntNullable = null;
                x.IntNullable = i;
            }, 10);
            var notnullcount = data.ToList().Where(x => x.IntNullable.HasValue).Count();
            var whereExpr = new WhereExpression<TestClass>("IntNullable", WhereCombinator.not, null);
            Assert.Equal(notnullcount, whereExpr.Map(data).Count());
        }

        [Fact]
        public void WhereExpression_GivenNullableProperty_Map_ReturnsQueriableWhereNullableIsTakenIntoAccountWithEqualsCombinator()
        {
            var data = QueryTestHelper.FormTestData((x, i) =>
            {
                if (i % 2 == 0)
                    x.IntNullable = null;
                x.IntNullable = i;
            }, 10);
            var notnullcount = data.ToList().Where(x => !x.IntNullable.HasValue).Count();
            var whereExpr = new WhereExpression<TestClass>("IntNullable", WhereCombinator.equals, null);
            Assert.Equal(notnullcount, whereExpr.Map(data).Count());
        }

        [Fact]
        public void WhereExpression_ExpressionContstructor_SetsPropertyPropertyName()
        {
            var whereExpr = new WhereExpression<TestClass>(t => t.IntNullable, WhereCombinator.equals, 4);
            Assert.Equal("intnullable", whereExpr.PropertyName);
        }

        [Fact]
        public void WhereExpression_ToString_StartsWithPropertyName()
        {
            var whereExpr = new WhereExpression<TestClass>(t => t.IntNullable, WhereCombinator.equals, 4);
            Assert.True(whereExpr.ToString().StartsWith(whereExpr.PropertyName));
        }

        [Theory]
        [InlineData(WhereCombinator.equals, "equals")]
        [InlineData(WhereCombinator.greaterthan, "greaterthan")]
        [InlineData(WhereCombinator.greaterthanequal, "greaterthanequal")]
        [InlineData(WhereCombinator.lessthan, "lessthan")]
        [InlineData(WhereCombinator.lessthanequal, "lessthanequal")]
        [InlineData(WhereCombinator.like, "like")]
        [InlineData(WhereCombinator.not, "not")]
        public void WhereExpression_ToString_ContainsCombinator(WhereCombinator whereCombinator, string expected)
        {
            var whereExpr = new WhereExpression<TestClass>(t => t.IntNullable, whereCombinator, 4);
            Assert.Contains(expected, whereExpr.ToString());
        }

        [Fact]
        public void WhereExpression_ToString_EndsWithValueProperty()
        {
            var whereExpr = new WhereExpression<TestClass>(t => t.IntNullable, WhereCombinator.equals, 4);
            Assert.True(whereExpr.ToString().EndsWith(whereExpr.Value.ToString()));
        }

        [Fact]
        public void WhereExpression_ToString_EndsWithNull_IfValuePropertyNull()
        {
            var whereExpr = new WhereExpression<TestClass>(t => t.IntNullable, WhereCombinator.equals, null);
            Assert.True(whereExpr.ToString().EndsWith("null"));
        }

        [Fact]
        public void WhereExpression_ToString_ContainsTwoInnerExpressionDeliminators()
        {
            Assert.Equal(2, new WhereExpression<TestClass>(t => t.IntNullable, WhereCombinator.equals, 4)
                            .ToString()
                            .ToCharArray()
                            .Where(x => x == ParserConstants.InnerExpressionSeparator)
                            .Count());
        }

        [Fact]
        public void WhereExpression_GivenLikeCombinator_Map_ReturnsIntegersThatContainOneWithListZeroToTwenty()
        {
            var whereExpr = new WhereExpression<TestClass>(x => x.IntProp, WhereCombinator.like, 1);
            var data = QueryTestHelper.FormTestData((x, i) => x.IntProp = i + 1, 20);
            int count = 11;
            Assert.Equal(count, whereExpr.Map(data).Count());
        }
    }
}
