using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Collections.Generic;
using System.IO;

namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public class SimpleTypeLocator
    {
        private string _assembly;
        private string _type;

        public SimpleTypeLocator(string assembly, string type)
        {
            _assembly = assembly;
            _type = type;
        }

        public SimpleType Locate()
        {
            if (!File.Exists(_assembly))
                return null;
            var asm = AssemblyDefinition.ReadAssembly(_assembly);
            foreach (var module in asm.Modules)
            {
                var result = locateSimpleType(module.Types);
                if (result != null)
                    return result;
            }
            return null;
        }

        public SimpleType LocateParent()
        {
            var end = _type.LastIndexOf('.');
            if (end == -1)
                return null;
            _type = _type.Substring(0, end);
            return Locate();
        }

        private SimpleType locateSimpleType(Collection<TypeDefinition> types)
        {
            foreach (var type in types)
            {
                if (type.FullName.Equals(_type))
                    return getType(type);
                var result = locateSimpleType(type.NestedTypes);
                if (result != null)
                    return result;
                result = locateSimpleType(type.Methods, type.FullName);
                if (result != null)
                    return result;
            }
            return null;
        }

        private SimpleType getType(TypeDefinition type)
        {
            return new SimpleType(
                TypeCategory.Class,
                type.FullName,
                null,
                type.IsPublic,
                getTypeAttributes(type),
                type.Methods.Select(x => new SimpleType(
                    TypeCategory.Method,
                    type.FullName + "." + x.Name, 
                    x.MethodReturnType.ReturnType.FullName,
                    x.IsPublic,
                    getAttributes(x.CustomAttributes),
                    new SimpleType[] { },
                    new SimpleType[] { })),
                type.Fields.Select(x => new SimpleType(
                    TypeCategory.Field,
                    type.FullName + "." + x.Name,
                    x.FieldType.FullName,
                    x.IsPublic,
                    getAttributes(x.CustomAttributes),
                    new SimpleType[] { },
                    new SimpleType[] { })));
        }

        private IEnumerable<string> getTypeAttributes(TypeDefinition type)
        {
            var attributes = new List<string>();
            attributes.AddRange(getAttributes(type.CustomAttributes));
            var baseType = type.BaseType as TypeDefinition;
            if (baseType != null)
                attributes.AddRange(getTypeAttributes(baseType));
            return attributes;
        }

        private IEnumerable<string> getAttributes(ICollection<CustomAttribute> customAttributes)
        {
            var attributes = new List<string>();
            foreach (var attribute in customAttributes)
            {
                var type = attribute.AttributeType;
                attributes.Add(type.FullName);
                addBaseAttributes(attributes, type as TypeDefinition);
            }
            return attributes;
        }

        private void addBaseAttributes(List<string> attributes, TypeDefinition type)
        {
            if (type == null)
                return;
            if (type.BaseType == null)
                return;
            attributes.Add(type.BaseType.FullName);
            addBaseAttributes(attributes, type.BaseType as TypeDefinition);
        }

        private SimpleType locateSimpleType(Collection<MethodDefinition> methods, string typeFullname)
        {
            foreach (var method in methods)
            {
                var fullName = typeFullname + "." + method.Name;
                if (fullName.Equals(_type))
                    return new SimpleType(TypeCategory.Method, fullName, method.MethodReturnType.ReturnType.FullName, method.IsPublic, getAttributes(method.CustomAttributes), new SimpleType[] { }, new SimpleType[] { });
            }
            return null;
        }
    }
}
