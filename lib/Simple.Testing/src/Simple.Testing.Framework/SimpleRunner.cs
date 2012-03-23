using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Simple.Testing.Framework
{
    public static class SimpleRunner
    {
        public static IEnumerable<RunResult> RunByName(Assembly assembly, string methodName)
        {
            return RunFromGenerator(new NamedMethodsGenerator(assembly, new[] {methodName}));
        }
        public static IEnumerable<RunResult> RunByName(Assembly assembly, IEnumerable<string> methodNames)
        {
            return RunFromGenerator(new NamedMethodsGenerator(assembly, methodNames));
        }
        public static IEnumerable<RunResult> RunAllInType(Assembly assembly, string typeName)
        {
            return RunAllInType(assembly.GetType(typeName));
        }

        public static IEnumerable<RunResult> RunAllInType(Type t)
        {
            return RunFromGenerator(new OnTypeGenerator(t));
        }

        public static IEnumerable<RunResult> RunAllInType<T>()
        {
            return RunAllInType(typeof (T));
        }

        public static IEnumerable<RunResult> RunAllInAssembly(string assemblyName)
        {
            var assembly = Assembly.LoadFrom(assemblyName);
            return RunAllInAssembly(assembly);
        }

        public static IEnumerable<RunResult> RunAllInAssembly(Assembly assembly)
        {
            return RunFromGenerator(new RootGenerator(assembly));
        }

        public static IEnumerable<RunResult> RunFromGenerator(ISpecificationGenerator generator)
        {
            var runner = new SpecificationRunner();
            return generator.GetSpecifications().Select(runner.RunSpecifciation);
        } 
    }
}
