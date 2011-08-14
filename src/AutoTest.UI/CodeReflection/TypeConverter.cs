using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace AutoTest.UI.CodeReflection
{
    public class TypeConverter
    {
        private AssemblyDefinition _assembly;

        public TypeConverter(string assembly)
        {
            _assembly = AssemblyDefinition.ReadAssembly(assembly);
        }

        public string ToSignature(string type)
        {
            string signature = null;
            foreach (var m in _assembly.Modules)
            {
                signature = getSignature(m.GetTypes(), type);
                if (signature != null) break;
            }
            return signature;
        }

        private string getSignature(IEnumerable<TypeDefinition> types, string type)
        {
            string signature = null;
            foreach (var t in types)
            {
                if (t.FullName.Equals(type))
                    signature = t.FullName;
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

        private string getMethods(string parent, Collection<MethodDefinition> methods, string type)
        {
            var method = methods
                .Where(x => type.Equals(string.Format("{0}.{1}", parent, x.Name)))
                .FirstOrDefault();
            if (method == null)
                return null;
            return method.FullName;
        }

        private string getFields(string parent, Collection<FieldDefinition> fields, string type)
        {
            var field = fields
                .Where(x => type.Equals(string.Format("{0}.{1}", parent, x.Name)))
                .FirstOrDefault();
            if (field == null)
                return null;
            return field.FullName;
        }
    }
}
