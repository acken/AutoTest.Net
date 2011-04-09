using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using System.Threading;

namespace AutoTest.TestRunners.MbUnitTests.Tests.TestResource
{
    public class ClassContainingTests
    {
        [Test]
        public void A_passing_test()
        {
            Thread.Sleep(10);
            Assert.AreEqual(2, 2);
        }

        [Test]
        public void A_failing_test()
        {
            Assert.AreEqual(2, 3);
        }

        [Test]
        public void An_inconclusive_test()
        {
            Assert.Inconclusive("inconclusive");
        }
    }
}
