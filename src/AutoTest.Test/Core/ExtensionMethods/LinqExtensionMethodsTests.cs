using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Test.Core.ExtensionMethods
{
    [TestFixture]
    public class LinqExtensionMethodsTests
    {
        [Test]
        public void Should_execute_method_on_each_item()
        {
            var result = 0;
            new[] {1, 2, 3, 4}.Each(num => result += num);
            result.ShouldEqual(10);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_not_allow_null_actioN()
        {
            new[] {1}.Each(null);
        }

        [Test]
        public void Should_do_nothing_on_null_collection()
        {
            IList<string> emptyList = null;
            var collector = string.Empty;
            emptyList.Each(s => collector += s);
            collector.ShouldEqual(String.Empty);
        }

        [Test]
        public void Should_inject_values_with_seed()
        {
            var ints = new[] {5, 6, 7, 8, 9, 10};
            ints.Inject(0, (sum, n) => sum + n).ShouldEqual(45);
            Func<int, int, int> block = (higest, num) => higest < num ? num : higest;
            ints.Inject(0, block).ShouldEqual(10);
            ints.Inject(11, block).ShouldEqual(11);
            ints.Inject(block).ShouldEqual(10);
        }

        [Test]
        public void Should_return_empty_collection()
        {
            IList<string> nullList = null;
            nullList.FilterNull().Count().ShouldEqual(0);
        }

        [Test]
        public void Should_filter_all_nulls()
        {
            var list = new[] {"hi", null, "", null, "there"};
            list.FilterNull().ToArray().ShouldEqual(new[] {"hi", "", "there"}.ToArray());
        }

        [Test]
        public void Should_not_filter_anything()
        {
            var list = new[] {-1, 0, 1};
            list.FilterNull().ToArray().ShouldEqual(list.ToArray());
        }
    }
}