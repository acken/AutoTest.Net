using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace AutoTest.UI.CodeReflection
{
    public class TypeConverter : IDisposable
    {
        private AssemblyDefinition _assembly = null;

        public TypeConverter(string assembly)
        {
            _assembly = AssemblyDefinition.ReadAssembly(assembly);
        }

        public Signature ToSignature(string type)
        {
            Signature signature = null;
            foreach (var m in _assembly.Modules)
            {
                signature = getSignature(m.GetTypes(), type);
                if (signature != null) break;
            }
            return signature;
        }

        private Signature getSignature(IEnumerable<TypeDefinition> types, string type)
        {
            Signature signature = null;
            foreach (var t in types)
            {
                if (t.FullName.Equals(type))
                    signature = new Signature(SignatureTypes.Class, t.FullName);
                else if (signature == null && t.HasNestedTypes)
                    signature = getSignature(t.NestedTypes, type);
                else if (signature == null && t.HasMethods)
                    signature = getMethods(t.FullName, t.Methods, type);
                else if (signature == null && t.HasFields)
                    signature = getFields(t.FullName, t.Fields, type);

                if (signature != null) break;
            }
            return signature;
        }

        private Signature getMethods(string parent, Collection<MethodDefinition> methods, string type)
        {
            var method = methods
                .Where(x => type.Equals(string.Format("{0}.{1}", parent, x.Name)))
                .FirstOrDefault();
            if (method == null)
                return null;
            return new Signature(SignatureTypes.Method, method.FullName);
        }

        private Signature getFields(string parent, Collection<FieldDefinition> fields, string type)
        {
            var field = fields
                .Where(x => type.Equals(string.Format("{0}.{1}", parent, x.Name)))
                .FirstOrDefault();
            if (field == null)
                return null;
            return new Signature(SignatureTypes.Field, field.FullName);
        }

        public void Dispose()
        {
            if (_assembly != null)
                _assembly.Dispose();
        }
    }

    public class Signature
    {
        public SignatureTypes Type { get; private set; }
        public string Name { get; private set; }

        public Signature(SignatureTypes type, string signature)
        {
            Type = type;
            Name = signature;
        }
    }

    public enum SignatureTypes
    {
        Class,
        Method,
        Field
    }
}
