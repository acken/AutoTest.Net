using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AutoTest.TestRunners.XUnit.Tests.TestResource
{
    public class Class1
    {
        [Fact]
        public void Should_pass()
        {
            Assert.Equal(1, 1);
        }

        [Fact]
        public void Should_fail()
        {
            Assert.Equal(1, 10);
        }
    }
}
