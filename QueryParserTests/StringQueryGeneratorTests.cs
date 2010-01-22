using System.Collections.Generic;
using System.Linq;
using QueryParser;
using Xunit;

namespace QueryParserTests
{
    public class StringQueryGeneratorTests
    {
        [Fact]
        public void StringQueryGenerator_GivenNullExpressionEnumerable_Generate_ReturnsEmptyString()
        {
            var generator = new StringQueryGenerator<TestClass>(null);
            Assert.Equal(string.Empty, generator.Generate());
        }

        [Fact]
        public void StringQueryGenerator_GivenEmptyExpressionEnumerable_Generatre_ReturnsEmptyString()
        {
            var generator = new StringQueryGenerator<TestClass>(new IParserExpression<TestClass>[0]);
            Assert.Equal(string.Empty, generator.Generate());
        }

        [Fact]
        public void StringQueryGenerator_GivenNonEmptyExpressionEnumerable_Generate_ReturnsStringStartingWithSlash()
        {
            var generator = new StringQueryGenerator<TestClass>(Generate(new SortExpression<TestClass>(new string[] { "IntProp" })));
            Assert.True(generator.Generate().StartsWith(ParserConstants.ExpressionSeparator.ToString()));
        }

        [Fact]
        public void StringQueryGenerator_GivenSortExpression_Generate_ReturnsStringWithSortInOutput()
        {
            var generator = new StringQueryGenerator<TestClass>(Generate(new SortExpression<TestClass>(new string[] { "IntProp" })));
            Assert.True(generator.Generate().Contains(ParserConstants.SortIndicator.ToString()));
        }

        [Fact]
        public void StringQueryGenerator_GivenSortExpression_Generate_ReturnsSortWithColumnNameLowerCase()
        {
            var generator = new StringQueryGenerator<TestClass>(Generate(new SortExpression<TestClass>(new string[] { "IntProp" })));
            Assert.True(generator.Generate().Contains("intprop"));
        }


        private IEnumerable<IParserExpression<TestClass>> Generate(params IParserExpression<TestClass>[] exprs)
        {
            return exprs.AsEnumerable();
        }
    }
}
