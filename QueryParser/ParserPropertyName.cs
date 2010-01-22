using System;

namespace QueryParser
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParserPropertyName : Attribute
    {
        public ParserPropertyName(string name)
        {
            PropertyName = name;
        }

        public string PropertyName { get; private set; }
    }
}
