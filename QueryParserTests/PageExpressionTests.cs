using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QueryParser;
using Xunit;
using Xunit.Extensions;

namespace QueryParserTests
{
    public class PageExpressionTests
    {
        [Fact]
        public void PageExpression_Constructor_ThrowsException_WithZeroPageSize()
        {
            Assert.Throws<InvalidOperationException>(() => new PageExpression<TestClass>(10, 0));
        }

        [Fact]
        public void PageExpression_Constructor_ThrowsException_WithZeroPageNumber()
        {
            Assert.Throws<InvalidOperationException>(() => new PageExpression<TestClass>(0, 10));
        }

        [Fact]
        public void PageExpression_WithPageOne_SkipIsZero()
        {
            Assert.Equal(0, new PageExpression<TestClass>(1, 10).Skip);
        }

        [Fact]
        public void PageExpression_WithPageTwo_SkipIsPageSize()
        {
            Assert.Equal(10, new PageExpression<TestClass>(2, 10).Skip);
        }

        [Fact]
        public void PageExpression_Take_EqualToPageSize()
        {
            int pageSize = 103;
            Assert.Equal(pageSize, new PageExpression<TestClass>(1, pageSize).Take);
        }

        [Fact]
        public void PageExpression_Map_CountIsEqualToPageSize()
        {
            int pageSize = 103;
            var query = new ForeverEnumerable().AsQueryable();
            Assert.Equal(pageSize, new PageExpression<int>(1, pageSize).Map(query).Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(400)]
        public void PageExpression_Map_FirstIsEqualToSkipPlusOne(int pageNumber)
        {
            var query = new ForeverEnumerable().AsQueryable();
            var pageExpr = new PageExpression<int>(pageNumber, 10);
            Assert.Equal(pageExpr.Skip + 1, pageExpr.Map(query).ToList()[0]);
        }

        [Fact]
        public void PageExpression_ToString_StartsWithPageIndicator()
        {
            Assert.True(new PageExpression<TestClass>(1, 10).ToString().StartsWith(ParserConstants.PageIndicator));
        }

        [Fact]
        public void PageExpression_ToString_ContainsPageNumber()
        { 
            int pageNumber = 1234;
            Assert.Contains(pageNumber.ToString(), new PageExpression<TestClass>(pageNumber, 11).ToString());
        }

        [Fact]
        public void PageExpression_ToString_ContainsPageSizeIfNotDefault()
        {
            int pageSize = ParserConstants.DefaultPageSize + 1;
            Assert.Contains(pageSize.ToString(), new PageExpression<TestClass>(1, pageSize).ToString());
        }

        [Fact]
        public void PageExpression_ToString_DoesNotContainsPageSizeIfDefault()
        {
            int pageSize = ParserConstants.DefaultPageSize;
            Assert.False(new PageExpression<TestClass>(1, pageSize).ToString().EndsWith(pageSize.ToString()));
        }

        private class ForeverEnumerable : IEnumerable<int>
        {
            public IEnumerator<int> GetEnumerator()
            {
                return new ForeverEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new ForeverEnumerator();
            }

            private class ForeverEnumerator : IEnumerator<int>
            {

                public int Current { get; private set; }

                public void Dispose()
                {
                }

                object IEnumerator.Current
                {
                    get { return Current; }
                }

                public bool MoveNext()
                {
                    Current++;
                    return true;
                }

                public void Reset()
                {
                    Current = 0;
                }
            }
        }
    }
}
