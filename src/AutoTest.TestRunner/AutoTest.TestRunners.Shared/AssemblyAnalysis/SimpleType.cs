using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public class SimpleClass : SimpleType
    {
        public IEnumerable<SimpleField> Fields { get; private set; }
        public IEnumerable<SimpleMethod> Methods { get; private set; }

        public SimpleClass(string fullname, IEnumerable<string> attributes, IEnumerable<SimpleField> fields, IEnumerable<SimpleMethod> methods)
            : base(fullname, attributes)
        {
            Fields = fields;
            Methods = methods;
        }
    }

    public class SimpleMethod : SimpleType
    {
        public SimpleMethod(string fullname, IEnumerable<string> attributes)
            : base(fullname, attributes)
        {
        }
    }

    public class SimpleField : SimpleType
    {
        public string FieldType { get; private set; }

        public SimpleField(string fullname, IEnumerable<string> attributes, string fieldType)
            : base(fullname, attributes)
        {
            FieldType = fieldType;
        }
    }

    public class SimpleType
    {
        public string Fullname { get; private set; }
        public IEnumerable<string> Attributes { get; private set; }

        public SimpleType(string fullname, IEnumerable<string> attributes)
        {
            Fullname = fullname;
            Attributes = attributes;
        }
    }
}
