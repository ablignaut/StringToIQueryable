using System;
using System.Collections.Generic;
using System.Linq;
using QueryParser;
using Xunit;

namespace QueryParserTests
{
    public class SortExpressionTests
    {
        [Fact]
        public void SortExpression_WithNoColumns_ReturnsSameNumberElemsAsQuery()
        {
            int number = 100;
            var query = QueryTestHelper.FormTestData((x, i) => x.IntProp = i, number);
            var sortexpr = new SortExpression<TestClass>(t => t.StringProp);
            Assert.Equal(number, sortexpr.Map(query).Count());
        }

        [Fact]
        public void SortExpression_WithNullColumns_ReturnsSameNumberElemsAsQuery()
        {
            int number = 100;
            var query = QueryTestHelper.FormTestData((x, i) => x.IntProp = i, number);
            var sortexpr = new SortExpression<TestClass>(t => t.StringProp);
            Assert.Equal(number, sortexpr.Map(query).Count());
        }

        [Fact]
        public void SortExpression_WithOneColumn_ReturnsTheSampleEnumerableBackwards()
        {
            var query = QueryTestHelper.FormTestData((x, i) => x.IntProp = i, 10);
            var back = QueryTestHelper.FormTestData((x, i) => x.IntProp = i, 10).Reverse();
            var sortexpr = new SortExpression<TestClass>(new string[] { "IntProp" }, true);
            Assert.Equal(back.ToList()[0].IntProp, sortexpr.Map(query).ToList()[0].IntProp);
        }

        [Fact]
        public void SortExpression_WithTwoColumnsAscendingSort_ReturnsSameEnumerable()
        {
            var random = new Random();
            var query = QueryTestHelper.FormTestData((x, i) => 
                {
                    x.IntProp = random.Next(i);
                    x.CharProp = (char)random.Next((int)char.MinValue, (int)char.MaxValue);
                }, 10);
            var sorted = new List<TestClass>(query.ToArray()).OrderBy(x => x.IntProp).ThenBy(x => x.CharProp).ToList();
            var sortexpr = new SortExpression<TestClass>(new string[] { "IntProp", "CharProp" });

            var output = sortexpr.Map(query).ToList();

            Assert.Equal(sorted[0].IntProp, output[0].IntProp);
            Assert.Equal(sorted[0].CharProp, output[0].CharProp);
        }

        [Fact]
        public void SortExpression_WithTwoColumnsDescendingSort_ReturnsSameEnumerable()
        {
            var random = new Random();
            var query = QueryTestHelper.FormTestData((x, i) =>
            {
                x.IntProp = random.Next(i);
                x.CharProp = (char)random.Next((int)char.MinValue, (int)char.MaxValue);
            }, 10);
            var sorted = new List<TestClass>(query.ToArray()).OrderByDescending(x => x.IntProp).ThenByDescending(x => x.CharProp).ToList();
            var sortexpr = new SortExpression<TestClass>(new string[] { "IntProp", "CharProp" }, true);

            var output = sortexpr.Map(query).ToList();

            Assert.Equal(sorted[0].IntProp, output[0].IntProp);
            Assert.Equal(sorted[0].CharProp, output[0].CharProp);
        }

        [Fact]
        public void SortExpression_ExpressionConstructor_WithOneMemberExpression_HasOneColumn()
        {
            var sort = new SortExpression<TestClass>(prop => prop.IntProp);
            Assert.Equal(1, sort.Columns.Length);
        }

        [Fact]
        public void SortExpression_ExpressionConstructor_WithOneMemberExpression_HasCorrectColumnName()
        {
            var sort = new SortExpression<TestClass>(prop => prop.IntProp);
            Assert.Equal("intprop", sort.Columns[0]);
        }

        [Fact]
        public void SortExpression_ToString_WithTwoColumns_HasHasBothColumnsInOutput()
        {
            var sort = new SortExpression<TestClass>(prop => prop.IntProp, prop => prop.StringProp);
            Assert.Contains("intprop,stringprop", sort.ToString());
        }

        [Fact]
        public void SortExpression_ToString_WithOneColumn_HasHasColumnNameInOutput()
        {
            var sort = new SortExpression<TestClass>(prop => prop.IntProp);
            Assert.Contains("intprop", sort.ToString());
        }

        [Fact]
        public void SortExpression_ToString_WithDescending_HasHasDescendingIndicatorInOutput()
        {
            var sort = new SortExpression<TestClass>(true, prop => prop.IntProp);
            Assert.Contains(ParserConstants.DescendingIndicator, sort.ToString());
        }

        [Fact]
        public void SortExpression_WithTwoColumns_ToString_ContainsSortPrefix()
        {
            var sort = new SortExpression<TestClass>(prop => prop.IntProp, prop => prop.StringProp);
            Assert.True(sort.ToString().StartsWith(ParserConstants.SortIndicator));
        }

        [Fact]
        public void SortExpression_Contructor_ThrowsExceptions_WhenZeroColumns()
        {
            Assert.Throws<InvalidOperationException>(() => new SortExpression<TestClass>(new string[0]));
        }

        [Fact]
        public void SortExpression_ExpressionContructor_ThrowsExceptions_WhenZeroColumns()
        {
            Assert.Throws<InvalidOperationException>(() => new SortExpression<TestClass>());
        }
    }
}
