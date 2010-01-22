using System;

namespace QueryParser
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParserIgnoreAttribute : Attribute
    {
    }
}