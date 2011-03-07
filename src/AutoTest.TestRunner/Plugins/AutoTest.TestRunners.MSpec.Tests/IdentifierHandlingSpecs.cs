using NUnit.Framework;

namespace AutoTest.TestRunners.MSpec.Tests
{
    public class When_asking_the_runner_if_it_handles_its_identifier : Spec
    {
        Runner _runner;
        bool _handles;

        protected override void Establish_context()
        {
            _runner = new Runner();
        }

        protected override void Because_of()
        {
            _handles = _runner.Handles(_runner.Identifier.ToLowerInvariant());
        }

        [Test]
        public void Should_handle_the_identifier()
        {
            Assert.That(_handles, Is.True);
        }
    }

    public class When_asking_the_runner_if_it_handles_anything_but_its_identifier : Spec
    {
        Runner _runner;
        bool _handles;

        protected override void Establish_context()
        {
            _runner = new Runner();
        }

        protected override void Because_of()
        {
            _handles = _runner.Handles("foo");
        }

        [Test]
        public void Should_not_handle_the_identifier()
        {
            Assert.That(_handles, Is.False);
        }
    }
}