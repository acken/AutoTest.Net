using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AutoTest.Runners.NUnit.Tests.TestResource
{
    [TestFixture]
    public class Fixture1
    {
        [Test]
        public void Should_pass()
        {
            Assert.AreEqual(1, 1);
        }

        [Test]
        public void Should_fail()
        {
            Assert.Fail("failing test");
        }

        [Test]
        public void Should_ignore()
        {
            Assert.Ignore("ignored test");
        }
    }
}
