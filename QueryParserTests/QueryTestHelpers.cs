using System;
using System.Collections.Generic;
using System.Linq;

namespace QueryParserTests
{
    internal static class QueryTestHelper
    {
        internal static IQueryable<TestClass> FormTestData(Action<TestClass, int> mutator, int number)
        {
            var list = new List<TestClass>();
            for (int i = 0; i < number; i++)
            {
                var inst = new TestClass();
                mutator(inst, i);
                list.Add(inst);
            }
            return list.AsQueryable();
        }
    }

    internal class TestClass
    {
        public int IntProp { get; set; }
        public string StringProp { get; set; }
        public bool BoolProp { get; set; }
        public double DoubleProp { get; set; }
        public char CharProp { get; set; }
        public Nullable<int> IntNullable { get; set; }
    }
}
