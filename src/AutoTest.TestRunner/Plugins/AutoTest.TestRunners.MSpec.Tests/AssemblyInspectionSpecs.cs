using NUnit.Framework;

namespace AutoTest.TestRunners.MSpec.Tests
{
    public class When_inspecting_an_assembly_referencing_Machine_Specifications : Spec
    {
        Runner _runner;
        bool _canContainContexts;
        string _assembly;

        protected override void Establish_context()
        {
            _runner = new Runner();
            _assembly = "AutoTest.TestRunners.MSpec.Tests.TestResource.dll".Path();
        }

        protected override void Because_of()
        {
            _canContainContexts = _runner.ContainsTestsFor(_assembly);
        }

        [Test]
        public void Should_be_considered_a_context_container()
        {
            Assert.That(_canContainContexts, Is.True);
        }
    }

    public class When_inspecting_an_assembly_not_referencing_Machine_Specifications : Spec
    {
        Runner _runner;
        bool _canContainContexts;
        string _assembly;

        protected override void Establish_context()
        {
            _runner = new Runner();
            _assembly = "AutoTest.TestRunners.MSpec.Tests.dll".Path();
        }

        protected override void Because_of()
        {
            _canContainContexts = _runner.ContainsTestsFor(_assembly);
        }

        [Test]
        public void Should_not_be_considered_a_context_container()
        {
            Assert.That(_canContainContexts, Is.False);
        }
    }
}