using QueryParser;
using Xunit;

namespace QueryParserTests
{
    public class PropertyNameResolverTests
    {
        internal const string DIFFERENTNAME = "MyProp";

        [Fact]
        public void PropertyNameResolver_FindProperty_GivenNormalPropertyName_ReturnsNormalProperty()
        {
            var expected = typeof(InnerTestingClass).GetProperty("NormalProperty");
            var actual = PropertyNameResolver.FindProperty<InnerTestingClass>("NormalProperty");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PropertyNameResolver_FindProperty_GivenNormalPropertyNameWithIncorrectCase_ReturnsNormalProperty()
        {
            var expected = typeof(InnerTestingClass).GetProperty("NormalProperty");
            var actual = PropertyNameResolver.FindProperty<InnerTestingClass>("normalproperty");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PropertyNameResolver_FindProperty_GivenIgnoredProperty_ReturnsNull()
        {
            Assert.Null(PropertyNameResolver.FindProperty<InnerTestingClass>("IgnoreMe"));
        }

        [Fact]
        public void PropertyNameResolver_FindProperty_GivenPropertyThatDoesntExist_ReturnsNull()
        {
            Assert.Null(PropertyNameResolver.FindProperty<InnerTestingClass>("NonExistingProperty"));
        }

        [Fact]
        public void PropertyNameResolver_FindProperty_GivenPropertyWithParserPropertyNameAttribute_ReturnsCorrectProperty()
        {
            var expected = typeof(InnerTestingClass).GetProperty("DifferentName");
            var actual = PropertyNameResolver.FindProperty<InnerTestingClass>(DIFFERENTNAME);
            Assert.Equal(expected, actual);
        }

        private class InnerTestingClass
        {
            [ParserIgnore]
            public string IgnoreMe { get; set; }

            public string NormalProperty { get; set; }

            [ParserPropertyName(DIFFERENTNAME)]
            public string DifferentName { get; set; }
        }
    }
}
