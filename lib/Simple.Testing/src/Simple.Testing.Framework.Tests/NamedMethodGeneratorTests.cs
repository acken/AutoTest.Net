using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Simple.Testing.ClientFramework;

namespace Simple.Testing.Framework.Tests
{
    public class NamedMethodGeneratorTests
    {
        public Specification when_finding_a_method_on_normal_class = new QuerySpecification<NamedMethodsGenerator, IEnumerable<SpecificationToRun>>()
        {
            On = () => new NamedMethodsGenerator(Assembly.GetExecutingAssembly(), "Simple.Testing.Framework.Tests.NamedGeneratorTestsData.A_Test"),
            When = runner => runner.GetSpecifications(),
            Expect =
                {
                    result => result.Count() == 1,
                    result => result.First().FoundOn.Name == "A_Test",
                    result => result.First().IsRunnable,
                    result => result.First().Exception == null,
                    result => result.First().Specification.GetType() == typeof(PassingSpecification<int>),
                }
        };

        public Specification when_finding_a_method_on_nested_class = new QuerySpecification<NamedMethodsGenerator, IEnumerable<SpecificationToRun>>()
        {
            On = () => new NamedMethodsGenerator(Assembly.GetExecutingAssembly(), "Simple.Testing.Framework.Tests.NamedGeneratorTestsData+Nested.A_Test"),
            When = runner => runner.GetSpecifications(),
            Expect =
                {
                    result => result.Count() == 1,
                    result => result.First().FoundOn.Name == "A_Test",
                    result => result.First().IsRunnable,
                    result => result.First().Exception == null,
                    result => result.First().Specification.GetType() == typeof(PassingSpecification<int>),
                }
        };

        public Specification when_finding_a_non_existant_method = new FailingSpecification<NamedMethodsGenerator, ArgumentException>()
        {
            On = () => new NamedMethodsGenerator(Assembly.GetExecutingAssembly(), "SOmethingWrong.Testing.Framework.Tests.NamedGeneratorTestsData+Nested.A_Test"),
            When = runner => runner.GetSpecifications(),
            Expect =
                {
                    exception => exception.ParamName == "method"
                }
        };
    }

    public class NamedGeneratorTestsData
    {
        public class Nested
        {
            public Specification A_Test()
            {
                return new PassingSpecification<int>();
            }
        }
        public Specification A_Test()
        {
            return new PassingSpecification<int>();
        }
    }
}
