using Simple.Testing.Framework;

namespace AutoTest.TestRunners.SimpleTesting.Tests.Resources
{

    public class SomeClass
    {
        public void Foo()
        {
            
        }
    }

    public class SimpleTestingTests
    {
        public Specification a_passing_test()
        {
            return new QuerySpecification<Foo, int>
                       {
                           On = () => new Foo(),
                           When = x => x.Bar(),
                           Expect =
                               {
                                   q => q == 12
                               }
                       };
        }
    }

    public class Foo
    {
        public int Bar()
        {
            return 12;
        }
    }
}
