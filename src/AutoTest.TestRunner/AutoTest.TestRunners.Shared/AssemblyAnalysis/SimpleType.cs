using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public enum TypeCategory
    {
        Class,
        Method
    }

    public class SimpleType
    {
        public TypeCategory Category { get; private set; }
        public IEnumerable<string> Attributes { get; private set; }
        public string Fullname { get; private set; }
        public IEnumerable<SimpleType> Methods { get; private set; }

        public SimpleType(TypeCategory category, string fullname, IEnumerable<string> attributes, IEnumerable<SimpleType> methods)
        {
            Category = category;
            Fullname = fullname;
            Attributes = attributes;
            Methods = methods;
        }
    }
}
