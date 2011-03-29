using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace AutoTest.TestRunners.MbUnitTests.Tests.TestResource
{
    [TestFixture]
    public class ClassContainingTests
    {
        [Test]
        public void A_passing_test()
        {
            Assert.AreEqual(2, 2);
        }
    }
}
