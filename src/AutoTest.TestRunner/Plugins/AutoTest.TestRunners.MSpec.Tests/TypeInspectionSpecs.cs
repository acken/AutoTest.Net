using NUnit.Framework;

namespace AutoTest.TestRunners.MSpec.Tests
{
    public class When_inspecting_a_class_with_It_fields : Spec
    {
        Runner _runner;
        bool _isContext;
        string _assembly;
        string _class;

        protected override void Establish_context()
        {
            _runner = new Runner();
            _assembly = "AutoTest.TestRunners.MSpec.Tests.TestResource.dll".Path();
            _class = "AutoTest.TestRunners.MSpec.Tests.TestResource.When_specifiying_the_most_simple_context";
        }

        protected override void Because_of()
        {
            _isContext = _runner.ContainsTestsFor(_assembly, _class);
        }

        [Test]
        public void Should_be_considered_a_context()
        {
            Assert.That(_isContext, Is.True);
        }
    }

    public class When_inspecting_a_class_without_It_fields : Spec
    {
        Runner _runner;
        bool _isContext;
        string _assembly;
        string _class;

        protected override void Establish_context()
        {
            _runner = new Runner();
            _assembly = "AutoTest.TestRunners.MSpec.Tests.TestResource.dll".Path();
            _class = "AutoTest.TestRunners.MSpec.Tests.TestResource.NoContext";
        }

        protected override void Because_of()
        {
            _isContext = _runner.ContainsTestsFor(_assembly, _class);
        }

        [Test]
        public void Should_not_be_considered_a_context()
        {
            Assert.That(_isContext, Is.False);
        }
    }

    public class When_inspecting_an_internal_class_type_with_It_fields : Spec
    {
        Runner _runner;
        bool _isContext;
        string _assembly;
        string _class;

        protected override void Establish_context()
        {
            _runner = new Runner();
            _assembly = "AutoTest.TestRunners.MSpec.Tests.TestResource.dll".Path();
            _class = "AutoTest.TestRunners.MSpec.Tests.TestResource.When_specifiying_an_internal_context";
        }

        protected override void Because_of()
        {
            _isContext = _runner.ContainsTestsFor(_assembly, _class);
        }

        [Test]
        public void Should_not_be_considered_a_context()
        {
            Assert.That(_isContext, Is.False);
        }
    }

    public class When_inspecting_an_It_field : Spec
    {
        Runner _runner;
        bool _isSpecification;
        string _assembly;
        string _field;

        protected override void Establish_context()
        {
            _runner = new Runner();
            _assembly = "AutoTest.TestRunners.MSpec.Tests.TestResource.dll".Path();
            _field = "AutoTest.TestRunners.MSpec.Tests.TestResource.When_specifiying_the_most_simple_context.should_detect_the_context";
        }

        protected override void Because_of()
        {
            _isSpecification = _runner.IsTest(_assembly, _field);
        }

        [Test]
        public void Should_be_considered_a_specification()
        {
            Assert.That(_isSpecification, Is.True);
        }
    }

    public class When_inspecting_any_other_field : Spec
    {
        Runner _runner;
        bool _isSpecification;
        string _assembly;
        string _field;

        protected override void Establish_context()
        {
            _runner = new Runner();
            _assembly = "AutoTest.TestRunners.MSpec.Tests.TestResource.dll".Path();
            _field = "AutoTest.TestRunners.MSpec.Tests.TestResource.When_specifiying_the_most_simple_context.not_an_It_field";
        }

        protected override void Because_of()
        {
            _isSpecification = _runner.IsTest(_assembly, _field);
        }

        [Test]
        public void Should_not_be_considered_a_specification()
        {
            Assert.That(_isSpecification, Is.False);
        }
    }
}