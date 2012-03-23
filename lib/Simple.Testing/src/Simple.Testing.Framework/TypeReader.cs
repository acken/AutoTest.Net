using System;
using System.Collections.Generic;
using System.Reflection;
using Simple.Testing.ClientFramework;

namespace Simple.Testing.Framework
{
    public static class TypeReader
    {
        public static IEnumerable<SpecificationToRun> GetSpecificationsIn(Type t)
        {
            foreach (var methodSpec in AllMethodSpecifications(t)) yield return methodSpec;
            foreach (var fieldSpec in AllFieldSpecifications(t)) yield return fieldSpec;
        }

        private static IEnumerable<SpecificationToRun> AllMethodSpecifications(Type t)
        {
            foreach (var s in t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (typeof(Specification).IsAssignableFrom(s.ReturnType))
                {
                    var result = s.CallMethod();
                    if (result != null) yield return new SpecificationToRun((Specification) result, s);
                }
                if (typeof(IEnumerable<Specification>).IsAssignableFrom(s.ReturnType))
                {
                    var obj = (IEnumerable<Specification>)s.CallMethod();
                    foreach (var item in obj)
                        yield return new SpecificationToRun(item, s);
                }
            }
        }

        private static IEnumerable<SpecificationToRun> AllFieldSpecifications(Type t)
        {
            foreach (var m in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (typeof(Specification).IsAssignableFrom(m.FieldType))
                {
                    yield return new SpecificationToRun((Specification) m.GetValue(Activator.CreateInstance(t)), m);
                }
                if (typeof(IEnumerable<Specification>).IsAssignableFrom(m.FieldType))
                {
                    var obj = (IEnumerable<Specification>)m.GetValue(Activator.CreateInstance(t));
                    foreach (var item in obj)
                        yield return new SpecificationToRun(item, m);
                }
            }
        }
    }
}