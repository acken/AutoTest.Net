using NUnit.Framework;

namespace AutoTest.TestRunners.MSpec.Tests
{
    [TestFixture]
    public abstract class Spec
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Establish_context();
            Because_of();
        }

        protected virtual void Establish_context()
        {
        }

        protected virtual void Because_of()
        {
        }
    }
}