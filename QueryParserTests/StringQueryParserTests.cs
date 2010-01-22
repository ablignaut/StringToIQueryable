using System.Linq;
using QueryParser;
using Xunit;
using Xunit.Extensions;

namespace QueryParserTests
{
    public class StringQueryParserTests
    {
        [Fact]
        public void StringQueryParser_GivenEmptyStringExpression_Parse_ReturnsNoParserExpressions()
        {
            Assert.Empty(new StringQueryParser<TestClass>(string.Empty).Parse());
        }

        [Fact]
        public void StringQueryParser_GivenSkipExpression_Parse_ReturnsOneParserExpression()
        {
            var exprs = new StringQueryParser<TestClass>("skip-5").Parse();

            Assert.Equal(1, exprs.Count());
        }

        [Fact]
        public void StringQueryParser_GivenInvalidSkipExpression_Parse_ReturnsEmptyParserExpressionEnumerable()
        {
            Assert.Empty(new StringQueryParser<TestClass>("skip-a").Parse());
        }

        [Fact]
        public void StringQueryParser_GivenSkipExpression_Parse_ReturnsOneSkipParserExpression()
        {
            var exprs = new StringQueryParser<TestClass>("skip-5").Parse();

            Assert.IsType<SkipExpression<TestClass>>(exprs.ToArray()[0]);
        }

        [Fact]
        public void StringQueryParser_GivenSkipExpression_Parse_ReturnsOneSkipParserExpressionWithCorrectSkipNumber()
        {
            int skipNum = 5;
            var exprs = new StringQueryParser<TestClass>("skip-" + skipNum).Parse();

            Assert.Equal(skipNum, (exprs.Single() as SkipExpression<TestClass>).SkipNumber);
        }

        [Fact]
        public void StringQueryParser_GivenExpressionWithsSlashAtStartAndEnd_Parse_ReturnsExpression()
        {
            var exprs = new StringQueryParser<TestClass>("/skip-5/").Parse();

            Assert.Equal(1, exprs.Count());
        }

        [Fact]
        public void StringQueryParser_GivenTakeExpression_Parse_ReturnsOneParserExpression()
        {
            var exprs = new StringQueryParser<TestClass>("take-5").Parse();

            Assert.Equal(1, exprs.Count());
        }

        [Fact]
        public void StringQueryParser_GivenTakeAllExpression_Parse_ReturnsOneTakeParserExpressionWithTakeAll()
        {
            var exprs = new StringQueryParser<TestClass>("take-all").Parse();

            Assert.Equal(-1, (exprs.Single() as TakeExpression<TestClass>).Take);
        }

        [Fact]
        public void StringQueryParser_GivenSkipExpression_Parse_ReturnsOneSortParserExpressionWithCorrectSkipNumber()
        {
            int takeNum = 5;
            var exprs = new StringQueryParser<TestClass>("take-" + takeNum).Parse();

            Assert.Equal(takeNum, (exprs.Single() as TakeExpression<TestClass>).Take);
        }

        [Fact]
        public void StringQueryParser_GivenDescSortExpression_Parse_ReturnsOneParserExpression()
        {
            var exprs = new StringQueryParser<TestClass>("sort-desc-intprop").Parse();
            Assert.Equal(1, exprs.Count());
        }

        [Fact]
        public void StringQueryParser_GivenAscSortExpression_Parse_ReturnsOneParserExpression()
        {
            var exprs = new StringQueryParser<TestClass>("sort-intprop").Parse();
            Assert.Equal(1, exprs.Count());
        }

        [Fact]
        public void StringQueryParser_GivenDescSortExpression_Parse_ReturnsSortParserExpression()
        {
            var exprs = new StringQueryParser<TestClass>("sort-desc-intprop").Parse();
            Assert.IsType<SortExpression<TestClass>>(exprs.Single());
        }

        [Fact]
        public void StringQueryParser_GivenDescSortExpression_Parse_ReturnsSortParserExpressionWithCorrectDescendingProperty()
        {
            var exprs = new StringQueryParser<TestClass>("sort-desc-intprop").Parse();
            Assert.True((exprs.Single() as SortExpression<TestClass>).Descending);
        }

        [Fact]
        public void StringQueryParser_GivenAscSortExpression_Parse_ReturnsSortParserExpressionWithCorrectDescendingProperty()
        {
            var exprs = new StringQueryParser<TestClass>("sort-intprop").Parse();
            Assert.False((exprs.Single() as SortExpression<TestClass>).Descending);
        }

        [Fact]
        public void StringQueryParser_GivenAscSortExpressionWithTwoColumns_Parse_ReturnsSortParserExpressionWithTwoColumns()
        {
            var exprs = new StringQueryParser<TestClass>("sort-intprop,stringprop").Parse();
            Assert.Equal(2, (exprs.Single() as SortExpression<TestClass>).Columns.Length);
        }

        [Fact]
        public void StringQueryParser_GivenDescSortExpressionWithTwoColumns_Parse_ReturnsSortParserExpressionWithTwoColumns()
        {
            var exprs = new StringQueryParser<TestClass>("sort-desc-intprop,stringprop").Parse();
            Assert.Equal(2, (exprs.Single() as SortExpression<TestClass>).Columns.Length);
        }

        [Fact]
        public void StringQueryParser_GivenWhereExpression_Parse_ReturnsOneParserExpression()
        {
            var exprs = new StringQueryParser<TestClass>("intprop-equals-4").Parse();
            Assert.Equal(1, exprs.Count());
        }

        [Fact]
        public void StringQueryParser_GivenWhereExpression_Parse_ReturnsOneWhereParserExpression()
        {
            var exprs = new StringQueryParser<TestClass>("intprop-equals-4").Parse();
            Assert.IsType<WhereExpression<TestClass>>(exprs.Single());
        }

        [Theory]
        [InlineData(WhereCombinator.equals, "equals")]
        [InlineData(WhereCombinator.greaterthan, "greaterthan")]
        [InlineData(WhereCombinator.greaterthanequal, "greaterthanequal")]
        [InlineData(WhereCombinator.lessthan, "lessthan")]
        [InlineData(WhereCombinator.lessthanequal, "lessthanequal")]
        [InlineData(WhereCombinator.like, "like")]
        [InlineData(WhereCombinator.not, "not")]
        public void StringQueryParser_GivenWhereExpression_Parse_ReturnsCorrectCombinatorProperty(WhereCombinator combinator, string combString)
        {
            var exprs = new StringQueryParser<TestClass>("intprop-" + combString + "-4").Parse();
            Assert.Equal(combinator, (exprs.Single() as WhereExpression<TestClass>).Combinator);
        }

        [Fact]
        public void StringQueryParser_GivenWhereExpression_Parse_ReturnsCorrectPropertyNameProperty()
        {
            var exprs = new StringQueryParser<TestClass>("intprop-equals-4").Parse();
            Assert.Equal("intprop", (exprs.Single() as WhereExpression<TestClass>).PropertyName);
        }

        [Fact]
        public void StringQueryParser_GivenWhereExpression_Parse_ReturnsCorrectValueProperty()
        {
            var exprs = new StringQueryParser<TestClass>("intprop-equals-4").Parse();
            Assert.Equal("4", (exprs.Single() as WhereExpression<TestClass>).Value);
        }

        [Fact]
        public void StringQueryParser_GivenPageExpression_Parse_ReturnsOneExpression()
        {
            var exprs = new StringQueryParser<TestClass>("page-4-4").Parse();
            Assert.Equal(1, exprs.Count());
        }

        [Fact]
        public void StringQueryParser_GivenPageExpression_Parse_ReturnsOnePageExpression()
        {
            var exprs = new StringQueryParser<TestClass>("page-4-4").Parse();
            Assert.IsType<PageExpression<TestClass>>(exprs.Single());
        }

        [Fact]
        public void StringQueryParser_GivenPageExpression_Parse_ParsesCorrectSkipNumber_OnPageOne()
        {
            var exprs = new StringQueryParser<TestClass>("page-1-8").Parse();
            Assert.Equal(0, (exprs.Single() as PageExpression<TestClass>).Skip);
        }

        [Fact]
        public void StringQueryParser_GivenPageExpression_Parse_ParsesCorrectSkipNumber_OnPageTwo()
        {
            var exprs = new StringQueryParser<TestClass>("page-2-8").Parse();
            Assert.Equal(8, (exprs.Single() as PageExpression<TestClass>).Skip);
        }

        [Fact]
        public void StringQueryParser_GivenPageExpression_Parse_ParsesCorrectTakeNumber()
        {
            var exprs = new StringQueryParser<TestClass>("page-2-8").Parse();
            Assert.Equal(8, (exprs.Single() as PageExpression<TestClass>).Take);
        }

        [Fact]
        public void StringQueryParser_GivenPageExpressionWithJustPageNumber_Parse_ParsesCorrectTakeNumber()
        {
            var exprs = new StringQueryParser<TestClass>("page-2").Parse();
            Assert.Equal(ParserConstants.DefaultPageSize, (exprs.Single() as PageExpression<TestClass>).Take);
        }

        [Fact]
        public void StringQueryParser_GivenPageExpressionWithJustPageNumber_Parse_ParsesCorrectTSkipNumber_OnPageOne()
        {
            var exprs = new StringQueryParser<TestClass>("page-1").Parse();
            Assert.Equal(0, (exprs.Single() as PageExpression<TestClass>).Skip);
        }

        [Fact]
        public void StringQueryParser_GivenPageExpressionWithJustPageNumber_Parse_ParsesCorrectTSkipNumber_OnPageTwo()
        {
            var exprs = new StringQueryParser<TestClass>("page-2").Parse();
            Assert.Equal(ParserConstants.DefaultPageSize, (exprs.Single() as PageExpression<TestClass>).Skip);
        }
    }
}
