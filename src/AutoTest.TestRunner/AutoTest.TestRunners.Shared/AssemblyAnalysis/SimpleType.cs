using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public enum TypeCategory
    {
        Class,
        Method,
        Field
    }

    public class SimpleType
    {
        public TypeCategory Category { get; private set; }
        public IEnumerable<string> Attributes { get; private set; }
        public string TypeName { get; private set; }
        public bool IsPublic { get; private set; }
        public string Fullname { get; private set; }
        public IEnumerable<SimpleType> Methods { get; private set; }
        public IEnumerable<SimpleType> Fields { get; private set; }

        public SimpleType(TypeCategory category, string fullname, string typeName, bool isPublic, IEnumerable<string> attributes, IEnumerable<SimpleType> methods, IEnumerable<SimpleType> fields)
        {
            Category = category;
            Fullname = fullname;
            TypeName = typeName;
            IsPublic = isPublic;
            Attributes = attributes;
            Methods = methods;
            Fields = fields;
        }
    }
}
