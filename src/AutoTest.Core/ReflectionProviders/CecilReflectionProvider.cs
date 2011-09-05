using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using Mono.Cecil;
using System.IO;
using AutoTest.TestRunners.Shared.Targeting;
using Mono.Collections.Generic;

namespace AutoTest.Core.ReflectionProviders
{
    public class CecilReflectionProvider : IReflectionProvider
    {
        private AssemblyDefinition _assembly = null;

        public CecilReflectionProvider(string assembly)
        {
            if (!File.Exists(assembly))
                return;
            _assembly = AssemblyDefinition.ReadAssembly(assembly);
        }

        public string GetName()
        {
            return _assembly.FullName;
        }

        public Version GetTargetFramework()
        {
            try
            {
                var runtime = _assembly.MainModule.Runtime.ToString().Replace("Net_", "").Replace("_", ".");
                return new Version(runtime);
            }
            catch
            {
                return new Version();
            }
        }

        public Platform GetPlatform()
        {
            try
            {
                var architecture = _assembly.MainModule.Architecture;
                if (architecture == TargetArchitecture.I386)
                    return Platform.x86;
                if (architecture == TargetArchitecture.AMD64)
                    return Platform.AnyCPU;
                if (architecture == TargetArchitecture.IA64)
                    return Platform.AnyCPU;
                return Platform.Unknown;
            }
            catch
            {
                return Platform.Unknown;
            }
        }

        public IEnumerable<TypeName> GetReferences()
        {
            try
            {
                var references = _assembly.MainModule.AssemblyReferences;
                var names = new List<TypeName>();
                foreach (var reference in references)
                    names.Add(new TypeName(reference.FullName, reference.Name));
                return names;
            }
            catch
            {
            }
            return new TypeName[] { };
        }

        public string GetParentType(string type)
        {
            var end = type.LastIndexOf('.');
            if (end == -1)
                return null;
            return type.Substring(0, end);
        }

        public SimpleClass LocateClass(string type)
        {
            var cls = locate(type);
            if (cls == null)
                return null;
            if (cls.GetType().Equals(typeof(SimpleClass)))
                return (SimpleClass)cls;
            return null;
        }

        public SimpleMethod LocateMethod(string type)
        {
            var method = locate(type);
            if (method == null)
                return null;
            if (method.GetType().Equals(typeof(SimpleMethod)))
                return (SimpleMethod)method;
            return null;
        }

        public void Dispose()
        {
            _assembly = null;
        }

        private SimpleType locate(string type)
        {
            foreach (var module in _assembly.Modules)
            {
                var result = locateSimpleType(module.Types, type);
                if (result != null)
                    return result;
            }
            return null;
        }

        private SimpleType locateSimpleType(Collection<TypeDefinition> types, string typeName)
        {
            foreach (var type in types)
            {
                if (type.FullName.Equals(typeName))
                    return getType(type);
                var result = locateSimpleType(type.NestedTypes, typeName);
                if (result != null)
                    return result;
                result = locateSimpleMethod(type.Methods, typeName, type.FullName);
                if (result != null)
                    return result;
            }
            return null;
        }

        private SimpleType getType(TypeDefinition type)
        {
            return new SimpleClass(
                type.FullName,
                getTypeAttributes(type),
                type.Fields.Select(x => new SimpleField(
                    type.FullName + "." + x.Name,
                    getAttributes(x.CustomAttributes),
                    x.FieldType.FullName)),
                type.Methods.Select(x => new SimpleMethod(
                    type.FullName + "." + x.Name,
                    getAttributes(x.CustomAttributes),
                    x.IsAbstract)),
                    type.IsAbstract);
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

        private SimpleType locateSimpleMethod(Collection<MethodDefinition> methods, string typeName, string typeFullname)
        {
            foreach (var method in methods)
            {
                var fullName = typeFullname + "." + method.Name;
                if (fullName.Equals(typeName))
                    return new SimpleMethod(fullName, getAttributes(method.CustomAttributes), method.IsAbstract);
            }
            return null;
        }
    }
}
